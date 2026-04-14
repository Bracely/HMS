using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using HMS.Services;
using HMS.Models;

namespace HMS
{
    // Doctor portal for logging illnesses and viewing simple statistics
    public class DoctorDashboardForm : Form
    {
        private ComboBox cboPatient;
        private TextBox txtDiagnosis, txtNotes;
        private DateTimePicker dtpDate;
        private Button btnAddRecord, btnRefresh;
        private Button btnEditRecord, btnDeleteRecord, btnPatientReport;
        private DataGridView dgvRecords;
        private ListBox lstStats;
        private Button btnExport;
        private int _editingRecordId = 0;

        public DoctorDashboardForm()
        {
            Text = "Doctor Portal - HIT";
            Width = 900;
            Height = 600;
            InitializeComponents();
            LoadPatients();
            LoadRecords();
            LoadStats();
            // Make this portal responsive when shown as a dialog
            EnableResponsivePopup();
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
            else { var wa = Screen.PrimaryScreen.WorkingArea; this.Size = new System.Drawing.Size((int)(wa.Width * 0.85), (int)(wa.Height * 0.85)); this.CenterToScreen(); }
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
                var w = Math.Max(700, (int)(o.ClientSize.Width * 0.85));
                var h = Math.Max(500, (int)(o.ClientSize.Height * 0.85));
                this.Size = new System.Drawing.Size(w, h);
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new System.Drawing.Point(o.Location.X + (o.Width - this.Width) / 2, o.Location.Y + (o.Height - this.Height) / 2);
            }
            catch { }
        }

        private void InitializeComponents()
        {
            var header = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(10, 60, 120) };
            var lbl = new Label { Text = "Doctor Portal", ForeColor = Color.White, Left = 12, Top = 18, Font = new Font("Segoe UI", 12F, FontStyle.Bold) };
            header.Controls.Add(lbl);

