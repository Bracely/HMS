using System;
using System.Windows.Forms;
using System.Drawing;

namespace HMS
{
    partial class AppointmentForm
    {
        private System.ComponentModel.IContainer components = null;

        private ComboBox cboPatient, cboDoctor;
        private ComboBox cboTimeSlots;
        private DateTimePicker dtpDate;
        private TextBox txtReason;
        private Button btnAdd, btnRefresh, btnEdit, btnDelete;
        private DataGridView dgv;
        private System.Windows.Forms.PictureBox pic;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.WhiteSmoke;
            this.ClientSize = new Size(820, 560);

            // Header (docked)
            var header = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.FromArgb(10, 60, 120) };
            pic = new PictureBox { Left = 12, Top = 12, Width = 56, Height = 56, BackColor = Color.Gainsboro, BorderStyle = BorderStyle.FixedSingle };
            var lblHeader = new Label { Left = 84, Top = 18, AutoSize = true, Text = "Harare Institute of Technology", ForeColor = Color.White, Font = new Font("Segoe UI", 14F, FontStyle.Bold) };
            var lblSub = new Label { Left = 84, Top = 40, AutoSize = true, Text = "Appointment Booking", ForeColor = Color.WhiteSmoke, Font = new Font("Segoe UI", 9F) };
            header.Controls.Add(pic);
            header.Controls.Add(lblHeader);
            header.Controls.Add(lblSub);

            // Top input panel - uses a TableLayoutPanel for responsive layout
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 140, BackColor = Color.Transparent };
            var table = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75F));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

            // Left side: inputs arranged vertically
            var inputs = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 2, Padding = new Padding(8) };
            inputs.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70)); // label
            inputs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F)); // control
            inputs.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));
            inputs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            inputs.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            inputs.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));

            var lblPatient = new Label { Text = "Patient", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9F) };
            cboPatient = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList }; 

            var lblDoctor = new Label { Text = "Doctor", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9F) };
            cboDoctor = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };

            var lblDate = new Label { Text = "Date", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9F) };
            dtpDate = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd" };

            var lblTime = new Label { Text = "Time", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9F) };
            var cboTimeSlots = new ComboBox { Dock = DockStyle.Fill, Name = "cboTimeSlots", DropDownStyle = ComboBoxStyle.DropDownList };
            this.cboTimeSlots = cboTimeSlots;

            var lblReason = new Label { Text = "Reason", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9F) };
            txtReason = new TextBox { Dock = DockStyle.Fill };

            // Add controls to inputs table (row 0)
            inputs.Controls.Add(lblPatient, 0, 0);
            inputs.Controls.Add(cboPatient, 1, 0);
            inputs.Controls.Add(lblDoctor, 2, 0);
            inputs.Controls.Add(cboDoctor, 3, 0);
            // row 1
            inputs.Controls.Add(lblDate, 0, 1);
            inputs.Controls.Add(dtpDate, 1, 1);
            inputs.Controls.Add(lblTime, 2, 1);
            inputs.Controls.Add(cboTimeSlots, 3, 1);
            inputs.Controls.Add(lblReason, 0, 2);
            inputs.Controls.Add(txtReason, 1, 2);

            // Right side: vertical button panel
            var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, Padding = new Padding(8), WrapContents = false }; 
            btnAdd = new Button { Text = "Add", Width = 120, Height = 36, BackColor = Color.FromArgb(10, 60, 120), ForeColor = Color.White, FlatStyle = FlatStyle.Flat }; btnAdd.Click += BtnAdd_Click;
            btnEdit = new Button { Text = "Edit", Width = 120, Height = 36, BackColor = Color.FromArgb(30, 120, 200), ForeColor = Color.White, FlatStyle = FlatStyle.Flat }; btnEdit.Click += BtnEdit_Click;
            btnDelete = new Button { Text = "Delete", Width = 120, Height = 36, BackColor = Color.FromArgb(200, 40, 40), ForeColor = Color.White, FlatStyle = FlatStyle.Flat }; btnDelete.Click += BtnDelete_Click;
            btnRefresh = new Button { Text = "Refresh", Width = 120, Height = 36, BackColor = Color.FromArgb(100, 100, 100), ForeColor = Color.White, FlatStyle = FlatStyle.Flat }; btnRefresh.Click += (s, e) => { LoadLists(); LoadAppointments(); };
            btnPanel.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnRefresh });

            table.Controls.Add(inputs, 0, 0);
            table.Controls.Add(btnPanel, 1, 0);
            topPanel.Controls.Add(table);

            // Data grid - fill the remaining space
            dgv = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White, AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(240, 245, 250) }, SelectionMode = DataGridViewSelectionMode.FullRowSelect };

            // Add to form
            Controls.Add(dgv);
            Controls.Add(topPanel);
            Controls.Add(header);
        }
    }
}
