using System;
using System.Drawing;
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

        private void metroButton1_Click(object sender, EventArgs e)
        {
            if (Theme == MetroThemeStyle.Dark)
                StyleManager.Theme = MetroThemeStyle.Light;
            else
                StyleManager.Theme = MetroThemeStyle.Dark;
        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
            Random m = new Random();
            int next = m.Next(0, 13);
            StyleManager.Style = (MetroColorStyle)next;
        }

        private void metroTile1_Click(object sender, EventArgs e)
        {
            StyleManager.Style = MetroColorStyle.Red;
        }

        private void metroTile2_Click(object sender, EventArgs e)
        {
            StyleManager.Style = MetroColorStyle.Green;
        }

        private void metroLink1_Click(object sender, EventArgs e)
        {
            StyleManager.Style = MetroColorStyle.Lime;
        }

        private void metroLink3_Click(object sender, EventArgs e)
        {
            StyleManager.Style = MetroColorStyle.Silver;
        }

        private void metroTile3_Click(object sender, EventArgs e)
        {
            MetroTaskWindow.ShowTaskWindow(this, "metro task dialog", new MetroFramework.Controls.MetroTile(), 3);
        }

        private int steps = 0;
        private void metroButton4_Click(object sender, EventArgs e)
        {
            Color target = Color.Purple;

            if (steps == 1)
                target = Color.RoyalBlue;
            if (steps == 2)
            {
                target = Color.YellowGreen;
                steps = -1;
            }

            steps++;

            MetroFramework.Animation.ColorBlendAnimation myColorAnim = new MetroFramework.Animation.ColorBlendAnimation();
            myColorAnim.Start(panel1, "BackColor", target, 1);
            myColorAnim.AnimationCompleted += new EventHandler(myColorAnim_AnimationCompleted);

            metroButton4.Enabled = false;
        }

        void myColorAnim_AnimationCompleted(object sender, EventArgs e)
        {
            metroButton4.Enabled = true;
        }

        private void metroLink4_Click(object sender, EventArgs e)
        {
            MetroTaskWindow.CancelAutoClose();
        }

        private void metroTrackBar1_Scroll(object sender, ScrollEventArgs e)
        {
            metroLabel5.Text = metroTrackBar1.Value.ToString();
        }

        private void metroLabel4_Click(object sender, EventArgs e)
        {
            
        }

        private void metroLink2_Click(object sender, EventArgs e)
        {
            

        }

        private void metroTextBox1_TextChanged(object sender, EventArgs e)
        {
            metroLabel6.Text = metroTextBox1.Text;
        }
    }
}
