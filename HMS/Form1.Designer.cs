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

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(820, 480);

            // Header
            var header = new Panel { Left = 0, Top = 0, Width = this.ClientSize.Width, Height = 70, BackColor = System.Drawing.Color.FromArgb(10, 60, 120) };
            var pic = new PictureBox { Left = 12, Top = 8, Width = 56, Height = 56, BackColor = System.Drawing.Color.Gainsboro, BorderStyle = BorderStyle.FixedSingle };
            var logoImg = HMS.Resources.ResourceHelper.LoadLogo();
            if (logoImg != null)
            {
                pic.Image = logoImg;
                pic.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            var lblTitleMain = new Label { Left = 80, Top = 18, AutoSize = true, Text = "Harare Institute of Technology", ForeColor = System.Drawing.Color.White, Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold) };
            var lblSubtitle = new Label { Left = 80, Top = 38, AutoSize = true, Text = "Clinic Management Dashboard", ForeColor = System.Drawing.Color.WhiteSmoke, Font = new System.Drawing.Font("Segoe UI", 9F) };
            header.Controls.Add(pic);
            header.Controls.Add(lblTitleMain);
            header.Controls.Add(lblSubtitle);

            // Buttons
            btnPatients = new System.Windows.Forms.Button { Left = 30, Top = 100, Width = 200, Height = 50, Text = "Patient Management", BackColor = System.Drawing.Color.FromArgb(10, 60, 120), ForeColor = System.Drawing.Color.White, FlatStyle = FlatStyle.Flat };
            btnPatients.Click += new System.EventHandler(this.btnPatients_Click);

            btnDoctors = new System.Windows.Forms.Button { Left = 250, Top = 100, Width = 200, Height = 50, Text = "Doctor Management", BackColor = System.Drawing.Color.FromArgb(10, 60, 120), ForeColor = System.Drawing.Color.White, FlatStyle = FlatStyle.Flat };
            btnDoctors.Click += new System.EventHandler(this.btnDoctors_Click);

            btnAppointments = new System.Windows.Forms.Button { Left = 470, Top = 100, Width = 200, Height = 50, Text = "Book Appointment", BackColor = System.Drawing.Color.FromArgb(10, 60, 120), ForeColor = System.Drawing.Color.White, FlatStyle = FlatStyle.Flat };
            btnAppointments.Click += new System.EventHandler(this.btnAppointments_Click);

            btnBilling = new System.Windows.Forms.Button { Left = 30, Top = 170, Width = 200, Height = 50, Text = "Billing", BackColor = System.Drawing.Color.FromArgb(30, 120, 200), ForeColor = System.Drawing.Color.White, FlatStyle = FlatStyle.Flat };
            btnBilling.Click += new System.EventHandler(this.btnBilling_Click);

            btnExit = new System.Windows.Forms.Button { Left = 250, Top = 170, Width = 200, Height = 50, Text = "Exit", BackColor = System.Drawing.Color.FromArgb(200, 40, 40), ForeColor = System.Drawing.Color.White, FlatStyle = FlatStyle.Flat };
            btnExit.Click += new System.EventHandler(this.btnExit_Click);

            // Add to form
            Controls.Add(header);
            Controls.Add(btnPatients);
            Controls.Add(btnDoctors);
            Controls.Add(btnAppointments);
            Controls.Add(btnBilling);
            Controls.Add(btnExit);

            this.Name = "Form1";
            this.Text = "HIT Clinic Dashboard";
            this.Load += new System.EventHandler(this.Form1_Load);
        }

        #endregion

        // Controls
        private System.Windows.Forms.Button btnPatients;
        private System.Windows.Forms.Button btnDoctors;
        private System.Windows.Forms.Button btnAppointments;
        private System.Windows.Forms.Button btnBilling;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label labelTitle;
    }
}

