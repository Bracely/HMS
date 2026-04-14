using System;
using System.Drawing;
using System.Windows.Forms;
using HMS.Services;

namespace HMS
{
    // LoginForm provides a lightweight UI for three demo roles: Student,
    // Doctor and Admin. The form is intentionally minimal: it demonstrates
    // how different types of users authenticate against the in-memory
    // ClinicService and AuthService. For real deployments replace this
    // with a secure login flow.
    public class LoginForm : Form
    {
        // Single unified input for all login types. The input is interpreted
        // at runtime: email => doctor, numeric => student, otherwise admin
        // (password). This keeps the UI simple while routing to the
        // appropriate AuthService method.
        private TextBox txtUnified;
        private Button btnSubmit;
        private Button btnCancel;

        public LoginForm()
        {
            Text = "Login - Harare Institute of Technology";
            Width = 480;
            Height = 160;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            var lbl = new Label { Left = 12, Top = 12, AutoSize = true, Text = "Enter student number, doctor email or admin password:" };
            txtUnified = new TextBox { Left = 12, Top = 36, Width = 420 }; // single input

            btnSubmit = new Button { Left = 12, Top = 72, Width = 100, Text = "Login" };
            btnSubmit.Click += BtnSubmit_Click;

            btnCancel = new Button { Left = 132, Top = 72, Width = 100, Text = "Cancel" };
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.AddRange(new Control[] { lbl, txtUnified, btnSubmit, btnCancel });
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            var input = (txtUnified.Text ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(input))
            {
                MessageBox.Show("Please enter credentials or identifier.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Detect email (doctor)
            if (IsEmail(input))
            {
                if (AuthService.LoginDoctor(input))
                {
                    DialogResult = DialogResult.OK;
                    Close();
                    return;
                }
                MessageBox.Show("Doctor not found. Check your email.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Detect numeric student id (per requirement). If numeric, treat
            // as student identifier; otherwise treat as admin password.
            if (IsNumeric(input))
            {
                if (AuthService.LoginStudent(input))
                {
                    DialogResult = DialogResult.OK;
                    Close();
                    return;
                }
                MessageBox.Show("Student not found. Check your student number.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Otherwise attempt admin login with provided input as password
            if (AuthService.LoginAdmin(input))
            {
                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            MessageBox.Show("Authentication failed. Check your credentials.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private static bool IsEmail(string value)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(value);
                return addr.Address == value;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsNumeric(string value)
        {
            // Treat as numeric if every char is a digit. This matches the
            // user's requirement (numeric => student number). Adjust if
            // your student IDs include letters.
            foreach (var c in value)
            {
                if (!char.IsDigit(c)) return false;
            }
            return value.Length > 0;
        }
    }
}
