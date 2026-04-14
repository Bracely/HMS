namespace HMS
{
    public partial class Form1 : Form
    {
        private readonly ToolTip _toolTip = new ToolTip();
        private readonly System.Windows.Forms.Timer _sidebarTimer = new System.Windows.Forms.Timer();
        private readonly System.Windows.Forms.Timer _clockTimer = new System.Windows.Forms.Timer();
        private bool _sidebarExpanded = true;

        public Form1()
        {
            InitializeComponent();

            // Initialize tooltips
            _toolTip.SetToolTip(btnSidebarToggle, "Toggle sidebar");
            _toolTip.SetToolTip(btnDashboard, "Open admin dashboard");
            _toolTip.SetToolTip(btnPatients, "Manage patients");
            _toolTip.SetToolTip(btnDoctors, "Manage doctors");
            _toolTip.SetToolTip(btnAppointments, "Manage appointments");
            _toolTip.SetToolTip(btnBilling, "Manage billing");
            _toolTip.SetToolTip(btnReports, "Reports and statistics");
            _toolTip.SetToolTip(btnLogout, "Logout");

            // Sidebar animation timer
            _sidebarTimer.Interval = 15;
            _sidebarTimer.Tick += SidebarTimer_Tick;

            // Clock
            _clockTimer.Interval = 1000;
            _clockTimer.Tick += (s, e) => lblClock.Text = DateTime.Now.ToString("f");
            _clockTimer.Start();

            // Enable key preview for global shortcuts (e.g., fullscreen toggle)
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;

            // Show login when application starts
            Load += (s, e) => ShowLoginAndConfigure();

            // load logo immediately after initialization
            try { LoadLogoIfAvailable(); } catch { }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            // load logo after designer initialization
            LoadLogoIfAvailable();
        }

        // Load optional logo in runtime (not in designer)
        partial void LoadLogoIfAvailable();


        private void Form1_Load(object sender, EventArgs e)
        {

            // Launch in fullscreen. We set these at runtime so the designer
            // remains editable while the runtime experience is immersive.
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            // Default: load dashboard control
            LoadControl(new Controls.DashboardControl());
        }

        // Generic loader for UserControls into main panel
        public void LoadControl(UserControl control)
        {
            if (control == null) return;

            // Show loading overlay
            panelOverlay.Visible = true;
            panelOverlay.BringToFront();
            lblLoading.Left = (panelOverlay.Width - lblLoading.Width) / 2;
            lblLoading.Top = (panelOverlay.Height - lblLoading.Height) / 2;
            Application.DoEvents();

            panelMain.Controls.Clear();
            control.Dock = DockStyle.Fill;
            control.Visible = false;
            panelMain.Controls.Add(control);

            // simple fade-in via overlay
            control.Visible = true;
            panelOverlay.Visible = false;
            statusLabel.Text = $"Loaded {control.GetType().Name}";
        }

        private void SidebarButton_Click(object? sender, EventArgs e)
        {
            if (sender is Button b && b.Tag is string tag)
            {
                HighlightSidebarButton(b);
                switch (tag)
                {
                    case "dashboard": LoadControl(new Controls.DashboardControl()); break;
                    case "patients": LoadControl(new Controls.PatientsControl()); break;
                    case "doctors": LoadControl(new Controls.DoctorsControl()); break;
                    case "appointments": LoadControl(new Controls.AppointmentsControl()); break;
                    case "billing": LoadControl(new Controls.BillingControl()); break;
                    case "reports": LoadControl(new Controls.ReportsControl()); break;
                    case "logout": btnLogout_Click(this, EventArgs.Empty); break;
                }
            }
        }

        private void HighlightSidebarButton(Button active)
        {
            foreach (Control c in panelSidebar.Controls)
            {
                if (c is Button btn && btn.Tag is string)
                {
                    btn.BackColor = System.Drawing.Color.White;
                    btn.ForeColor = System.Drawing.Color.Black;
                }
            }
            active.BackColor = System.Drawing.Color.FromArgb(10, 60, 120);
            active.ForeColor = System.Drawing.Color.White;
        }

        private void BtnSidebarToggle_Click(object? sender, EventArgs e)
        {
            // If currently collapsed we need to make controls visible before
            // expanding so they animate into view. When collapsing we will
            // hide them at the end so only the toggle remains visible.
            if (!_sidebarExpanded)
            {
                foreach (Control c in panelSidebar.Controls)
                {
                    c.Visible = true;
                }
            }
            _sidebarTimer.Start();
        }

        private void SidebarTimer_Tick(object? sender, EventArgs e)
        {
            // Use a compact collapsed width so only the toggle remains.
            const int collapsedWidth = 48;
            const int expandedWidth = 220;
            const int step = 20;

            if (_sidebarExpanded)
            {
                // Collapse
                panelSidebar.Width = Math.Max(collapsedWidth, panelSidebar.Width - step);
                if (panelSidebar.Width <= collapsedWidth)
                {
                    panelSidebar.Width = collapsedWidth;
                    _sidebarTimer.Stop();
                    _sidebarExpanded = false;

                    // Hide all sidebar controls except the toggle so they
                    // disappear completely from sight.
                    foreach (Control c in panelSidebar.Controls)
                    {
                        if (c == btnSidebarToggle) continue;
                        c.Visible = false;
                    }
                }
            }
            else
            {
                // Expand
                panelSidebar.Width = Math.Min(expandedWidth, panelSidebar.Width + step);
                if (panelSidebar.Width >= expandedWidth)
                {
                    panelSidebar.Width = expandedWidth;
                    _sidebarTimer.Stop();
                    _sidebarExpanded = true;

                    // Ensure all sidebar controls are visible when expanded.
                    foreach (Control c in panelSidebar.Controls)
                    {
                        c.Visible = true;
                    }
                }
            }
        }

        private void BtnProfile_Click(object? sender, EventArgs e)
        {
            // simple profile dropdown: show message in status
            statusLabel.Text = "Profile clicked - Settings / Logout available.";
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            // F11 toggles fullscreen, Escape exits fullscreen
            if (e.KeyCode == Keys.F11)
            {
                if (this.FormBorderStyle == FormBorderStyle.None)
                {
                    this.FormBorderStyle = FormBorderStyle.Sizable;
                    this.WindowState = FormWindowState.Normal;
                }
                else
                {
                    this.FormBorderStyle = FormBorderStyle.None;
                    this.WindowState = FormWindowState.Maximized;
                }
            }
            else if (e.KeyCode == Keys.Escape && this.FormBorderStyle == FormBorderStyle.None)
            {
                // Restore windowed mode
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void UpdateUiForRole()
        {
            var role = HMS.Services.AuthService.CurrentRole;
            // Default disable all
            btnPatients.Enabled = false;
            btnDoctors.Enabled = false;
            btnAppointments.Enabled = false;
            btnBilling.Enabled = false;
            // no direct doctor portal button in new layout
            lblUserBadge.Text = string.Empty;
            btnLogout.Visible = false;

            if (role == HMS.Services.UserRole.Admin)
            {
                btnPatients.Enabled = btnDoctors.Enabled = btnAppointments.Enabled = btnBilling.Enabled = true;
            }
            else if (role == HMS.Services.UserRole.Doctor)
            {
                // Doctors can access doctor portal, appointments, and patient view
                btnDoctors.Enabled = true;
                btnAppointments.Enabled = true;
                btnPatients.Enabled = true;
                // doctor-specific UI handled via role checks
                btnLogout.Visible = true;
                var doc = HMS.Services.AuthService.CurrentDoctor;
                lblUserBadge.Text = doc != null ? $"Doctor: {doc.Name} ({doc.Email})" : "Doctor";
            }
            else if (role == HMS.Services.UserRole.Student)
            {
                // Students (patients) can view their patient data and book appointments
                btnPatients.Enabled = true;
                btnAppointments.Enabled = true;
                btnLogout.Visible = true;
                var pat = HMS.Services.AuthService.CurrentPatient;
                lblUserBadge.Text = pat != null ? $"Student: {pat.FullName} ({pat.StudentId})" : "Student";
            }
        }

        // Logout and show login again
        private void btnLogout_Click(object sender, EventArgs e)
        {
            HMS.Services.AuthService.Logout();
            // Reset UI and show login
            ShowLoginAndConfigure();
        }

        private void ShowLoginAndConfigure()
        {
            using (var f = new LoginForm())
            {
                var res = f.ShowDialog(this);
                if (res == DialogResult.OK)
                {
                    UpdateUiForRole();
                }
                else
                {
                    // if user cancels, close application
                    Close();
                }
            }
        }
        
        // Open Patient Management
        private void btnPatients_Click(object sender, EventArgs e)
        {
            using (var f = new PatientForm())
            {
                f.ShowDialog(this);
            }
        }

        // Open Doctor Management
        private void btnDoctors_Click(object sender, EventArgs e)
        {
            using (var f = new DoctorForm())
            {
                f.ShowDialog(this);
            }
        }

        // Open Appointment Booking
        private void btnAppointments_Click(object sender, EventArgs e)
        {
            using (var f = new AppointmentForm())
            {
                f.ShowDialog(this);
            }
        }

        // Open Billing
        private void btnBilling_Click(object sender, EventArgs e)
        {
            using (var f = new BillingForm())
            {
                f.ShowDialog(this);
            }
        }

        // Exit application
        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
