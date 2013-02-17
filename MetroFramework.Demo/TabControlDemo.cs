using MetroFramework.Controls;

namespace MetroFramework.Demo
{
    public partial class TabControlDemo : Forms.MetroForm
    {
        public TabControlDemo()
        {
            InitializeComponent();
            var top = tabPage1.Top;
            for (var i = 0; i < 30; i++)
            {
                var label = new MetroLabel
                {
                    Text = string.Format("Label {0}", i),
                    Top = top
                };
                tabPage1.Controls.Add(label);
                top += label.Height + 3;
            }
        }
    }
}
