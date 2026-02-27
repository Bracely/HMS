using System;
using System.Windows.Forms;

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
            this.Text = "Billing";
            this.Width = 700;
            this.Height = 520;

            var lblPatient = new Label { Left = 10, Top = 10, Text = "Patient" };
            cboPatient = new ComboBox { Left = 80, Top = 10, Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };

            var lblAmount = new Label { Left = 10, Top = 40, Text = "Amount" };
            nudAmount = new NumericUpDown { Left = 80, Top = 40, Width = 120, DecimalPlaces = 2, Maximum = 1000000 };

            var lblDate = new Label { Left = 220, Top = 40, Text = "Date" };
            dtpDate = new DateTimePicker { Left = 260, Top = 40, Width = 150 };

            btnAdd = new Button { Left = 430, Top = 10, Width = 80, Text = "Add" };
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button { Left = 610, Top = 10, Width = 80, Text = "Edit" };
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button { Left = 610, Top = 50, Width = 80, Text = "Delete" };
            btnDelete.Click += BtnDelete_Click;

            btnRefresh = new Button { Left = 520, Top = 10, Width = 80, Text = "Refresh" };
            btnRefresh.Click += (s, e) => { LoadPatients(); LoadBills(); };

            dgv = new DataGridView { Left = 10, Top = 90, Width = 660, Height = 380, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

            Controls.AddRange(new Control[] { lblPatient, cboPatient, lblAmount, nudAmount, lblDate, dtpDate, btnAdd, btnEdit, btnDelete, btnRefresh, dgv });
        }
    }
}
