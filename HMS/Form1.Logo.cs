using System;
using System.Windows.Forms;
using System.Drawing;

namespace HMS
{
    public partial class Form1 : Form
    {
        partial void LoadLogoIfAvailable()
        {
            try
            {
                var logo = HMS.Resources.ResourceHelper.LoadLogo();
                if (logo != null && this.pic != null)
                {
                    this.pic.Image = logo;
                    this.pic.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
            catch { /* ignore */ }
        }
    }
}
