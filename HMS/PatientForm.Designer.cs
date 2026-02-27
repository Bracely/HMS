using System;
using System.Windows.Forms;

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
            this.Text = "Patient Management";
            this.Width = 700;
            this.Height = 500;

            dgv = new DataGridView { Left = 10, Top = 160, Width = 660, Height = 280, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

            var lblFirst = new Label { Left = 10, Top = 10, Text = "First Name" };
            txtFirst = new TextBox { Left = 100, Top = 10, Width = 200 };

            var lblLast = new Label { Left = 10, Top = 40, Text = "Last Name" };
            txtLast = new TextBox { Left = 100, Top = 40, Width = 200 };

            var lblDob = new Label { Left = 320, Top = 10, Text = "Date of Birth" };
            dtpDob = new DateTimePicker { Left = 420, Top = 10, Width = 200 };

            var lblPhone = new Label { Left = 320, Top = 40, Text = "Phone" };
            txtPhone = new TextBox { Left = 420, Top = 40, Width = 200 };

            btnAdd = new Button { Left = 100, Top = 80, Width = 100, Text = "Add" };
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button { Left = 340, Top = 80, Width = 100, Text = "Edit" };
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button { Left = 460, Top = 80, Width = 100, Text = "Delete" };
            btnDelete.Click += BtnDelete_Click;

            btnRefresh = new Button { Left = 220, Top = 80, Width = 100, Text = "Refresh" };
            btnRefresh.Click += (s, e) => LoadPatients();

            txtSearch = new TextBox { Left = 10, Top = 120, Width = 200 };
            btnSearch = new Button { Left = 220, Top = 118, Width = 100, Text = "Search" };
            btnSearch.Click += (s, e) => SearchPatients();

            Controls.AddRange(new Control[] { lblFirst, txtFirst, lblLast, txtLast, lblDob, dtpDob, lblPhone, txtPhone, btnAdd, btnEdit, btnDelete, btnRefresh, txtSearch, btnSearch, dgv });
        }
    }
}
