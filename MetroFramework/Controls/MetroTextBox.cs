using System;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using MetroFramework.Components;
using MetroFramework.Design;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;
using MetroFramework.Native;

namespace MetroFramework.Controls
{
    [Designer(typeof(MetroTextBoxDesigner))]
    public class MetroTextBox : Control, IMetroControl
    {
        #region Interface

        private MetroColorStyle metroStyle = MetroColorStyle.Blue;
        [Category("Metro Appearance")]
        public MetroColorStyle Style
        {
            get
            {
                if (StyleManager != null)
                    return StyleManager.Style;

                return metroStyle;
            }
            set { metroStyle = value; }
        }

        private MetroThemeStyle metroTheme = MetroThemeStyle.Light;
        [Category("Metro Appearance")]
        public MetroThemeStyle Theme
        {
            get
            {
                if (StyleManager != null)
                    return StyleManager.Theme;

                return metroTheme;
            }
            set { metroTheme = value; }
        }

        private MetroStyleManager metroStyleManager = null;
        [Browsable(false)]
        public MetroStyleManager StyleManager
        {
            get { return metroStyleManager; }
            set { metroStyleManager = value; }
        }

        #endregion

        #region Fields

        private TextBox baseTextBox;

        private bool useStyleColors = false;
        [Category("Metro Appearance")]
        public bool UseStyleColors
        {
            get { return useStyleColors; }
            set { useStyleColors = value; UpdateBaseTextBox(); }
        }

        private MetroTextBoxSize metroTextBoxSize = MetroTextBoxSize.Small;
        [Category("Metro Appearance")]
        public MetroTextBoxSize FontSize
        {
            get { return metroTextBoxSize; }
            set { metroTextBoxSize = value; UpdateBaseTextBox(); }
        }

        private MetroTextBoxWeight metroTextBoxWeight = MetroTextBoxWeight.Regular;
        [Category("Metro Appearance")]
        public MetroTextBoxWeight FontWeight
        {
            get { return metroTextBoxWeight; }
            set { metroTextBoxWeight = value; UpdateBaseTextBox(); }
        }

        #endregion

        #region Routing Fields

        public bool Multiline
        {
            get { return baseTextBox.Multiline; }
            set { baseTextBox.Multiline = value; }
        }

        public override string Text
        {
            get { return baseTextBox.Text; }
            set { baseTextBox.Text = value; }
        }

        #endregion

        #region Constructor

        public MetroTextBox()
        {
            DoubleBuffered = true;

            CreateBaseTextBox();
            UpdateBaseTextBox();
            AddEventHandler();       
        }

        private void CreateBaseTextBox()
        {
            baseTextBox = new TextBox();
            baseTextBox.BorderStyle = BorderStyle.None;
            baseTextBox.Font = MetroFonts.Default(12f);
            baseTextBox.Location = new Point(3, 3);

            Size = new Size(baseTextBox.Width + 6, baseTextBox.Height + 6);
            Controls.Add(baseTextBox);
        }

        private void AddEventHandler()
        {
            baseTextBox.AcceptsTabChanged += new EventHandler(baseTextBox_AcceptsTabChanged);

            baseTextBox.CausesValidationChanged += new EventHandler(baseTextBox_CausesValidationChanged);
            baseTextBox.ChangeUICues += new UICuesEventHandler(baseTextBox_ChangeUICues);
            baseTextBox.Click += new EventHandler(baseTextBox_Click);
            baseTextBox.ClientSizeChanged += new EventHandler(baseTextBox_ClientSizeChanged);
            baseTextBox.ContextMenuChanged += new EventHandler(baseTextBox_ContextMenuChanged);
            baseTextBox.ContextMenuStripChanged += new EventHandler(baseTextBox_ContextMenuStripChanged);
            baseTextBox.CursorChanged += new EventHandler(baseTextBox_CursorChanged);

            baseTextBox.EnabledChanged += new EventHandler(baseTextBox_EnabledChanged);

            baseTextBox.KeyDown += new KeyEventHandler(baseTextBox_KeyDown);
            baseTextBox.KeyPress += new KeyPressEventHandler(baseTextBox_KeyPress);
            baseTextBox.KeyUp += new KeyEventHandler(baseTextBox_KeyUp);

            baseTextBox.SizeChanged += new EventHandler(baseTextBox_SizeChanged);

            baseTextBox.TextChanged += new EventHandler(baseTextBox_TextChanged);
        }

        private void RemoveEventHandler()
        {

        }

        #endregion

        #region Routing Methods

        public event EventHandler AcceptsTabChanged;
        private void baseTextBox_AcceptsTabChanged(object sender, EventArgs e)
        {
            if (AcceptsTabChanged != null)
                AcceptsTabChanged(this, e);
        }

        private void baseTextBox_EnabledChanged(object sender, EventArgs e)
        {
            base.OnEnabledChanged(e);
        }
        
        private void baseTextBox_SizeChanged(object sender, EventArgs e)
        {
            base.OnSizeChanged(e);
        }

        private void baseTextBox_CursorChanged(object sender, EventArgs e)
        {
            base.OnCursorChanged(e);
        }

        private void baseTextBox_ContextMenuStripChanged(object sender, EventArgs e)
        {
            base.OnContextMenuStripChanged(e);
        }

        private void baseTextBox_ContextMenuChanged(object sender, EventArgs e)
        {
            base.OnContextMenuChanged(e);
        }

        private void baseTextBox_ClientSizeChanged(object sender, EventArgs e)
        {
            base.OnClientSizeChanged(e);
        }

        private void baseTextBox_Click(object sender, EventArgs e)
        {
            base.OnClick(e);
        }

        private void baseTextBox_ChangeUICues(object sender, UICuesEventArgs e)
        {
            base.OnChangeUICues(e);
        }

        private void baseTextBox_CausesValidationChanged(object sender, EventArgs e)
        {
            base.OnCausesValidationChanged(e);
        }

        private void baseTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            base.OnKeyUp(e);
        }

        private void baseTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
        }

        private void baseTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        private void baseTextBox_TextChanged(object sender, EventArgs e)
        {
            base.OnTextChanged(e);
        }

        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.Clear(MetroPaint.BackColor.Button.Normal(Theme));
            baseTextBox.BackColor = MetroPaint.BackColor.Button.Normal(Theme);

            using (Pen p = new Pen(MetroPaint.BorderColor.Button.Normal(Theme), 2f))
            {
                e.Graphics.DrawRectangle(p, new Rectangle(1, 1, Width - 2, Height - 2));
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            baseTextBox.Location = new Point(3, 3);
            baseTextBox.Width = Width - 6;
            Height = baseTextBox.Height + 6;
        }

        private void UpdateBaseTextBox()
        {
            baseTextBox.Font = MetroFonts.TextBox(metroTextBoxSize, metroTextBoxWeight);

            Size = new Size(baseTextBox.Width + 6, baseTextBox.Height + 6);
        }
    }
}
