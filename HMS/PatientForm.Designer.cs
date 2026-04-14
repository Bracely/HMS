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
        private TextBox txtFirst, txtLast, txtPhone, txtSearch, txtStudentId;
        private System.Windows.Forms.PictureBox pic;
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.BackColor = Color.WhiteSmoke;
            this.ClientSize = new Size(900, 600);

            // Header - uses FlowLayoutPanel so it adapts on resize
            var header = new Panel { Dock = DockStyle.Top, Height = 72, BackColor = Color.FromArgb(10, 60, 120) };
            var headerFlow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, Padding = new Padding(12) };
            pic = new PictureBox { Size = new Size(56, 56), Margin = new Padding(6), BackColor = Color.Gainsboro, BorderStyle = BorderStyle.FixedSingle };
            var headerTexts = new Panel { AutoSize = true };
            var lblHeader = new Label { AutoSize = true, Text = "Harare Institute of Technology", ForeColor = Color.White, Font = new Font("Segoe UI", 14F, FontStyle.Bold), Dock = DockStyle.Top };
            var lblSub = new Label { AutoSize = true, Text = "Patient Management", ForeColor = Color.WhiteSmoke, Font = new Font("Segoe UI", 9F), Dock = DockStyle.Top };
            headerTexts.Controls.Add(lblSub);
            headerTexts.Controls.Add(lblHeader);
            headerFlow.Controls.Add(pic);
            headerFlow.Controls.Add(headerTexts);
            header.Controls.Add(headerFlow);

            // Top toolbar with search + actions
            var topToolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48, Padding = new Padding(8), FlowDirection = FlowDirection.LeftToRight, WrapContents = false, AutoSize = false };
            txtSearch = new TextBox { Width = 300, Margin = new Padding(6) };
            txtSearch.TextChanged += (s, e) => SearchPatients();
            btnSearch = new Button { Text = "Search", AutoSize = true, Margin = new Padding(6) };
            btnSearch.Click += (s, e) => SearchPatients();
            btnRefresh = new Button { Text = "Refresh", AutoSize = true, Margin = new Padding(6) };
            btnRefresh.Click += (s, e) => LoadPatients();
            topToolbar.Controls.Add(txtSearch);
            topToolbar.Controls.Add(btnSearch);
            topToolbar.Controls.Add(btnRefresh);

            // Input area uses FlowLayoutPanel so fields wrap on resize
            var inputsFlow = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 120, Padding = new Padding(8), AutoSize = true, FlowDirection = FlowDirection.LeftToRight, WrapContents = true };

            var pFirst = new Panel { Width = 260, Height = 56, Margin = new Padding(6) };
            var lblFirst = new Label { Text = "First Name", Dock = DockStyle.Top, AutoSize = true };
            txtFirst = new TextBox { Dock = DockStyle.Top, Width = 240 };
            pFirst.Controls.Add(txtFirst); pFirst.Controls.Add(lblFirst);

            var pLast = new Panel { Width = 260, Height = 56, Margin = new Padding(6) };
            var lblLast = new Label { Text = "Last Name", Dock = DockStyle.Top, AutoSize = true };
            txtLast = new TextBox { Dock = DockStyle.Top, Width = 240 };
            pLast.Controls.Add(txtLast); pLast.Controls.Add(lblLast);

            var pStudent = new Panel { Width = 260, Height = 56, Margin = new Padding(6) };
            var lblStudent = new Label { Text = "Student ID", Dock = DockStyle.Top, AutoSize = true };
            txtStudentId = new TextBox { Dock = DockStyle.Top, Width = 240 };
            pStudent.Controls.Add(txtStudentId); pStudent.Controls.Add(lblStudent);

            var pDob = new Panel { Width = 260, Height = 56, Margin = new Padding(6) };
            var lblDob = new Label { Text = "Date of Birth", Dock = DockStyle.Top, AutoSize = true };
            dtpDob = new DateTimePicker { Dock = DockStyle.Top, Width = 240 };
            pDob.Controls.Add(dtpDob); pDob.Controls.Add(lblDob);

            var pPhone = new Panel { Width = 260, Height = 56, Margin = new Padding(6) };
            var lblPhone = new Label { Text = "Phone", Dock = DockStyle.Top, AutoSize = true };
            txtPhone = new TextBox { Dock = DockStyle.Top, Width = 240 };
            pPhone.Controls.Add(txtPhone); pPhone.Controls.Add(lblPhone);

            inputsFlow.Controls.AddRange(new Control[] { pFirst, pLast, pStudent, pDob, pPhone });

            // Main grid
            dgv = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White, AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(240, 245, 250) }, SelectionMode = DataGridViewSelectionMode.FullRowSelect };

            // Bottom toolbar with action buttons
            var bottomToolbar = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 56, Padding = new Padding(8), FlowDirection = FlowDirection.LeftToRight, WrapContents = false }; 
            btnAdd = new Button { Text = "Add", AutoSize = true, BackColor = Color.FromArgb(10, 60, 120), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Margin = new Padding(6) };
            btnAdd.Click += BtnAdd_Click;
            btnEdit = new Button { Text = "Edit", AutoSize = true, BackColor = Color.FromArgb(30, 120, 200), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Margin = new Padding(6) };
            btnEdit.Click += BtnEdit_Click;
            btnDelete = new Button { Text = "Delete", AutoSize = true, BackColor = Color.FromArgb(200, 40, 40), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Margin = new Padding(6) };
            btnDelete.Click += BtnDelete_Click;
            bottomToolbar.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete });

            // Add controls in order so docking works predictably
            Controls.Add(dgv);
            Controls.Add(bottomToolbar);
            Controls.Add(inputsFlow);
            Controls.Add(topToolbar);
            Controls.Add(header);
        }
    }
}
