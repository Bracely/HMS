using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using HMS.Services;
using HMS.Models;

namespace HMS.Controls
{
    // AppointmentsControl - displays upcoming and past appointments in a
    // simple grid. The control is read-only in the demo and includes a
    // refresh button. A production-ready control would allow editing,
    // conflict checking, filtering and calendar integration.
    public class AppointmentsControl : UserControl
    {
        private DataGridView dgv;
        private Button btnRefresh;

        public AppointmentsControl()
        {
            Dock = DockStyle.Fill;
            Initialize();
            LoadAppointments();
        }

        private void Initialize()
        {
            var top = new Panel { Dock = DockStyle.Top, Height = 48, Padding = new Padding(8) };
            btnRefresh = new Button { Text = "Refresh" };
            btnRefresh.Click += (s, e) => LoadAppointments();
            top.Controls.Add(btnRefresh);

            dgv = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            Controls.Add(dgv);
            Controls.Add(top);
        }

        private void LoadAppointments()
        {
            var list = ClinicService.Instance.GetAppointments().Select(a => new { a.AppointmentId, Patient = a.Patient?.FirstName + " " + a.Patient?.LastName, Doctor = a.Doctor?.Name, Date = a.Date, a.Reason }).ToList();
            dgv.DataSource = list;
        }
    }
}