            // Top panel: inputs and actions arranged responsively
            var pnl = new Panel { Dock = DockStyle.Top, Height = 140, Padding = new Padding(8) };
            var table = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75F));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

            var inputs = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 2, Padding = new Padding(8) };
            inputs.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));
            inputs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            inputs.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));
            inputs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            inputs.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            inputs.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));

            var lblPatient = new Label { Text = "Patient", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9F) };
            cboPatient = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };

            var lblDate = new Label { Text = "Date", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9F) };
            dtpDate = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd HH:mm" };

            var lblDiagnosis = new Label { Text = "Diagnosis", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9F) };
            txtDiagnosis = new TextBox { Dock = DockStyle.Fill };

            var lblNotes = new Label { Text = "Notes", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9F) };
            txtNotes = new TextBox { Dock = DockStyle.Fill };

            inputs.Controls.Add(lblPatient, 0, 0);
            inputs.Controls.Add(cboPatient, 1, 0);
            inputs.Controls.Add(lblDate, 2, 0);
            inputs.Controls.Add(dtpDate, 3, 0);
            inputs.Controls.Add(lblDiagnosis, 0, 1);
            inputs.Controls.Add(txtDiagnosis, 1, 1);
            inputs.Controls.Add(lblNotes, 2, 1);
            inputs.Controls.Add(txtNotes, 3, 1);

            var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, Padding = new Padding(8), WrapContents = false };
            btnAddRecord = new Button { Text = "Add Record", Width = 140, Height = 36, BackColor = Color.FromArgb(10, 60, 120), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnAddRecord.Click += BtnAddRecord_Click;
            btnRefresh = new Button { Text = "Refresh", Width = 120, Height = 36 };
            btnRefresh.Click += (s, e) => { LoadRecords(); LoadStats(); };
            btnExport = new Button { Text = "Export CSV", Width = 140, Height = 36 };
            btnExport.Click += BtnExport_Click;
            btnEditRecord = new Button { Text = "Edit Record", Width = 140, Height = 36 };
            btnEditRecord.Click += BtnEditRecord_Click;
            btnDeleteRecord = new Button { Text = "Delete Record", Width = 140, Height = 36, BackColor = Color.FromArgb(200, 40, 40), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnDeleteRecord.Click += BtnDeleteRecord_Click;
            btnPatientReport = new Button { Text = "Patient Report", Width = 140, Height = 36 };
            btnPatientReport.Click += BtnPatientReport_Click;
            btnPanel.Controls.AddRange(new Control[] { btnAddRecord, btnRefresh, btnExport, btnEditRecord, btnDeleteRecord, btnPatientReport });

            table.Controls.Add(inputs, 0, 0);
            table.Controls.Add(btnPanel, 1, 0);
            pnl.Controls.Add(table);

            dgvRecords = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

            lstStats = new ListBox { Dock = DockStyle.Right, Width = 200 };

            Controls.Add(dgvRecords);
            Controls.Add(lstStats);
            Controls.Add(pnl);
            Controls.Add(header);
        }

        private void LoadPatients()
        {
            cboPatient.DataSource = ClinicService.Instance.GetPatients();
            cboPatient.DisplayMember = "FullName";
        }

        private void LoadRecords()
        {
            if (AuthService.CurrentDoctor == null)
            {
                dgvRecords.DataSource = null;
                return;
            }

            var list = ClinicService.Instance.GetIllnessRecordsForDoctor(AuthService.CurrentDoctor.Id)
                .Select(r => new { r.Id, Patient = r.Patient.FullName, r.Date, r.Diagnosis, r.Notes }).ToList();
            dgvRecords.DataSource = list;
            dgvRecords.SelectionChanged -= DgvRecords_SelectionChanged;
            dgvRecords.SelectionChanged += DgvRecords_SelectionChanged;
            UpdateRecordButtons();
        }

        private void LoadStats()
        {
            lstStats.Items.Clear();
            if (AuthService.CurrentDoctor == null) return;

            var records = ClinicService.Instance.GetIllnessRecordsForDoctor(AuthService.CurrentDoctor.Id);
            var stats = records.GroupBy(r => (r.Diagnosis ?? string.Empty).Trim().ToLowerInvariant()).Where(g => !string.IsNullOrEmpty(g.Key)).OrderByDescending(g => g.Count()).Take(20);
            foreach (var g in stats)
            {
                lstStats.Items.Add($"{g.Key}: {g.Count()}");
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (AuthService.CurrentDoctor == null)
            {
                MessageBox.Show("No doctor logged in.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var records = ClinicService.Instance.GetIllnessRecordsForDoctor(AuthService.CurrentDoctor.Id);
            if (records == null || records.Count == 0)
            {
                MessageBox.Show("No records to export.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var dlg = new SaveFileDialog { Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*", FileName = $"Doctor_{AuthService.CurrentDoctor.Id}_Records.csv" };
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                using var sw = new StreamWriter(dlg.FileName);
                sw.WriteLine("Id,Patient,Date,Diagnosis,Notes");
                foreach (var r in records)
                {
                    var line = $"{r.Id},\"{r.Patient.FullName}\",\"{r.Date:yyyy-MM-dd HH:mm}\",\"{r.Diagnosis?.Replace("\"", """")}\",\"{r.Notes?.Replace("\"", """")}\"";
                    sw.WriteLine(line);
                }
                MessageBox.Show("Exported successfully.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to export: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAddRecord_Click(object sender, EventArgs e)
        {
            try
            {
                var patient = cboPatient.SelectedItem as Patient;
                var doctor = AuthService.CurrentDoctor;
                if (patient == null || doctor == null) throw new Exception("Patient and doctor must be selected.");
                if (_editingRecordId == 0)
                {
                    var rec = new IllnessRecord { Patient = patient, Doctor = doctor, Date = dtpDate.Value, Diagnosis = txtDiagnosis.Text.Trim(), Notes = txtNotes.Text.Trim() };
                    ClinicService.Instance.AddIllnessRecord(rec);
                    MessageBox.Show("Record added.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var rec = new IllnessRecord { Id = _editingRecordId, Patient = patient, Doctor = doctor, Date = dtpDate.Value, Diagnosis = txtDiagnosis.Text.Trim(), Notes = txtNotes.Text.Trim() };
                    ClinicService.Instance.UpdateIllnessRecord(rec);
                    MessageBox.Show("Record updated.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _editingRecordId = 0;
                    btnAddRecord.Text = "Add Record";
                }
                ClearRecordInputs();
                LoadRecords();
                LoadStats();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ClearRecordInputs()
        {
            dtpDate.Value = DateTime.Now;
            txtDiagnosis.Text = string.Empty;
            txtNotes.Text = string.Empty;
            _editingRecordId = 0;
            btnAddRecord.Text = "Add Record";
            UpdateRecordButtons();
        }

        private void DgvRecords_SelectionChanged(object sender, EventArgs e)
        {
            UpdateRecordButtons();
        }

        private void UpdateRecordButtons()
        {
            var role = AuthService.CurrentRole;
            var hasSelection = dgvRecords.CurrentRow != null;
            btnEditRecord.Enabled = hasSelection && role == UserRole.Doctor;
            btnDeleteRecord.Enabled = hasSelection && role == UserRole.Doctor;
            btnPatientReport.Enabled = hasSelection;
        }

        private void BtnEditRecord_Click(object sender, EventArgs e)
        {
            if (dgvRecords.CurrentRow == null) return;
            var idObj = dgvRecords.CurrentRow.Cells["Id"]?.Value ?? dgvRecords.CurrentRow.Cells[0].Value;
            if (!int.TryParse(idObj?.ToString(), out var id)) return;
            var rec = ClinicService.Instance.GetIllnessRecords().FirstOrDefault(r => r.Id == id);
            if (rec == null) return;
            // populate inputs
            _editingRecordId = rec.Id;
            // select patient in combo
            for (int i = 0; i < cboPatient.Items.Count; i++)
            {
                if ((cboPatient.Items[i] as Patient)?.Id == rec.Patient.Id)
                {
                    cboPatient.SelectedIndex = i;
                    break;
                }
            }
            dtpDate.Value = rec.Date;
            txtDiagnosis.Text = rec.Diagnosis;
            txtNotes.Text = rec.Notes;
            btnAddRecord.Text = "Update Record";
            UpdateRecordButtons();
        }

        private void BtnDeleteRecord_Click(object sender, EventArgs e)
        {
            if (dgvRecords.CurrentRow == null) return;
            var idObj = dgvRecords.CurrentRow.Cells["Id"]?.Value ?? dgvRecords.CurrentRow.Cells[0].Value;
            if (!int.TryParse(idObj?.ToString(), out var id)) return;
            var confirm = MessageBox.Show("Delete selected record?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;
            if (ClinicService.Instance.DeleteIllnessRecord(id))
            {
                MessageBox.Show("Record deleted.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadRecords();
                LoadStats();
            }
            else
            {
                MessageBox.Show("Failed to delete record.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnPatientReport_Click(object sender, EventArgs e)
        {
            if (dgvRecords.CurrentRow == null) return;
            var idObj = dgvRecords.CurrentRow.Cells["Id"]?.Value ?? dgvRecords.CurrentRow.Cells[0].Value;
            if (!int.TryParse(idObj?.ToString(), out var id)) return;
            var rec = ClinicService.Instance.GetIllnessRecords().FirstOrDefault(r => r.Id == id);
            if (rec == null) return;
            using var f = new PatientReportForm(rec.Patient.Id);
            f.ShowDialog(this);
        }
    }
}
