using System;
using System.Linq;
using System.Windows.Forms;
using HMS.Models;
using HMS.Services;

namespace HMS
{
    public partial class BillingForm : Form
    {
        private int _editingBillId = 0;

        public BillingForm()
        {
            InitializeComponent();
            LoadPatients();
            LoadBills();
        }

        private void LoadPatients()
        {
            cboPatient.DataSource = ClinicService.Instance.GetPatients();
            cboPatient.DisplayMember = "FullName";
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var patient = cboPatient.SelectedItem as Patient;
                var bill = new Bill { Patient = patient, Amount = nudAmount.Value, Date = dtpDate.Value };

                if (_editingBillId == 0)
                {
                    ClinicService.Instance.AddBill(bill);
                    MessageBox.Show("Bill recorded.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    bill.BillId = _editingBillId;
                    ClinicService.Instance.UpdateBill(bill);
                    MessageBox.Show("Bill updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _editingBillId = 0;
                    btnAdd.Text = "Add";
                }

                ClearInputs();
                LoadBills();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ClearInputs()
        {
            nudAmount.Value = 0;
            dtpDate.Value = DateTime.Today;
            _editingBillId = 0;
            btnAdd.Text = "Add";
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Select a bill to edit.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var idObj = dgv.CurrentRow.Cells["BillId"]?.Value ?? dgv.CurrentRow.Cells[0].Value;
            if (!int.TryParse(idObj?.ToString(), out var id))
            {
                MessageBox.Show("Unable to determine selected bill ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var bill = ClinicService.Instance.GetBills().FirstOrDefault(b => b.BillId == id);
            if (bill == null) return;

            // Select patient
            for (int i = 0; i < cboPatient.Items.Count; i++)
            {
                if ((cboPatient.Items[i] as Patient)?.Id == bill.Patient.Id)
                {
                    cboPatient.SelectedIndex = i;
                    break;
                }
            }

            nudAmount.Value = bill.Amount;
            dtpDate.Value = bill.Date;
            _editingBillId = bill.BillId;
            btnAdd.Text = "Update";
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Select a bill to delete.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var idObj = dgv.CurrentRow.Cells["BillId"]?.Value ?? dgv.CurrentRow.Cells[0].Value;
            if (!int.TryParse(idObj?.ToString(), out var id))
            {
                MessageBox.Show("Unable to determine selected bill ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("Are you sure you want to delete the selected bill?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            if (ClinicService.Instance.DeleteBill(id))
            {
                MessageBox.Show("Bill deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadBills();
            }
            else
            {
                MessageBox.Show("Failed to delete bill.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void LoadBills()
        {
            var list = ClinicService.Instance.GetBills().Select(b => new { b.BillId, Patient = b.Patient.FullName, Amount = b.Amount.ToString("C"), Date = b.Date.ToShortDateString() }).ToList();
            dgv.DataSource = list;
        }
    }
}
