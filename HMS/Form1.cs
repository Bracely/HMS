namespace HMS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

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
