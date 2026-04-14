using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using HMS.Services;
using HMS.Models;

namespace HMS.Controls
{
    // BillingControl - provides a view of generated bills with a simple
    // refresh action. It surfaces bill date, amount and patient for quick
    // review. In a complete system this control would support creating
    // invoices, recording payments and integrating with accounting.
    public class BillingControl : UserControl
    {
        private DataGridView dgv;
        private Button btnRefresh;

        public BillingControl()
        {
            Dock = DockStyle.Fill;
            Initialize();
            LoadBills();
        }

        private void Initialize()
        {
            var top = new Panel { Dock = DockStyle.Top, Height = 48, Padding = new Padding(8) };
            btnRefresh = new Button { Text = "Refresh" };
            btnRefresh.Click += (s, e) => LoadBills();
            top.Controls.Add(btnRefresh);

            dgv = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            Controls.Add(dgv);
            Controls.Add(top);
        }

        private void LoadBills()
        {
            var list = ClinicService.Instance.GetBills().Select(b => new { b.BillId, Patient = b.Patient?.FirstName + " " + b.Patient?.LastName, b.Amount, Date = b.Date }).ToList();
            dgv.DataSource = list;
        }
    }
}
