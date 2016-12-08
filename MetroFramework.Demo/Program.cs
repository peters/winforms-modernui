using System;
using System.Windows.Forms;

namespace MetroFramework.Demo
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("de-DE");
            Application.CurrentCulture = cultureInfo;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
