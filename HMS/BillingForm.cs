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
            try { var logo = HMS.Resources.ResourceHelper.LoadLogo(); if (logo != null && this.pic != null) { this.pic.Image = logo; this.pic.SizeMode = PictureBoxSizeMode.StretchImage; } } catch { }
            // Make popup responsive to parent size changes
            EnableResponsivePopup();
            LoadPatients();
            LoadBills();
        }

        private void EnableResponsivePopup()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.Shown += (s, e) => AttachToOwner();
            this.FormClosed += (s, e) => DetachFromOwner();
        }

        private void AttachToOwner()
        {
            if (this.Owner != null)
            {
                this.Owner.SizeChanged += Owner_SizeChanged;
                AdjustToOwner();
            }
            else { var wa = Screen.PrimaryScreen.WorkingArea; this.Size = new System.Drawing.Size((int)(wa.Width * 0.8), (int)(wa.Height * 0.8)); this.CenterToScreen(); }
        }

        private void DetachFromOwner()
        {
            if (this.Owner != null) this.Owner.SizeChanged -= Owner_SizeChanged;
        }

        private void Owner_SizeChanged(object? sender, EventArgs e) => AdjustToOwner();

        private void AdjustToOwner()
        {
            if (this.Owner == null) return;
            try
            {
                var o = this.Owner;
                var w = Math.Max(600, (int)(o.ClientSize.Width * 0.8));
                var h = Math.Max(400, (int)(o.ClientSize.Height * 0.8));
                this.Size = new System.Drawing.Size(w, h);
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new System.Drawing.Point(o.Location.X + (o.Width - this.Width) / 2, o.Location.Y + (o.Height - this.Height) / 2);
            }
            catch { }
        }

        private void LoadPatients()
        {
            var role = HMS.Services.AuthService.CurrentRole;
            if (role == HMS.Services.UserRole.Student)
            {
                var p = HMS.Services.AuthService.CurrentPatient;
                cboPatient.DataSource = new System.Collections.Generic.List<Patient> { p };
                cboPatient.DisplayMember = "FullName";
                cboPatient.Enabled = false;
                // students should not be able to add or delete bills
                btnAdd.Enabled = false;
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
            }
            else
            {
                cboPatient.DataSource = ClinicService.Instance.GetPatients();
                cboPatient.DisplayMember = "FullName";
                cboPatient.Enabled = true;
                btnAdd.Enabled = true;
                btnEdit.Enabled = true;
                btnDelete.Enabled = true;
            }
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
