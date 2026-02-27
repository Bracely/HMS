using System;
using System.Windows.Forms;

namespace HMS
{
    partial class AppointmentForm
    {
        private System.ComponentModel.IContainer components = null;

        private ComboBox cboPatient, cboDoctor;
        private DateTimePicker dtpDate;
        private TextBox txtReason;
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
            this.Text = "Appointments";
            this.Width = 800;
            this.Height = 520;

            var lblPatient = new Label { Left = 10, Top = 10, Text = "Patient" };
            cboPatient = new ComboBox { Left = 80, Top = 10, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };

            var lblDoctor = new Label { Left = 350, Top = 10, Text = "Doctor" };
            cboDoctor = new ComboBox { Left = 410, Top = 10, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };

            var lblDate = new Label { Left = 10, Top = 50, Text = "Date & Time" };
            dtpDate = new DateTimePicker { Left = 100, Top = 50, Width = 250, Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd HH:mm" };

            var lblReason = new Label { Left = 350, Top = 50, Text = "Reason" };
            txtReason = new TextBox { Left = 410, Top = 50, Width = 250 };

            btnAdd = new Button { Left = 680, Top = 10, Width = 80, Text = "Add" };
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button { Left = 680, Top = 90, Width = 80, Text = "Edit" };
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button { Left = 680, Top = 130, Width = 80, Text = "Delete" };
            btnDelete.Click += BtnDelete_Click;

            btnRefresh = new Button { Left = 680, Top = 50, Width = 80, Text = "Refresh" };
            btnRefresh.Click += (s, e) => { LoadLists(); LoadAppointments(); };

            dgv = new DataGridView { Left = 10, Top = 100, Width = 660, Height = 360, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

            Controls.AddRange(new Control[] { lblPatient, cboPatient, lblDoctor, cboDoctor, lblDate, dtpDate, lblReason, txtReason, btnAdd, btnRefresh, btnEdit, btnDelete, dgv });
        }
    }
}
