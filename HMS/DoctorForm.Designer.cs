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
            // Form styling
            this.Text = "Harare Institute of Technology - Doctor Management";
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.BackColor = Color.WhiteSmoke;
            this.ClientSize = new Size(800, 520);

            // Header (responsive)
            var header = new Panel { Dock = DockStyle.Top, Height = 72, BackColor = Color.FromArgb(10, 60, 120) };
            var headerFlow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, Padding = new Padding(12) };
            pic = new PictureBox { Size = new Size(56, 56), Margin = new Padding(6), BackColor = Color.Gainsboro, BorderStyle = BorderStyle.FixedSingle, SizeMode = PictureBoxSizeMode.CenterImage };
            var headerTexts = new Panel { AutoSize = true };
            var lblHeader = new Label { AutoSize = true, Text = "Harare Institute of Technology", ForeColor = Color.White, Font = new Font("Segoe UI", 14F, FontStyle.Bold), Dock = DockStyle.Top };
            var lblSub = new Label { AutoSize = true, Text = "Doctor Management", ForeColor = Color.WhiteSmoke, Font = new Font("Segoe UI", 9F), Dock = DockStyle.Top };
            headerTexts.Controls.Add(lblSub); headerTexts.Controls.Add(lblHeader);
            headerFlow.Controls.Add(pic); headerFlow.Controls.Add(headerTexts);
            header.Controls.Add(headerFlow);

            // Inputs area
            var inputsFlow = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 72, Padding = new Padding(8), FlowDirection = FlowDirection.LeftToRight, WrapContents = true };
            var pName = new Panel { Width = 360, Height = 56, Margin = new Padding(6) };
            var lblName = new Label { Text = "Name", Dock = DockStyle.Top, AutoSize = true };
            txtName = new TextBox { Dock = DockStyle.Top, Width = 340 };
            pName.Controls.Add(txtName); pName.Controls.Add(lblName);

            var pSpec = new Panel { Width = 360, Height = 56, Margin = new Padding(6) };
            var lblSpec = new Label { Text = "Specialization", Dock = DockStyle.Top, AutoSize = true };
            txtSpec = new TextBox { Dock = DockStyle.Top, Width = 340 };
            pSpec.Controls.Add(txtSpec); pSpec.Controls.Add(lblSpec);

            inputsFlow.Controls.AddRange(new Control[] { pName, pSpec });

            // Main grid
            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(240, 245, 250) }
            };

            // Bottom toolbar
            var bottomToolbar = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 56, Padding = new Padding(8), FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
            btnAdd = new Button { Text = "Add", AutoSize = true, BackColor = Color.FromArgb(10, 60, 120), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Margin = new Padding(6) };
            btnAdd.Click += BtnAdd_Click;
            btnEdit = new Button { Text = "Edit", AutoSize = true, BackColor = Color.FromArgb(30, 120, 200), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Margin = new Padding(6) };
            btnEdit.Click += BtnEdit_Click;
            btnDelete = new Button { Text = "Delete", AutoSize = true, BackColor = Color.FromArgb(200, 40, 40), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Margin = new Padding(6) };
            btnDelete.Click += BtnDelete_Click;
            btnRefresh = new Button { Text = "Refresh", AutoSize = true, BackColor = Color.FromArgb(100, 100, 100), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Margin = new Padding(6) };
            btnRefresh.Click += (s, e) => LoadDoctors();
            bottomToolbar.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnRefresh });

            // Add controls
            Controls.Add(dgv);
            Controls.Add(bottomToolbar);
            Controls.Add(inputsFlow);
            Controls.Add(header);
        }
    }
}
