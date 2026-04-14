using System;
using System.Windows.Forms;
using System.Drawing;

namespace HMS
{
    partial class BillingForm
    {
        private System.ComponentModel.IContainer components = null;

        private ComboBox cboPatient;
        private NumericUpDown nudAmount;
        private DateTimePicker dtpDate;
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.BackColor = Color.WhiteSmoke;
            this.ClientSize = new Size(820, 600);

            // Header
            var header = new Panel { Dock = DockStyle.Top, Height = 72, BackColor = Color.FromArgb(10, 60, 120) };
            var headerFlow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, Padding = new Padding(12) };
            pic = new PictureBox { Size = new Size(56, 56), Margin = new Padding(6), BackColor = Color.Gainsboro, BorderStyle = BorderStyle.FixedSingle };
            var headerTexts = new Panel { AutoSize = true };
            var lblHeader = new Label { AutoSize = true, Text = "Harare Institute of Technology", ForeColor = Color.White, Font = new Font("Segoe UI", 14F, FontStyle.Bold), Dock = DockStyle.Top };
            var lblSub = new Label { AutoSize = true, Text = "Billing", ForeColor = Color.WhiteSmoke, Font = new Font("Segoe UI", 9F), Dock = DockStyle.Top };
            headerTexts.Controls.Add(lblSub); headerTexts.Controls.Add(lblHeader);
            headerFlow.Controls.Add(pic); headerFlow.Controls.Add(headerTexts);
            header.Controls.Add(headerFlow);

            // Inputs - responsive
            var inputsFlow = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 88, Padding = new Padding(8), FlowDirection = FlowDirection.LeftToRight, WrapContents = true };
            var pPatient = new Panel { Width = 360, Height = 56, Margin = new Padding(6) };
            var lblPatient = new Label { Text = "Patient", Dock = DockStyle.Top, AutoSize = true };
            cboPatient = new ComboBox { Dock = DockStyle.Top, Width = 340, DropDownStyle = ComboBoxStyle.DropDownList };
            pPatient.Controls.Add(cboPatient); pPatient.Controls.Add(lblPatient);

            var pAmount = new Panel { Width = 220, Height = 56, Margin = new Padding(6) };
            var lblAmount = new Label { Text = "Amount", Dock = DockStyle.Top, AutoSize = true };
            nudAmount = new NumericUpDown { Dock = DockStyle.Top, Width = 200, DecimalPlaces = 2, Maximum = 1000000 };
            pAmount.Controls.Add(nudAmount); pAmount.Controls.Add(lblAmount);

            var pDate = new Panel { Width = 220, Height = 56, Margin = new Padding(6) };
            var lblDate = new Label { Text = "Date", Dock = DockStyle.Top, AutoSize = true };
            dtpDate = new DateTimePicker { Dock = DockStyle.Top, Width = 200 };
            pDate.Controls.Add(dtpDate); pDate.Controls.Add(lblDate);

            inputsFlow.Controls.AddRange(new Control[] { pPatient, pAmount, pDate });

            // Main grid
            dgv = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White, AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(240, 245, 250) }, SelectionMode = DataGridViewSelectionMode.FullRowSelect };

            // Bottom toolbar
            var bottomToolbar = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 56, Padding = new Padding(8), FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
            btnAdd = new Button { Text = "Add", AutoSize = true, BackColor = Color.FromArgb(10, 60, 120), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Margin = new Padding(6) };
            btnAdd.Click += BtnAdd_Click;
            btnEdit = new Button { Text = "Edit", AutoSize = true, BackColor = Color.FromArgb(30, 120, 200), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Margin = new Padding(6) };
            btnEdit.Click += BtnEdit_Click;
            btnDelete = new Button { Text = "Delete", AutoSize = true, BackColor = Color.FromArgb(200, 40, 40), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Margin = new Padding(6) };
            btnDelete.Click += BtnDelete_Click;
            btnRefresh = new Button { Text = "Refresh", AutoSize = true, BackColor = Color.FromArgb(100, 100, 100), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Margin = new Padding(6) };
            btnRefresh.Click += (s, e) => { LoadPatients(); LoadBills(); };
            bottomToolbar.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnRefresh });

            Controls.Add(dgv);
            Controls.Add(bottomToolbar);
            Controls.Add(inputsFlow);
            Controls.Add(header);
        }
    }
}
