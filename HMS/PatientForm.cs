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

        public PatientForm()
        {
            InitializeComponent();
            LoadPatients();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var p = new Patient
                {
                    FirstName = txtFirst.Text.Trim(),
                    LastName = txtLast.Text.Trim(),
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
            var list = ClinicService.Instance.GetPatients().Select(p => new { p.Id, p.FirstName, p.LastName, DateOfBirth = p.DateOfBirth.ToShortDateString(), p.PhoneNumber }).ToList();
            dgv.DataSource = list;
        }

        private void SearchPatients()
        {
            var q = txtSearch.Text;
            var list = ClinicService.Instance.SearchPatients(q).Select(p => new { p.Id, p.FirstName, p.LastName, DateOfBirth = p.DateOfBirth.ToShortDateString(), p.PhoneNumber }).ToList();
            dgv.DataSource = list;
        }
    }
}
