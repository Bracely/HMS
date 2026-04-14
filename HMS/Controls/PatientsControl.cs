using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using HMS.Services;
using HMS.Models;

namespace HMS.Controls
{
    // PatientsControl - displays a searchable list of patients and
    // provides basic refresh functionality. This control binds a read-
    // only DataGridView to the ClinicService in-memory list. In a full
    // implementation this control would include Add/Edit/Delete flows and
    // validation forms.
    public class PatientsControl : UserControl
    {
        private DataGridView dgv;
        private TextBox txtSearch;
        private Button btnRefresh;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;

        private ContextMenuStrip _rowMenu;

        public PatientsControl()
        {
            Dock = DockStyle.Fill;
            Initialize();
            LoadPatients();
        }

        private void Initialize()
        {
            // Top toolbar with search and actions
            var top = new Panel { Dock = DockStyle.Top, Height = 56, Padding = new Padding(8) };
            var toolbar = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, AutoSize = false };

            txtSearch = new TextBox { Width = 300, PlaceholderText = "Search by name, id or phone..." };
            txtSearch.TextChanged += (s, e) => SearchPatients();

            btnAdd = new Button { Text = "Add", AutoSize = true };
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button { Text = "Edit", AutoSize = true };
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button { Text = "Delete", AutoSize = true };
            btnDelete.Click += BtnDelete_Click;

            btnRefresh = new Button { Text = "Refresh", AutoSize = true };
            btnRefresh.Click += (s, e) => LoadPatients();

            toolbar.Controls.Add(txtSearch);
            toolbar.Controls.Add(btnAdd);
            toolbar.Controls.Add(btnEdit);
            toolbar.Controls.Add(btnDelete);
            toolbar.Controls.Add(btnRefresh);
            top.Controls.Add(toolbar);

            // Data grid
            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                MultiSelect = false
            };

            dgv.CellDoubleClick += Dgv_CellDoubleClick;

            // Context menu for row actions
            _rowMenu = new ContextMenuStrip();
            _rowMenu.Items.Add("Edit").Name = "edit";
            _rowMenu.Items.Add("Delete").Name = "delete";
            _rowMenu.ItemClicked += RowMenu_ItemClicked;
            dgv.ContextMenuStrip = _rowMenu;

            Controls.Add(dgv);
            Controls.Add(top);
        }

        //patient list components 

        private void LoadPatients()
        {
            var list = ClinicService.Instance.GetPatients().Select(p => new { p.Id, FullName = p.FirstName + " " + p.LastName, p.StudentId, p.PhoneNumber }).ToList();
            dgv.DataSource = list;
        }

        private void SearchPatients()
        {
            var q = txtSearch.Text;
            var list = ClinicService.Instance.SearchPatients(q).Select(p => new { p.Id, FullName = p.FirstName + " " + p.LastName, p.StudentId, p.PhoneNumber }).ToList();
            dgv.DataSource = list;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var f = new PatientForm())
            {
                var res = f.ShowDialog(this);
                if (res == DialogResult.OK || res == DialogResult.None)
                {
                    LoadPatients();
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) { MessageBox.Show("Select a patient to edit.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            var idObj = dgv.CurrentRow.Cells["Id"]?.Value ?? dgv.CurrentRow.Cells[0].Value;
            if (!int.TryParse(idObj?.ToString(), out var id)) { MessageBox.Show("Unable to determine selected patient ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            using (var f = new PatientForm(id)) { var res = f.ShowDialog(this); if (res == DialogResult.OK || res == DialogResult.None) LoadPatients(); }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) { MessageBox.Show("Select a patient to delete.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            var idObj = dgv.CurrentRow.Cells["Id"]?.Value ?? dgv.CurrentRow.Cells[0].Value;
            if (!int.TryParse(idObj?.ToString(), out var id)) { MessageBox.Show("Unable to determine selected patient ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            var confirm = MessageBox.Show("Are you sure you want to delete the selected patient?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;
            if (ClinicService.Instance.DeletePatient(id)) { MessageBox.Show("Patient deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information); LoadPatients(); }
            else MessageBox.Show("Failed to delete patient.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void Dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var idObj = dgv.Rows[e.RowIndex].Cells["Id"]?.Value ?? dgv.Rows[e.RowIndex].Cells[0].Value;
            if (!int.TryParse(idObj?.ToString(), out var id)) return;
            using (var f = new PatientForm(id)) { var res = f.ShowDialog(this); if (res == DialogResult.OK || res == DialogResult.None) LoadPatients(); }
        }

        private void RowMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            var idObj = dgv.CurrentRow.Cells["Id"]?.Value ?? dgv.CurrentRow.Cells[0].Value;
            if (!int.TryParse(idObj?.ToString(), out var id)) return;
            if (e.ClickedItem.Name == "edit") { using (var f = new PatientForm(id)) { var res = f.ShowDialog(this); if (res == DialogResult.OK || res == DialogResult.None) LoadPatients(); } }
            else if (e.ClickedItem.Name == "delete") { var confirm = MessageBox.Show("Delete this patient?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question); if (confirm == DialogResult.Yes) { if (ClinicService.Instance.DeletePatient(id)) LoadPatients(); } }
        }
    }
}
