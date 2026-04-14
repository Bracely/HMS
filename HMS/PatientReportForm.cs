using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using HMS.Services;
using HMS.Models;

namespace HMS
{
    public class PatientReportForm : Form
    {
        private int _patientId;
        private DataGridView dgvStats;
        private ListBox lstRecords;
        private Button btnExportCsv;

        public PatientReportForm(int patientId)
        {
            _patientId = patientId;
            Text = "Patient Report";
            Width = 800;
            Height = 600;
            InitializeComponents();
            // Make popup responsive to parent size changes
            EnableResponsivePopup();
            LoadReport();
        }

        private void InitializeComponents()
        {
            var header = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(10, 60, 120) };
            var lbl = new Label { Text = "Patient Report", ForeColor = Color.White, Left = 12, Top = 12, Font = new Font("Segoe UI", 11F, FontStyle.Bold) };
            header.Controls.Add(lbl);

            dgvStats = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            dgvStats.Columns.Add("Diagnosis", "Diagnosis");
            dgvStats.Columns.Add("Count", "Count");

            lstRecords = new ListBox { Dock = DockStyle.Right, Width = 320 };

            btnExportCsv = new Button { Text = "Export CSV", Dock = DockStyle.Bottom, Height = 30 };
            btnExportCsv.Click += BtnExportCsv_Click;

            Controls.Add(dgvStats);
            Controls.Add(lstRecords);
            Controls.Add(btnExportCsv);
            Controls.Add(header);
        }

        private void LoadReport()
        {
            var patient = ClinicService.Instance.GetPatients().FirstOrDefault(p => p.Id == _patientId);
            if (patient == null)
            {
                MessageBox.Show("Patient not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }

            Text = $"Patient Report - {patient.FullName} ({patient.StudentId})";

            var records = ClinicService.Instance.GetIllnessRecordsForPatient(_patientId);
            lstRecords.Items.Clear();
            foreach (var r in records.OrderByDescending(x => x.Date))
            {
                lstRecords.Items.Add($"{r.Date:yyyy-MM-dd}: {r.Diagnosis} - {r.Notes}");
            }

            var stats = records.GroupBy(r => (r.Diagnosis ?? string.Empty).Trim()).Where(g => !string.IsNullOrEmpty(g.Key)).OrderByDescending(g => g.Count());
            dgvStats.Rows.Clear();
            foreach (var g in stats)
            {
                dgvStats.Rows.Add(g.Key, g.Count());
            }
        }

        private void BtnExportCsv_Click(object sender, EventArgs e)
        {
            var records = ClinicService.Instance.GetIllnessRecordsForPatient(_patientId);
            if (records == null || records.Count == 0)
            {
                MessageBox.Show("No records to export.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var dlg = new SaveFileDialog { Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*", FileName = $"Patient_{_patientId}_Report.csv" };
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                using var sw = new System.IO.StreamWriter(dlg.FileName);
                sw.WriteLine("Date,Diagnosis,Notes,Doctor");
                foreach (var r in records.OrderByDescending(x => x.Date))
                {
                    sw.WriteLine($"{r.Date:yyyy-MM-dd HH:mm},\"{r.Diagnosis}\",\"{r.Notes}\",\"{r.Doctor?.Name}\"");
                }
                MessageBox.Show("Exported successfully.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to export: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
    }
}
