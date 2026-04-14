using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using HMS.Services;

namespace HMS.Controls
{
    // ReportsControl - lightweight reporting panel that lists aggregated
    // diagnosis statistics. This control reads aggregate data from the
    // ClinicService and displays it in a simple list. It is suitable for
    // quick inspection and can be extended to export CSV or render
    // graphical charts.
    public class ReportsControl : UserControl
    {
        private ListBox lstStats;
        public ReportsControl()
        {
            Dock = DockStyle.Fill;
            Initialize();
            LoadStats();
        }

        private void Initialize()
        {
            lstStats = new ListBox { Dock = DockStyle.Fill };
            Controls.Add(lstStats);
        }

        private void LoadStats()
        {
            lstStats.Items.Clear();
            var stats = ClinicService.Instance.GetDiagnosisStatistics().OrderByDescending(kv => kv.Value);
            foreach(var kv in stats)
            {
                lstStats.Items.Add($"{kv.Key}: {kv.Value}");
            }
        }
    }
}
