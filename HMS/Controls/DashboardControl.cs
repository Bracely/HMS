using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HMS.Services;

namespace HMS.Controls
{
    // DashboardControl - a concise overview panel intended for admin and
    // reception staff. It shows high-level KPIs (cards) and a simple bar
    // visualization of common diagnoses. This control is intentionally
    // lightweight and draws a simple bar chart without external
    // dependencies so it can run reliably in the demo environment.
    public class DashboardControl : UserControl
    {
        private FlowLayoutPanel cardsPanel;
        private Panel chartPanel;

        public DashboardControl()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(246, 248, 251);
            InitializeComponents();
            LoadData();
        }

        private void InitializeComponents()
        {
            cardsPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 140, Padding = new Padding(12), AutoSize = false };
            cardsPanel.FlowDirection = FlowDirection.LeftToRight;
            cardsPanel.WrapContents = false;

            // create 4 cards
            cardsPanel.Controls.Add(CreateCard("Total Patients", "0", Color.FromArgb(52, 152, 219)));
            cardsPanel.Controls.Add(CreateCard("Today's Appointments", "0", Color.FromArgb(46, 204, 113)));
            cardsPanel.Controls.Add(CreateCard("Active Doctors", "0", Color.FromArgb(155, 89, 182)));
            cardsPanel.Controls.Add(CreateCard("Revenue Today", "$0", Color.FromArgb(241, 196, 15)));

            // simple chart panel (custom draw)
            chartPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            chartPanel.Paint += ChartPanel_Paint;

            Controls.Add(chartPanel);
            Controls.Add(cardsPanel);
        }

        private Panel CreateCard(string title, string value, Color accent)
        {
            var p = new Panel { Width = 220, Height = 100, Margin = new Padding(8), BackColor = Color.White };
            p.Paint += (s, e) => {
                // rounded corners
                var r = new System.Drawing.Drawing2D.GraphicsPath();
                r.AddArc(0, 0, 20, 20, 180, 90);
                r.AddArc(p.Width - 20, 0, 20, 20, 270, 90);
                r.AddArc(p.Width - 20, p.Height - 20, 20, 20, 0, 90);
                r.AddArc(0, p.Height - 20, 20, 20, 90, 90);
                r.CloseAllFigures();
                p.Region = new Region(r);
            };
            var lblTitle = new Label { Text = title, Left = 12, Top = 12, AutoSize = true, Font = new Font("Segoe UI", 9F) };
            var lblValue = new Label { Text = value, Left = 12, Top = 36, AutoSize = true, Font = new Font("Segoe UI", 16F, FontStyle.Bold) };
            var icon = new Label { Text = "●", ForeColor = accent, Left = p.Width - 36, Top = 12, AutoSize = true, Font = new Font("Segoe UI", 18F) };
            p.Controls.AddRange(new Control[] { lblTitle, lblValue, icon });
            p.MouseEnter += (s, e) => p.BackColor = Color.FromArgb(250, 250, 250);
            p.MouseLeave += (s, e) => p.BackColor = Color.White;
            // store value label in Tag for update
            p.Tag = lblValue;
            return p;
        }

        private void LoadData()
        {
            var patients = ClinicService.Instance.GetPatients();
            var doctors = ClinicService.Instance.GetDoctors();
            var appts = ClinicService.Instance.GetAppointments().Where(a => a.Date.Date == DateTime.Today).ToList();

            // update cards
            ((Label)((Panel)cardsPanel.Controls[0]).Tag).Text = patients.Count.ToString();
            ((Label)((Panel)cardsPanel.Controls[1]).Tag).Text = appts.Count.ToString();
            ((Label)((Panel)cardsPanel.Controls[2]).Tag).Text = doctors.Count.ToString();
            // revenue today approximate from bills
            var todayRevenue = ClinicService.Instance.GetBills().Where(b => b.Date.Date == DateTime.Today).Sum(b => b.Amount);
            ((Label)((Panel)cardsPanel.Controls[3]).Tag).Text = "$" + todayRevenue.ToString("0.00");

            // chart data: top diagnoses
            var stats = ClinicService.Instance.GetDiagnosisStatistics().OrderByDescending(kv => kv.Value).Take(10).ToList();
            chartPanel.Tag = stats; // store for paint
            chartPanel.Invalidate();
        }

        private void ChartPanel_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.White);
            var stats = (chartPanel.Tag as System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string,int>>);
            if (stats == null || stats.Count == 0) return;
            int padding = 20;
            int w = chartPanel.ClientSize.Width - padding * 2;
            int h = chartPanel.ClientSize.Height - padding * 2;
            int barWidth = Math.Max(20, w / stats.Count - 10);
            int max = stats.Max(s => s.Value);
            for (int i = 0; i < stats.Count; i++)
            {
                var kv = stats[i];
                int barHeight = max == 0 ? 0 : (int)((kv.Value / (double)max) * (h - 40));
                int x = padding + i * (barWidth + 10);
                int y = padding + (h - barHeight);
                var rect = new Rectangle(x, y, barWidth, barHeight);
                g.FillRectangle(Brushes.SteelBlue, rect);
                g.DrawString(kv.Key, new Font("Segoe UI", 8F), Brushes.Black, x, padding + h + 2);
            }
        }
    }
}
