using MetroFramework.Controls;
using MetroFramework.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using MetroFramework.Forms;
using MetroFramework.Localization;

namespace MetroFramework
{
    public partial class MetroMessageBoxControl : Form
    {
        private MetroLocalize metroLocalize = null;

        public MetroMessageBoxControl()
        {
            InitializeComponent();

            _properties = new MetroMessageBoxProperties(this);

            StylizeButton(metroButton1);
            StylizeButton(metroButton2);
            StylizeButton(metroButton3);

            metroButton1.Click += new EventHandler(button_Click);
            metroButton2.Click += new EventHandler(button_Click);
            metroButton3.Click += new EventHandler(button_Click);

            metroLocalize = new MetroLocalize(this);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Color _defaultColor = Color.FromArgb(57, 179, 215);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Color _errorColor = Color.FromArgb(210, 50, 45);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Color _warningColor = Color.FromArgb(237, 156, 40);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Color _success = Color.FromArgb(71, 164, 71);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Color _question = Color.FromArgb(71, 164, 71);

        /// <summary>
        /// Gets the top body section of the control. 
        /// </summary>
        public Panel Body
        {
            get { return panelbody; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private MetroMessageBoxProperties _properties = null;

        /// <summary>
        /// Gets the message box display properties.
        /// </summary>
        public MetroMessageBoxProperties Properties
        { get { return _properties; } }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private DialogResult _result = DialogResult.None;

        /// <summary>
        /// Gets the dialog result that the user have chosen.
        /// </summary>
        public DialogResult Result
        {
            get { return _result; }
        }

        /// <summary>
        /// Arranges the apperance of the message box overlay.
        /// </summary>
        public void ArrangeApperance()
        {
            titleLabel.Text = _properties.Title;
            messageLabel.Text = _properties.Message;

            switch (_properties.Icon)
            {
                case MessageBoxIcon.Exclamation:
                    panelbody.BackColor = _warningColor;
                    break;
                case MessageBoxIcon.Error:
                    panelbody.BackColor = _errorColor;
                    break;
                default: break;
            }

            switch (_properties.Buttons)
            {
                case MessageBoxButtons.OK:
                    EnableButton(metroButton1);

                    metroButton1.Text = metroLocalize.translate("Ok");
                    metroButton1.Location = metroButton3.Location;
                    metroButton1.Tag = DialogResult.OK;

                    EnableButton(metroButton2, false);
                    EnableButton(metroButton3, false);
                    break;
                case MessageBoxButtons.OKCancel:
                    EnableButton(metroButton1);

                    metroButton1.Text = metroLocalize.translate("Ok");
                    metroButton1.Location = metroButton2.Location;
                    metroButton1.Tag = DialogResult.OK;

                    EnableButton(metroButton2);

                    metroButton2.Text = metroLocalize.translate("Cancel");
                    metroButton2.Location = metroButton3.Location;
                    metroButton2.Tag = DialogResult.Cancel;

                    EnableButton(metroButton3, false);
                    break;
                case MessageBoxButtons.RetryCancel:
                    EnableButton(metroButton1);

                    metroButton1.Text = metroLocalize.translate("Retry");
                    metroButton1.Location = metroButton2.Location;
                    metroButton1.Tag = DialogResult.Retry;

                    EnableButton(metroButton2);

                    metroButton2.Text = metroLocalize.translate("Cancel");
                    metroButton2.Location = metroButton3.Location;
                    metroButton2.Tag = DialogResult.Cancel;

                    EnableButton(metroButton3, false);
                    break;
                case MessageBoxButtons.YesNo:
                    EnableButton(metroButton1);

                    metroButton1.Text = metroLocalize.translate("Yes");
                    metroButton1.Location = metroButton2.Location;
                    metroButton1.Tag = DialogResult.Yes;

                    EnableButton(metroButton2);

                    metroButton2.Text = metroLocalize.translate("No");
                    metroButton2.Location = metroButton3.Location;
                    metroButton2.Tag = DialogResult.No;

                    EnableButton(metroButton3, false);
                    break;
                case MessageBoxButtons.YesNoCancel:
                    EnableButton(metroButton1);

                    metroButton1.Text = metroLocalize.translate("Yes");
                    metroButton1.Tag = DialogResult.Yes;

                    EnableButton(metroButton2);

                    metroButton2.Text = metroLocalize.translate("No");
                    metroButton2.Tag = DialogResult.No;

                    EnableButton(metroButton3);

                    metroButton3.Text = metroLocalize.translate("Cancel");
                    metroButton3.Tag = DialogResult.Cancel;

                    break;
                case MessageBoxButtons.AbortRetryIgnore:
                    EnableButton(metroButton1);

                    metroButton1.Text = metroLocalize.translate("Abort");
                    metroButton1.Tag = DialogResult.Abort;

                    EnableButton(metroButton2);

                    metroButton2.Text = metroLocalize.translate("Retry");
                    metroButton2.Tag = DialogResult.Retry;

                    EnableButton(metroButton3);

                    metroButton3.Text = metroLocalize.translate("Ignore");
                    metroButton3.Tag = DialogResult.Ignore;

                    break;
                default : break;
            }

            switch (_properties.Icon)
            {
                case  MessageBoxIcon.Error:
                    panelbody.BackColor = _errorColor; break;
                case MessageBoxIcon.Warning:
                    panelbody.BackColor = _warningColor; break;
                case MessageBoxIcon.Information:
                    panelbody.BackColor = _defaultColor;                    
                     break;
                case MessageBoxIcon.Question:
                    panelbody.BackColor = _question; break;
                default:
                    panelbody.BackColor = Color.DarkGray; break;
            }
        }

        private void EnableButton(MetroButton button)
        { EnableButton(button, true); }

        private void EnableButton(MetroButton button, bool enabled)
        {
            button.Enabled = enabled; button.Visible = enabled;
        }

        /// <summary>
        /// Sets the default focused button.
        /// </summary>
        public void SetDefaultButton()
        {
            switch (_properties.DefaultButton)
            {
                case MessageBoxDefaultButton.Button1:
                    if (metroButton1 != null)
                    {
                        if (metroButton1.Enabled) metroButton1.Focus();
                    }
                    break;
                case MessageBoxDefaultButton.Button2:
                    if (metroButton2 != null)
                    {
                        if (metroButton2.Enabled) metroButton2.Focus();
                    }
                    break;
                case MessageBoxDefaultButton.Button3:
                    if (metroButton3 != null)
                    {
                        if (metroButton3.Enabled) metroButton3.Focus();
                    }
                    break;  
                default: break;
            }
        }

        private void button_MouseClick(object sender, MouseEventArgs e)
        {
            //MetroButton button = (MetroButton)sender;
            //button.BackColor = MetroPaint.BackColor.Button.Press(MetroFramework.MetroThemeStyle.Light);
            //button.FlatAppearance.BorderColor = MetroPaint.BorderColor.Button.Press(MetroFramework.MetroThemeStyle.Light);
            //button.ForeColor = MetroPaint.ForeColor.Button.Press(MetroFramework.MetroThemeStyle.Light);
        }

        private void button_MouseEnter(object sender, EventArgs e)
        { StylizeButton((MetroButton)sender, true); }

        private void button_MouseLeave(object sender, EventArgs e)
        { StylizeButton((MetroButton)sender); }

        private void StylizeButton(MetroButton button)
        { StylizeButton(button, false); }

        private void StylizeButton(MetroButton button, bool hovered)
        {
            button.Cursor = Cursors.Hand;

            button.MouseClick -= button_MouseClick;
            button.MouseClick += button_MouseClick;
            
            button.MouseEnter -= button_MouseEnter;
            button.MouseEnter += button_MouseEnter;

            button.MouseLeave -= button_MouseLeave;
            button.MouseLeave += button_MouseLeave;

            //if (hovered)
            //{
            //    button.FlatAppearance.BorderColor = MetroPaint.BorderColor.Button.Hover(MetroFramework.MetroThemeStyle.Light);
            //    button.ForeColor = MetroPaint.ForeColor.Button.Hover(MetroFramework.MetroThemeStyle.Light);
            //}
            //else
            //{
            //    button.BackColor = MetroPaint.BackColor.Button.Normal(MetroFramework.MetroThemeStyle.Light);
            //    button.FlatAppearance.BorderColor = Color.SlateGray;
            //    button.FlatAppearance.MouseOverBackColor = MetroPaint.BorderColor.Button.Hover(MetroFramework.MetroThemeStyle.Light);
            //    button.ForeColor = MetroPaint.ForeColor.Button.Normal(MetroFramework.MetroThemeStyle.Light);
            //    button.FlatAppearance.BorderSize = 1;
            //}
        }

        private void button_Click(object sender, EventArgs e)
        {
            MetroButton button = (MetroButton)sender;
            if (!button.Enabled) return;
            _result = (DialogResult)button.Tag;
            Hide(); 
        }

    }
}
