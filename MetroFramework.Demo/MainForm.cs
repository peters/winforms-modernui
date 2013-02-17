using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using MetroFramework.Forms;

namespace MetroFramework.Demo
{
    public partial class MainForm : MetroForm
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MetroButton1Click(object sender, EventArgs e)
        {
            StyleManager.Theme = Theme == MetroThemeStyle.Dark ? MetroThemeStyle.Light : MetroThemeStyle.Dark;
        }

        private void MetroButton3Click(object sender, EventArgs e)
        {
            var m = new Random();
            int next = m.Next(0, 13);
            StyleManager.Style = (MetroColorStyle)next;
        }

        private void MetroTile1Click(object sender, EventArgs e)
        {
            StyleManager.Style = MetroColorStyle.Red;
        }

        private void MetroTile2Click(object sender, EventArgs e)
        {
            StyleManager.Style = MetroColorStyle.Green;
        }

        private void MetroLink1Click(object sender, EventArgs e)
        {
            StyleManager.Style = MetroColorStyle.Lime;
        }

        private void MetroLink3Click(object sender, EventArgs e)
        {
            StyleManager.Style = MetroColorStyle.Silver;
        }

        private void MetroTile3Click(object sender, EventArgs e)
        {
            MetroTaskWindow.ShowTaskWindow(this, "metro task dialog", new Controls.MetroTile(), 3);
        }

        private int _steps;
        private void MetroButton4Click(object sender, EventArgs e)
        {
            Color target = Color.Purple;

            if (_steps == 1)
                target = Color.RoyalBlue;
            if (_steps == 2)
            {
                target = Color.YellowGreen;
                _steps = -1;
            }

            _steps++;

            var myColorAnim = new Animation.ColorBlendAnimation();
            myColorAnim.Start(panel1, "BackColor", target, 1);
            myColorAnim.AnimationCompleted += MyColorAnimAnimationCompleted;

            metroButton4.Enabled = false;
        }

        void MyColorAnimAnimationCompleted(object sender, EventArgs e)
        {
            metroButton4.Enabled = true;
        }

        private void MetroLink4Click(object sender, EventArgs e)
        {
            MetroTaskWindow.CancelAutoClose();
        }

        private void MetroTrackBar1Scroll(object sender, ScrollEventArgs e)
        {
            metroLabel5.Text = metroTrackBar1.Value.ToString(CultureInfo.InvariantCulture);
        }


        private void MetroTextBox1TextChanged(object sender, EventArgs e)
        {
            metroLabel6.Text = metroTextBox1.Text;
        }

        private void MainFormLoad(object sender, EventArgs e)
        {
            var animateProgressbar = new Timer();
            animateProgressbar.Tick += (o, args) =>
                {
                    metroProgressBar1.Value += 1;
                    if (metroProgressBar1.Value >= 100)
                    {
                        animateProgressbar.Dispose();
                    }
                };
            animateProgressbar.Interval = 200;
            animateProgressbar.Start();
        }

        private void MetroButton5Click(object sender, EventArgs e)
        {
            using (var tcd = new TabControlDemo())
            {
                tcd.ShowDialog(this);
            }
        }
    }
}
