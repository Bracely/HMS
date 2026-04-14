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
            try { var logo = HMS.Resources.ResourceHelper.LoadLogo(); if (logo != null && this.pic != null) { this.pic.Image = logo; this.pic.SizeMode = PictureBoxSizeMode.StretchImage; } } catch { }
            // Make popup responsive to parent size changes
            EnableResponsivePopup();
            LoadLists();
            LoadAppointments();
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

        private void LoadLists()
        {
            var role = HMS.Services.AuthService.CurrentRole;
            if (role == HMS.Services.UserRole.Student)
            {
                var p = HMS.Services.AuthService.CurrentPatient;
                cboPatient.DataSource = new System.Collections.Generic.List<Patient> { p };
                cboPatient.DisplayMember = "FullName";
                cboPatient.Enabled = false;
            }
            else
            {
                cboPatient.DataSource = ClinicService.Instance.GetPatients();
                cboPatient.DisplayMember = "FullName";
                cboPatient.Enabled = true;
            }

            cboDoctor.DataSource = ClinicService.Instance.GetDoctors();
            cboDoctor.DisplayMember = "Name";

            // Wire events so time slots refresh when doctor or date changes
            cboDoctor.SelectedIndexChanged -= (s, e) => { }; // detach safe
            cboDoctor.SelectedIndexChanged += (s, e) => RefreshTimeSlots();
            dtpDate.ValueChanged -= (s, e) => { };
            dtpDate.ValueChanged += (s, e) => RefreshTimeSlots();

            // initial populate of time slots
            RefreshTimeSlots();

            // Only doctors can edit or delete appointments
            btnEdit.Enabled = (role == HMS.Services.UserRole.Doctor);
            btnDelete.Enabled = (role == HMS.Services.UserRole.Doctor);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var patient = cboPatient.SelectedItem as Patient;
                var doctor = cboDoctor.SelectedItem as Doctor;
                if (cboTimeSlots.SelectedValue == null)
                    throw new ArgumentException("Please select an available time slot.");

                var selected = (DateTime)cboTimeSlots.SelectedValue;
                var ap = new Appointment { Patient = patient, Doctor = doctor, Date = selected, Reason = txtReason.Text.Trim() };

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
            // refresh and include the current appointment time as selectable
            RefreshTimeSlots(ap.Date);
            // select the appointment time
            try { cboTimeSlots.SelectedValue = ap.Date; } catch { }
            txtReason.Text = ap.Reason;
            _editingAppointmentId = ap.AppointmentId;
            btnAdd.Text = "Update";
        }

        private void RefreshTimeSlots(DateTime? include = null)
        {
            cboTimeSlots.Items.Clear();
            var doctor = cboDoctor.SelectedItem as Doctor;
            if (doctor == null) return;

            var slots = ClinicService.Instance.GetAvailableSlots(doctor.Id, dtpDate.Value.Date);
            // ensure include is present (for editing existing appointment)
            if (include.HasValue && !slots.Any(s => s == include.Value))
            {
                slots.Insert(0, include.Value);
            }

            var list = slots.Select(dt => new { Time = dt.ToString("HH:mm"), Value = dt }).ToList();
            cboTimeSlots.DisplayMember = "Time";
            cboTimeSlots.ValueMember = "Value";
            cboTimeSlots.DataSource = list;
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
