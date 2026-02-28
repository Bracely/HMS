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
            this.ClientSize = new Size(740, 520);

            // Header
            var header = new Panel { Left = 0, Top = 0, Width = this.ClientSize.Width, Height = 70, BackColor = Color.FromArgb(10, 60, 120) };
            var pic = new PictureBox { Left = 12, Top = 8, Width = 56, Height = 56, BackColor = Color.Gainsboro, BorderStyle = BorderStyle.FixedSingle };
            var logoImg = HMS.Resources.ResourceHelper.LoadLogo();
            if (logoImg != null)
            {
                pic.Image = logoImg;
                pic.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            var lblHeader = new Label { Left = 84, Top = 18, AutoSize = true, Text = "Harare Institute of Technology", ForeColor = Color.White, Font = new Font("Segoe UI", 14F, FontStyle.Bold) };
            var lblSub = new Label { Left = 84, Top = 38, AutoSize = true, Text = "Billing", ForeColor = Color.WhiteSmoke, Font = new Font("Segoe UI", 9F) };
            header.Controls.Add(pic);
            header.Controls.Add(lblHeader);
            header.Controls.Add(lblSub);

            var lblPatient = new Label { Left = 12, Top = 86, Text = "Patient", Font = new Font("Segoe UI", 9F) };
            cboPatient = new ComboBox { Left = 80, Top = 82, Width = 320, DropDownStyle = ComboBoxStyle.DropDownList };

            var lblAmount = new Label { Left = 12, Top = 120, Text = "Amount", Font = new Font("Segoe UI", 9F) };
            nudAmount = new NumericUpDown { Left = 80, Top = 116, Width = 140, DecimalPlaces = 2, Maximum = 1000000 };

            var lblDate = new Label { Left = 240, Top = 120, Text = "Date", Font = new Font("Segoe UI", 9F) };
            dtpDate = new DateTimePicker { Left = 280, Top = 116, Width = 160 };

            btnAdd = new Button { Left = 460, Top = 82, Width = 100, Text = "Add", BackColor = Color.FromArgb(10, 60, 120), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button { Left = 580, Top = 82, Width = 100, Text = "Edit", BackColor = Color.FromArgb(30, 120, 200), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button { Left = 580, Top = 122, Width = 100, Text = "Delete", BackColor = Color.FromArgb(200, 40, 40), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnDelete.Click += BtnDelete_Click;

            btnRefresh = new Button { Left = 460, Top = 122, Width = 100, Text = "Refresh", BackColor = Color.FromArgb(100, 100, 100), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnRefresh.Click += (s, e) => { LoadPatients(); LoadBills(); };

            dgv = new DataGridView { Left = 12, Top = 160, Width = 710, Height = 340, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White, AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(240, 245, 250) }, SelectionMode = DataGridViewSelectionMode.FullRowSelect };

            Controls.Add(header);
            Controls.AddRange(new Control[] { lblPatient, cboPatient, lblAmount, nudAmount, lblDate, dtpDate, btnAdd, btnEdit, btnDelete, btnRefresh, dgv });
        }
    }
}
