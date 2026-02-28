using System;
using System.Windows.Forms;
using System.Drawing;

namespace HMS
{
    partial class PatientForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private DataGridView dgv;
        private TextBox txtFirst, txtLast, txtPhone, txtSearch;
        private DateTimePicker dtpDob;
        private Button btnAdd, btnRefresh, btnSearch, btnEdit, btnDelete;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing"></param>
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
            this.ClientSize = new Size(720, 540);

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
            var lblSub = new Label { Left = 84, Top = 38, AutoSize = true, Text = "Patient Management", ForeColor = Color.WhiteSmoke, Font = new Font("Segoe UI", 9F) };
            header.Controls.Add(pic);
            header.Controls.Add(lblHeader);
            header.Controls.Add(lblSub);

            dgv = new DataGridView { Left = 10, Top = 120, Width = 690, Height = 360, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White, AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(240, 245, 250) }, SelectionMode = DataGridViewSelectionMode.FullRowSelect };

            var lblFirst = new Label { Left = 12, Top = 86, Text = "First Name", Font = new Font("Segoe UI", 9F) };
            txtFirst = new TextBox { Left = 90, Top = 82, Width = 180 };

            var lblLast = new Label { Left = 280, Top = 86, Text = "Last Name", Font = new Font("Segoe UI", 9F) };
            txtLast = new TextBox { Left = 350, Top = 82, Width = 180 };

            var lblDob = new Label { Left = 12, Top = 116, Text = "Date of Birth", Font = new Font("Segoe UI", 9F) };
            dtpDob = new DateTimePicker { Left = 100, Top = 112, Width = 160 };

            var lblPhone = new Label { Left = 280, Top = 116, Text = "Phone", Font = new Font("Segoe UI", 9F) };
            txtPhone = new TextBox { Left = 350, Top = 112, Width = 180 };

            btnAdd = new Button { Left = 90, Top = 480, Width = 100, Text = "Add", BackColor = Color.FromArgb(10, 60, 120), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button { Left = 200, Top = 480, Width = 100, Text = "Edit", BackColor = Color.FromArgb(30, 120, 200), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button { Left = 310, Top = 480, Width = 100, Text = "Delete", BackColor = Color.FromArgb(200, 40, 40), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnDelete.Click += BtnDelete_Click;

            btnRefresh = new Button { Left = 420, Top = 480, Width = 100, Text = "Refresh", BackColor = Color.FromArgb(100, 100, 100), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnRefresh.Click += (s, e) => LoadPatients();

            txtSearch = new TextBox { Left = 12, Top = 480 - 40, Width = 200 };
            btnSearch = new Button { Left = 220, Top = 480 - 40, Width = 100, Text = "Search", BackColor = Color.FromArgb(10, 60, 120), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSearch.Click += (s, e) => SearchPatients();

            Controls.Add(header);
            Controls.AddRange(new Control[] { lblFirst, txtFirst, lblLast, txtLast, lblDob, dtpDob, lblPhone, txtPhone, btnAdd, btnEdit, btnDelete, btnRefresh, txtSearch, btnSearch, dgv });
        }
    }
}
