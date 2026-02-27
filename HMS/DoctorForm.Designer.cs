using System;
using System.Windows.Forms;

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
            this.Text = "Doctor Management";
            this.Width = 600;
            this.Height = 450;

            dgv = new DataGridView { Left = 10, Top = 100, Width = 560, Height = 300, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

            var lblName = new Label { Left = 10, Top = 10, Text = "Name" };
            txtName = new TextBox { Left = 80, Top = 10, Width = 200 };

            var lblSpec = new Label { Left = 10, Top = 40, Text = "Specialization" };
            txtSpec = new TextBox { Left = 110, Top = 40, Width = 170 };

            btnAdd = new Button { Left = 300, Top = 10, Width = 80, Text = "Add" };
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button { Left = 480, Top = 10, Width = 80, Text = "Edit" };
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button { Left = 480, Top = 40, Width = 80, Text = "Delete" };
            btnDelete.Click += BtnDelete_Click;

            btnRefresh = new Button { Left = 390, Top = 10, Width = 80, Text = "Refresh" };
            btnRefresh.Click += (s, e) => LoadDoctors();

            Controls.AddRange(new Control[] { lblName, txtName, lblSpec, txtSpec, btnAdd, btnEdit, btnDelete, btnRefresh, dgv });
        }
    }
}
