using System;
using System.Linq;
using System.Windows.Forms;
using HMS.Models;
using HMS.Services;

namespace HMS
{
    public partial class AppointmentForm : Form
    {
        private int _editingAppointmentId = 0;

        public AppointmentForm()
        {
            InitializeComponent();
            LoadLists();
            LoadAppointments();
        }

        private void LoadLists()
        {
            cboPatient.DataSource = ClinicService.Instance.GetPatients();
            cboPatient.DisplayMember = "FullName";

            cboDoctor.DataSource = ClinicService.Instance.GetDoctors();
            cboDoctor.DisplayMember = "Name";
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var patient = cboPatient.SelectedItem as Patient;
                var doctor = cboDoctor.SelectedItem as Doctor;
                var ap = new Appointment { Patient = patient, Doctor = doctor, Date = dtpDate.Value, Reason = txtReason.Text.Trim() };

                if (_editingAppointmentId == 0)
                {
                    ClinicService.Instance.AddAppointment(ap);
                    MessageBox.Show("Appointment added.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    ap.AppointmentId = _editingAppointmentId;
                    ClinicService.Instance.UpdateAppointment(ap);
                    MessageBox.Show("Appointment updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _editingAppointmentId = 0;
                    btnAdd.Text = "Add";
                }

                ClearInputs();
                LoadAppointments();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ClearInputs()
        {
            txtReason.Text = string.Empty;
            dtpDate.Value = DateTime.Now;
            _editingAppointmentId = 0;
            btnAdd.Text = "Add";
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Select an appointment to edit.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var idObj = dgv.CurrentRow.Cells["AppointmentId"]?.Value ?? dgv.CurrentRow.Cells[0].Value;
            if (!int.TryParse(idObj?.ToString(), out var id))
            {
                MessageBox.Show("Unable to determine selected appointment ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var ap = ClinicService.Instance.GetAppointments().FirstOrDefault(a => a.AppointmentId == id);
            if (ap == null) return;

            // Select patient in combo
            for (int i = 0; i < cboPatient.Items.Count; i++)
            {
                if ((cboPatient.Items[i] as Patient)?.Id == ap.Patient.Id)
                {
                    cboPatient.SelectedIndex = i;
                    break;
                }
            }

            for (int i = 0; i < cboDoctor.Items.Count; i++)
            {
                if ((cboDoctor.Items[i] as Doctor)?.Id == ap.Doctor.Id)
                {
                    cboDoctor.SelectedIndex = i;
                    break;
                }
            }

            dtpDate.Value = ap.Date;
            txtReason.Text = ap.Reason;
            _editingAppointmentId = ap.AppointmentId;
            btnAdd.Text = "Update";
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Select an appointment to delete.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var idObj = dgv.CurrentRow.Cells["AppointmentId"]?.Value ?? dgv.CurrentRow.Cells[0].Value;
            if (!int.TryParse(idObj?.ToString(), out var id))
            {
                MessageBox.Show("Unable to determine selected appointment ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("Are you sure you want to delete the selected appointment?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            if (ClinicService.Instance.DeleteAppointment(id))
            {
                MessageBox.Show("Appointment deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadAppointments();
            }
            else
            {
                MessageBox.Show("Failed to delete appointment.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void LoadAppointments()
        {
            var list = ClinicService.Instance.GetAppointments().Select(a => new { a.AppointmentId, Patient = a.Patient.FullName, Doctor = a.Doctor.Name, Date = a.Date.ToString("g"), a.Reason }).ToList();
            dgv.DataSource = list;
        }
    }
}
