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
            LoadDoctors();
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
