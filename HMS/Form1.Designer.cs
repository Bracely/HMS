namespace HMS
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            // Configure the main window appearance and behavior.
            // We keep a fixed single border to maintain layout consistency and
            // prevent unintended resizing that could break the dashboard layout.
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(1000, 700);

            // Header
            // The top header contains the application brand, a realtime clock,
            // and a compact user/profile area. It is docked to the top and uses
            // the clinic's primary color (dark blue) for a professional look.
            panelHeader = new Panel { Dock = DockStyle.Top, Height = 72, BackColor = System.Drawing.Color.FromArgb(10, 60, 120) };
            // Logo placeholder: the runtime will load the logo image if available
            // so the designer remains free of file I/O. The PictureBox is created
            // here and populated at runtime from ResourceHelper.
            pic = new PictureBox { Left = 12, Top = 12, Width = 48, Height = 48, BackColor = System.Drawing.Color.Gainsboro, BorderStyle = BorderStyle.FixedSingle };
            pic.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            // Application title shown in the header. Uses Segoe UI and a larger
            // font weight to convey importance and branding consistency.
            lblTitleMain = new Label { Left = 72, Top = 18, AutoSize = true, Text = "Harare Institute of Technology - Clinic", ForeColor = System.Drawing.Color.White, Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold) };
            lblTitleMain.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            // Realtime clock label. Updated from Form1 at runtime to show the
            // current date/time in a human-friendly format.
            lblClock = new Label { Left = 400, Top = 22, AutoSize = true, ForeColor = System.Drawing.Color.White, Font = new System.Drawing.Font("Segoe UI", 9F) };
            lblClock.Anchor = AnchorStyles.Top;
            // Compact user badge. Displays current user role and identity (doctor
            // name or student id) after successful login. Anchored to the right
            // so it remains visible when the form is resized horizontally.
            lblUserBadge = new Label { Anchor = AnchorStyles.Top | AnchorStyles.Right, Left = 760, Top = 22, AutoSize = true, ForeColor = System.Drawing.Color.White, Font = new System.Drawing.Font("Segoe UI", 9F) };
            // Profile button placeholder. In the full implementation this will
            // open a user menu (Settings, Logout) – for now it updates the status
            // label when clicked.
            btnProfile = new Button { Anchor = AnchorStyles.Top | AnchorStyles.Right, Left = 900, Top = 18, Width = 80, Height = 28, Text = "Profile", BackColor = System.Drawing.Color.FromArgb(30, 120, 200), ForeColor = System.Drawing.Color.White, FlatStyle = FlatStyle.Flat };
            panelHeader.Controls.Add(pic);
            panelHeader.Controls.Add(lblTitleMain);
            panelHeader.Controls.Add(lblClock);
            panelHeader.Controls.Add(lblUserBadge);
            panelHeader.Controls.Add(btnProfile);

            // Sidebar
            // The left navigation column provides role-based access to the
            // application modules. It is collapsible and contains clearly
            // labeled buttons for each section.
            panelSidebar = new Panel { Dock = DockStyle.Left, Width = 220, BackColor = System.Drawing.Color.White, Padding = new Padding(6) };
            // Sidebar toggle (hamburger button). Clicking this button animates
            // the sidebar between expanded and collapsed states for a modern
            // responsive feel without changing application flow.
            btnSidebarToggle = new Button { Left = 6, Top = 6, Width = 36, Height = 36, Text = "≡", FlatStyle = FlatStyle.Flat, BackColor = System.Drawing.Color.FromArgb(10, 60, 120), ForeColor = System.Drawing.Color.White };
            // Primary navigation buttons. Each carries a "Tag" string used by
            // the click handler to determine which UserControl to load into
            // the main content panel (panelMain).
            btnDashboard = new Button { Left = 6, Top = 56, Width = 200, Height = 40, Text = "Dashboard", Tag = "dashboard", FlatStyle = FlatStyle.Flat };
            btnPatients = new Button { Left = 6, Top = 106, Width = 200, Height = 40, Text = "Patients", Tag = "patients", FlatStyle = FlatStyle.Flat };
            btnDoctors = new Button { Left = 6, Top = 156, Width = 200, Height = 40, Text = "Doctors", Tag = "doctors", FlatStyle = FlatStyle.Flat };
            btnAppointments = new Button { Left = 6, Top = 206, Width = 200, Height = 40, Text = "Appointments", Tag = "appointments", FlatStyle = FlatStyle.Flat };
            btnBilling = new Button { Left = 6, Top = 256, Width = 200, Height = 40, Text = "Billing", Tag = "billing", FlatStyle = FlatStyle.Flat };
            btnReports = new Button { Left = 6, Top = 306, Width = 200, Height = 40, Text = "Reports", Tag = "reports", FlatStyle = FlatStyle.Flat };
            // Logout button: destructive red style communicates a state change.
            btnLogout = new Button { Left = 6, Top = 360, Width = 200, Height = 40, Text = "Logout", Tag = "logout", FlatStyle = FlatStyle.Flat, BackColor = System.Drawing.Color.FromArgb(200, 40, 40), ForeColor = System.Drawing.Color.White };
            panelSidebar.Controls.AddRange(new Control[] { btnSidebarToggle, btnDashboard, btnPatients, btnDoctors, btnAppointments, btnBilling, btnReports, btnLogout });

            // Main content panel
            // This is the dynamic host for all UserControls. Use LoadControl
            // to populate this panel and ensure consistent docking and layout.
            panelMain = new Panel { Dock = DockStyle.Fill, BackColor = System.Drawing.Color.FromArgb(246, 248, 251) };

            // Overlay and loading indicator
            // The overlay covers the main content during load operations to
            // prevent interaction and present a clear loading state. The
            // indicator text is centered at runtime by the form logic.
            panelOverlay = new Panel { Dock = DockStyle.Fill, Visible = false, BackColor = System.Drawing.Color.FromArgb(200, 255, 255, 255) };
            lblLoading = new Label { AutoSize = true, Text = "Loading...", Font = new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold), ForeColor = System.Drawing.Color.FromArgb(10,60,120) };
            panelOverlay.Controls.Add(lblLoading);
            panelOverlay.Visible = false;

            // Status label
            statusLabel = new Label { Dock = DockStyle.Bottom, Height = 24, Text = "Ready", TextAlign = System.Drawing.ContentAlignment.MiddleLeft, Padding = new Padding(8), BackColor = System.Drawing.Color.WhiteSmoke };

            // Add controls to form in z-order (main first, overlays and chrome
            // last) so the overlay and header render above the content area.
            Controls.Add(panelMain);
            Controls.Add(panelOverlay);
            Controls.Add(panelSidebar);
            Controls.Add(panelHeader);
            Controls.Add(statusLabel);

            // Wire basic events (actual handlers are implemented in the
            // code-behind file Form1.cs). The sidebar buttons all route to
            // the same click handler which uses the Tag property to load
            // the relevant UserControl into panelMain.
            btnSidebarToggle.Click += BtnSidebarToggle_Click;
            btnDashboard.Click += SidebarButton_Click;
            btnPatients.Click += SidebarButton_Click;
            btnDoctors.Click += SidebarButton_Click;
            btnAppointments.Click += SidebarButton_Click;
            btnBilling.Click += SidebarButton_Click;
            btnReports.Click += SidebarButton_Click;
            btnLogout.Click += SidebarButton_Click;
            btnProfile.Click += BtnProfile_Click;

            this.Name = "Form1";
            this.Text = "HIT Clinic Dashboard";
            this.Load += new System.EventHandler(this.Form1_Load);
        }

        #endregion

        // Controls
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Panel panelSidebar;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelOverlay;
        private System.Windows.Forms.Label lblLoading;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Button btnSidebarToggle;
        private System.Windows.Forms.Button btnDashboard;
        private System.Windows.Forms.Button btnPatients;
        private System.Windows.Forms.Button btnDoctors;
        private System.Windows.Forms.Button btnAppointments;
        private System.Windows.Forms.Button btnBilling;
        private System.Windows.Forms.Button btnReports;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Label lblTitleMain;
        private System.Windows.Forms.Label lblClock;
        private System.Windows.Forms.Label lblUserBadge;
        private System.Windows.Forms.Button btnProfile;
        private System.Windows.Forms.PictureBox pic;
    }
}

