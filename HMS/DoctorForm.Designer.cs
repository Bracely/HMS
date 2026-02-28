using System;
using System.Windows.Forms;
using System.Drawing;

namespace HMS
{
    partial class DoctorForm
    {
        private System.ComponentModel.IContainer components = null;

        private DataGridView dgv;
        private TextBox txtName, txtSpec;
        private Button btnAdd, btnRefresh, btnEdit, btnDelete;

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
            // Form styling to match Harare Institute of Technology
            this.Text = "Harare Institute of Technology - Doctor Management";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.WhiteSmoke;
            this.ClientSize = new Size(620, 480);

            // Header panel
            var header = new Panel { Left = 0, Top = 0, Width = this.ClientSize.Width, Height = 70, BackColor = Color.FromArgb(10, 60, 120) };
            var lblHeader = new Label { Left = 84, Top = 18, AutoSize = true, Text = "Harare Institute of Technology", ForeColor = Color.White, Font = new Font("Segoe UI", 14F, FontStyle.Bold) };
            var lblSub = new Label { Left = 84, Top = 38, AutoSize = true, Text = "Doctor Management", ForeColor = Color.WhiteSmoke, Font = new Font("Segoe UI", 9F, FontStyle.Regular) };

            // Logo placeholder (replace with actual logo in designer)
            var pic = new PictureBox { Left = 12, Top = 8, Width = 56, Height = 56, BackColor = Color.Gainsboro, BorderStyle = BorderStyle.FixedSingle, SizeMode = PictureBoxSizeMode.CenterImage };
            var logoImg = HMS.Resources.ResourceHelper.LoadLogo();
            if (logoImg != null)
            {
                pic.Image = logoImg;
                pic.SizeMode = PictureBoxSizeMode.StretchImage;
            }

            header.Controls.Add(pic);
            header.Controls.Add(lblHeader);
            header.Controls.Add(lblSub);

            // Data grid
            dgv = new DataGridView
            {
                Left = 10,
                Top = 100,
                Width = 600,
                Height = 320,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(240, 245, 250) }
            };

            var lblName = new Label { Left = 12, Top = 82, Text = "Name", Font = new Font("Segoe UI", 9F, FontStyle.Regular) };
            txtName = new TextBox { Left = 70, Top = 78, Width = 220 };

            var lblSpec = new Label { Left = 300, Top = 82, Text = "Specialization", Font = new Font("Segoe UI", 9F, FontStyle.Regular) };
            txtSpec = new TextBox { Left = 400, Top = 78, Width = 180 };

            btnAdd = new Button { Left = 70, Top = 430, Width = 100, Text = "Add", BackColor = Color.FromArgb(10, 60, 120), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button { Left = 190, Top = 430, Width = 100, Text = "Edit", BackColor = Color.FromArgb(30, 120, 200), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button { Left = 310, Top = 430, Width = 100, Text = "Delete", BackColor = Color.FromArgb(200, 40, 40), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnDelete.Click += BtnDelete_Click;

            btnRefresh = new Button { Left = 430, Top = 430, Width = 100, Text = "Refresh", BackColor = Color.FromArgb(100, 100, 100), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnRefresh.Click += (s, e) => LoadDoctors();

            // Add controls
            Controls.Add(header);
            Controls.AddRange(new Control[] { lblName, txtName, lblSpec, txtSpec, btnAdd, btnEdit, btnDelete, btnRefresh, dgv });
        }
    }
}
