using System;
using System.Linq;
using System.Windows.Forms;
using HMS.Models;
using HMS.Services;

namespace HMS
{

    // Simple patient management form (designer-based)
    public partial class PatientForm : Form
    {
        // When editing a patient this holds the id, otherwise 0
        private int _editingPatientId = 0;

        // Allow constructing the form to edit a specific patient by id.
        // This is useful for embedding the patient editor from other
        // controls (e.g., PatientsControl double-click).
        public PatientForm(int patientId) : this()
        {
            LoadPatient(patientId);
        }

        public PatientForm()
        {
            InitializeComponent();
            // load logo after designer initialization
            try { var logo = HMS.Resources.ResourceHelper.LoadLogo(); if (logo != null && this.pic != null) { this.pic.Image = logo; this.pic.SizeMode = PictureBoxSizeMode.StretchImage; } } catch { }
            // Make this popup responsive to parent size changes (not applied to LoginForm)
            EnableResponsivePopup();
            LoadPatients();
        }

        // Load a patient into the form fields and switch to edit mode.
        private void LoadPatient(int id)
        {
            var patient = ClinicService.Instance.GetPatients().FirstOrDefault(p => p.Id == id);
            if (patient == null) return;

            txtFirst.Text = patient.FirstName;
            txtStudentId.Text = patient.StudentId;
            txtLast.Text = patient.LastName;
            txtPhone.Text = patient.PhoneNumber;
            dtpDob.Value = patient.DateOfBirth;
            _editingPatientId = patient.Id;
            btnAdd.Text = "Update";
        }

        // Responsive popup helpers
        private void EnableResponsivePopup()
        {
            // allow resizing and maximize
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;

            this.Shown += (s, e) => AttachToOwner();
            this.FormClosed += (s, e) => DetachFromOwner();
        }

        private void AttachToOwner()
        {
            if (this.Owner != null)
            {
                this.Owner.SizeChanged += Owner_SizeChanged;
                AdjustToOwner();
            }
            else
            {
                // If no owner, size to 80% of the working area
                var wa = Screen.PrimaryScreen.WorkingArea;
                this.Size = new System.Drawing.Size((int)(wa.Width * 0.8), (int)(wa.Height * 0.8));
                this.CenterToScreen();
            }
        }

        private void DetachFromOwner()
        {
            if (this.Owner != null)
            {
                this.Owner.SizeChanged -= Owner_SizeChanged;
            }
        }

        private void Owner_SizeChanged(object? sender, EventArgs e)
        {
            AdjustToOwner();
        }

        private void AdjustToOwner()
        {
            if (this.Owner == null) return;
            try
            {
                var o = this.Owner;
                var w = Math.Max(600, (int)(o.ClientSize.Width * 0.8));
                var h = Math.Max(400, (int)(o.ClientSize.Height * 0.8));
                this.Size = new System.Drawing.Size(w, h);
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new System.Drawing.Point(o.Location.X + (o.Width - this.Width) / 2, o.Location.Y + (o.Height - this.Height) / 2);
            }
            catch { }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var p = new Patient
                {
                    FirstName = txtFirst.Text.Trim(),
                    LastName = txtLast.Text.Trim(),
                    StudentId = txtStudentId.Text.Trim(),
                    DateOfBirth = dtpDob.Value.Date,
                    PhoneNumber = txtPhone.Text.Trim()
                };

                if (_editingPatientId == 0)
                {
                    ClinicService.Instance.AddPatient(p);
                    MessageBox.Show("Patient added.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    p.Id = _editingPatientId;
                    ClinicService.Instance.UpdatePatient(p);
                    MessageBox.Show("Patient updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _editingPatientId = 0;
                    btnAdd.Text = "Add";
                }

                ClearInputs();
                LoadPatients();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ClearInputs()
        {
            txtFirst.Text = string.Empty;
            txtLast.Text = string.Empty;
            txtStudentId.Text = string.Empty;
            txtPhone.Text = string.Empty;
            dtpDob.Value = DateTime.Today;
            _editingPatientId = 0;
            btnAdd.Text = "Add";
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Select a patient to edit.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var idObj = dgv.CurrentRow.Cells["Id"]?.Value ?? dgv.CurrentRow.Cells[0].Value;
            if (!int.TryParse(idObj?.ToString(), out var id))
            {
                MessageBox.Show("Unable to determine selected patient ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var patient = ClinicService.Instance.GetPatients().FirstOrDefault(p => p.Id == id);
            if (patient == null) return;

            txtFirst.Text = patient.FirstName;
            txtStudentId.Text = patient.StudentId;
            txtLast.Text = patient.LastName;
            txtPhone.Text = patient.PhoneNumber;
            dtpDob.Value = patient.DateOfBirth;
            _editingPatientId = patient.Id;
            btnAdd.Text = "Update";
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Select a patient to delete.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var idObj = dgv.CurrentRow.Cells["Id"]?.Value ?? dgv.CurrentRow.Cells[0].Value;
            if (!int.TryParse(idObj?.ToString(), out var id))
            {
                MessageBox.Show("Unable to determine selected patient ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("Are you sure you want to delete the selected patient?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            if (ClinicService.Instance.DeletePatient(id))
            {
                MessageBox.Show("Patient deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadPatients();
            }
            else
            {
                MessageBox.Show("Failed to delete patient.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void LoadPatients()
        {
            var role = HMS.Services.AuthService.CurrentRole;
            if (role == HMS.Services.UserRole.Student)
            {
                var p = HMS.Services.AuthService.CurrentPatient;
                var list = new[] { new { p.Id, p.FirstName, p.LastName, p.StudentId, DateOfBirth = p.DateOfBirth.ToShortDateString(), p.PhoneNumber } };
                dgv.DataSource = list;

                // Disable editing for students
                btnAdd.Enabled = false;
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
            }
            else
            {
                var list = ClinicService.Instance.GetPatients().Select(p => new { p.Id, p.FirstName, p.LastName, p.StudentId, DateOfBirth = p.DateOfBirth.ToShortDateString(), p.PhoneNumber }).ToList();
                dgv.DataSource = list;
                btnAdd.Enabled = true;
                btnEdit.Enabled = true;
                btnDelete.Enabled = true;
            }
        }

        private void SearchPatients()
        {
            var q = txtSearch.Text;
            var list = ClinicService.Instance.SearchPatients(q).Select(p => new { p.Id, p.FirstName, p.LastName, p.StudentId, DateOfBirth = p.DateOfBirth.ToShortDateString(), p.PhoneNumber }).ToList();
            dgv.DataSource = list;
        }
    }
}
