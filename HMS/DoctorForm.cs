using System;
using System.Linq;
using System.Windows.Forms;
using HMS.Models;
using HMS.Services;

namespace HMS
{
    public partial class DoctorForm : Form
    {
        private int _editingDoctorId = 0;

        public DoctorForm()
        {
            InitializeComponent();
            try { var logo = HMS.Resources.ResourceHelper.LoadLogo(); if (logo != null && this.pic != null) { this.pic.Image = logo; this.pic.SizeMode = PictureBoxSizeMode.StretchImage; } } catch { }
            // Make popup responsive to parent size changes
            EnableResponsivePopup();
            LoadDoctors();
        }

        private void EnableResponsivePopup()
        {
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
            else { var wa = Screen.PrimaryScreen.WorkingArea; this.Size = new System.Drawing.Size((int)(wa.Width * 0.8), (int)(wa.Height * 0.8)); this.CenterToScreen(); }
        }

        private void DetachFromOwner()
        {
            if (this.Owner != null) this.Owner.SizeChanged -= Owner_SizeChanged;
        }

        private void Owner_SizeChanged(object? sender, EventArgs e) => AdjustToOwner();

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
                var d = new Doctor { Name = txtName.Text.Trim(), Specialization = txtSpec.Text.Trim() };

                if (_editingDoctorId == 0)
                {
                    ClinicService.Instance.AddDoctor(d);
                    MessageBox.Show("Doctor added.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    d.Id = _editingDoctorId;
                    ClinicService.Instance.UpdateDoctor(d);
                    MessageBox.Show("Doctor updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _editingDoctorId = 0;
                    btnAdd.Text = "Add";
                }

                ClearInputs();
                LoadDoctors();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ClearInputs()
        {
            txtName.Text = string.Empty;
            txtSpec.Text = string.Empty;
            _editingDoctorId = 0;
            btnAdd.Text = "Add";
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Select a doctor to edit.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var idObj = dgv.CurrentRow.Cells["Id"]?.Value ?? dgv.CurrentRow.Cells[0].Value;
            if (!int.TryParse(idObj?.ToString(), out var id))
            {
                MessageBox.Show("Unable to determine selected doctor ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var doctor = ClinicService.Instance.GetDoctors().FirstOrDefault(d => d.Id == id);
            if (doctor == null) return;

            txtName.Text = doctor.Name;
            txtSpec.Text = doctor.Specialization;
            _editingDoctorId = doctor.Id;
            btnAdd.Text = "Update";
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Select a doctor to delete.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var idObj = dgv.CurrentRow.Cells["Id"]?.Value ?? dgv.CurrentRow.Cells[0].Value;
            if (!int.TryParse(idObj?.ToString(), out var id))
            {
                MessageBox.Show("Unable to determine selected doctor ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("Are you sure you want to delete the selected doctor?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            if (ClinicService.Instance.DeleteDoctor(id))
            {
                MessageBox.Show("Doctor deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDoctors();
            }
            else
            {
                MessageBox.Show("Failed to delete doctor.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void LoadDoctors()
        {
            var list = ClinicService.Instance.GetDoctors().Select(d => new { d.Id, d.Name, d.Specialization }).ToList();
            dgv.DataSource = list;
        }
    }
}
