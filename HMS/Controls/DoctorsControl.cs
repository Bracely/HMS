using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using HMS.Services;
using HMS.Models;

namespace HMS.Controls
{
    // DoctorsControl - simple management view for doctors. The control
    // lists doctors from ClinicService and exposes a refresh action.
    // Note: For real workflows this control should provide edit forms,
    // validation and a way to manage doctor-specific settings (email,
    // specialization, etc.).
    public class DoctorsControl : UserControl
    {
        private DataGridView dgv;
        private Button btnRefresh;

        public DoctorsControl()
        {
            Dock = DockStyle.Fill;
            Initialize();
            LoadDoctors();
        }

        private void Initialize()
        {
            var top = new Panel { Dock = DockStyle.Top, Height = 48, Padding = new Padding(8) };
            btnRefresh = new Button { Text = "Refresh" };
            btnRefresh.Click += (s, e) => LoadDoctors();
            top.Controls.Add(btnRefresh);

            dgv = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            Controls.Add(dgv);
            Controls.Add(top);
        }
        //Doctor list components
        private void LoadDoctors()
        {
            var list = ClinicService.Instance.GetDoctors().Select(d => new { d.Id, d.Name, d.Email, d.Specialization }).ToList();
            dgv.DataSource = list;
        }
    }
}
