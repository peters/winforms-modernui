namespace MetroFramework.Demo
{
    partial class TabControlDemo
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.metroTabControl1 = new MetroFramework.Controls.MetroTabControl();
            this.tabPage1 = new MetroFramework.Controls.TabPage();
            this.metroStyleManager1 = new MetroFramework.Components.MetroStyleManager();
            this.metroTabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // metroTabControl1
            // 
            this.metroTabControl1.Controls.Add(this.tabPage1);
            this.metroTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metroTabControl1.FontSize = MetroFramework.MetroLabelSize.Medium;
            this.metroTabControl1.FontWeight = MetroFramework.MetroLabelWeight.Light;
            this.metroTabControl1.Location = new System.Drawing.Point(20, 60);
            this.metroTabControl1.Name = "metroTabControl1";
            this.metroTabControl1.SelectedIndex = 0;
            this.metroTabControl1.Size = new System.Drawing.Size(663, 165);
            this.metroTabControl1.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroTabControl1.StyleManager = this.metroStyleManager1;
            this.metroTabControl1.TabIndex = 37;
            this.metroTabControl1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.metroTabControl1.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroTabControl1.UseStyleColors = false;
            // 
            // tabPage1
            // 
            this.tabPage1.AutoScroll = true;
            this.tabPage1.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.tabPage1.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.tabPage1.HorizontalScrollBarShow = false;
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(0);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(655, 136);
            this.tabPage1.Style = MetroFramework.MetroColorStyle.Blue;
            this.tabPage1.StyleManager = this.metroStyleManager1;
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Scrollable tabcontrol";
            this.tabPage1.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.tabPage1.VerticalScrollBarShow = true;
            // 
            // metroStyleManager1
            // 
            this.metroStyleManager1.OwnerForm = this;
            this.metroStyleManager1.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroStyleManager1.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // TabControlDemo
            // 
            this.ClientSize = new System.Drawing.Size(703, 245);
            this.Controls.Add(this.metroTabControl1);
            this.Name = "TabControlDemo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.StyleManager = this.metroStyleManager1;
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroTabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.MetroTabControl metroTabControl1;
        private Controls.TabPage tabPage1;
        private Components.MetroStyleManager metroStyleManager1;

    }
}
