/**
 * MetroFramework - Modern UI for WinForms
 * 
 * The MIT License (MIT)
 * Copyright (c) 2011 Sven Walter, http://github.com/viperneo
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in the 
 * Software without restriction, including without limitation the rights to use, copy, 
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
 * and to permit persons to whom the Software is furnished to do so, subject to the 
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
 * PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
 * CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
 * OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MetroFramework.Components;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;

namespace MetroFramework.Controls
{
    [Designer("MetroFramework.Design.MetroTextBoxDesigner, " + AssemblyRef.MetroFrameworkDesignSN)]
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

        private PromptedTextBox baseTextBox;
        
        private bool useStyleColors = false;
        [DefaultValue(false)]
        [Category("Metro Appearance")]
        public bool UseStyleColors
        {
            get { return useStyleColors; }
            set { useStyleColors = value; UpdateBaseTextBox(); }
        }

        private MetroTextBoxSize metroTextBoxSize = MetroTextBoxSize.Small;
        [DefaultValue(MetroTextBoxSize.Small)]
        [Category("Metro Appearance")]
        public MetroTextBoxSize FontSize
        {
            get { return metroTextBoxSize; }
            set { metroTextBoxSize = value; UpdateBaseTextBox(); }
        }

        private MetroTextBoxWeight metroTextBoxWeight = MetroTextBoxWeight.Regular;
        [DefaultValue(MetroTextBoxWeight.Regular)]
        [Category("Metro Appearance")]
        public MetroTextBoxWeight FontWeight
        {
            get { return metroTextBoxWeight; }
            set { metroTextBoxWeight = value; UpdateBaseTextBox(); }
        }

        private bool useCustomBackground = false;
        [DefaultValue(false)]
        [Category("Metro Appearance")]
        public bool CustomBackground
        {
            get { return useCustomBackground; }
            set { useCustomBackground = value; }
        }

        private bool useCustomForeColor = false;
        [DefaultValue(false)]
        [Category("Metro Appearance")]
        public bool CustomForeColor
        {
            get { return useCustomForeColor; }
            set { useCustomForeColor = value; }
        }

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DefaultValue("")]
        [Category("Metro Appearance")]
        public string PromptText
        {
            get { return baseTextBox.PromptText; }
            set { baseTextBox.PromptText = value; }
        }

        #endregion

        #region Routing Fields

        public override ContextMenu ContextMenu
        {
            get { return baseTextBox.ContextMenu; }
            set 
            {
                ContextMenu = value;
                baseTextBox.ContextMenu = value; 
            }
        }

        public override ContextMenuStrip ContextMenuStrip
        {
            get { return baseTextBox.ContextMenuStrip; }
            set
            {
                ContextMenuStrip = value;
                baseTextBox.ContextMenuStrip = value;
            }
        }

        [DefaultValue(false)]
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

        [Browsable(false)]
        public string SelectedText
        {
            get { return baseTextBox.SelectedText;  }
            set { baseTextBox.Text = value; }
        }

        [DefaultValue(false)]
        public bool ReadOnly
        {
            get { return baseTextBox.ReadOnly; }
            set { baseTextBox.ReadOnly = value; }
        }

        public char PasswordChar
        {
            get { return baseTextBox.PasswordChar; }
            set { baseTextBox.PasswordChar = value; }
        }

        [DefaultValue(false)]
        public bool UseSystemPasswordChar
        {
            get { return baseTextBox.UseSystemPasswordChar; }
            set { baseTextBox.UseSystemPasswordChar = value; }
        }

        [DefaultValue(HorizontalAlignment.Left)]
        public HorizontalAlignment TextAlign
        {
            get { return baseTextBox.TextAlign; }
            set { baseTextBox.TextAlign = value; }
        }

        [DefaultValue(true)]
        public new bool TabStop
        {
            get { return baseTextBox.TabStop; }
            set { baseTextBox.TabStop = value; }
        }

        public int MaxLength
        {
            get { return baseTextBox.MaxLength; }
            set { baseTextBox.MaxLength = value; }
        }

        public ScrollBars ScrollBars
        {
            get { return baseTextBox.ScrollBars; }
            set { baseTextBox.ScrollBars = value; }
        }

        #endregion

        #region Constructor

        public MetroTextBox()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, true);

            base.TabStop = false;

            CreateBaseTextBox();
            UpdateBaseTextBox();
            AddEventHandler();       
        }

        #endregion

        #region Routing Methods

        public event EventHandler AcceptsTabChanged;
        private void BaseTextBoxAcceptsTabChanged(object sender, EventArgs e)
        {
            if (AcceptsTabChanged != null)
                AcceptsTabChanged(this, e);
        }
        
        private void BaseTextBoxSizeChanged(object sender, EventArgs e)
        {
            base.OnSizeChanged(e);
        }

        private void BaseTextBoxCursorChanged(object sender, EventArgs e)
        {
            base.OnCursorChanged(e);
        }

        private void BaseTextBoxContextMenuStripChanged(object sender, EventArgs e)
        {
            base.OnContextMenuStripChanged(e);
        }

        private void BaseTextBoxContextMenuChanged(object sender, EventArgs e)
        {
            base.OnContextMenuChanged(e);
        }

        private void BaseTextBoxClientSizeChanged(object sender, EventArgs e)
        {
            base.OnClientSizeChanged(e);
        }

        private void BaseTextBoxClick(object sender, EventArgs e)
        {
            base.OnClick(e);
        }

        private void BaseTextBoxChangeUiCues(object sender, UICuesEventArgs e)
        {
            base.OnChangeUICues(e);
        }

        private void BaseTextBoxCausesValidationChanged(object sender, EventArgs e)
        {
            base.OnCausesValidationChanged(e);
        }

        private void BaseTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            base.OnKeyUp(e);
        }

        private void BaseTextBoxKeyPress(object sender, KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
        }

        private void BaseTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        private void BaseTextBoxTextChanged(object sender, EventArgs e)
        {
            base.OnTextChanged(e);
        }

        public void Select(int start, int length)
        {
            baseTextBox.Select(start, length);
        }

        public void SelectAll()
        {
            baseTextBox.SelectAll();
        }

        public void Clear()
        {
            baseTextBox.Clear();
        }

        public void AppendText(string text)
        {
            baseTextBox.AppendText(text);
        }

        #endregion

        #region Paint Methods

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (useCustomBackground)
            {
                e.Graphics.Clear(BackColor);
                baseTextBox.BackColor = BackColor;
            }
            else
            {
                e.Graphics.Clear(MetroPaint.BackColor.Button.Normal(Theme));
                baseTextBox.BackColor = MetroPaint.BackColor.Button.Normal(Theme);
            }

            if (useCustomForeColor)
            {
                baseTextBox.ForeColor = ForeColor;
            }
            else
            {
                baseTextBox.ForeColor = MetroPaint.ForeColor.Button.Normal(Theme);
            }

            Color borderColor = MetroPaint.BorderColor.Button.Normal(Theme);
            if (useStyleColors)
                borderColor = MetroPaint.GetStyleColor(Style);

            using (Pen p = new Pen(borderColor))
            {
                e.Graphics.DrawRectangle(p, new Rectangle(0, 0, Width - 1, Height - 1));
            }
        }

        #endregion

        #region Overridden Methods

        public override void Refresh()
        {
            base.Refresh();
            UpdateBaseTextBox();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateBaseTextBox();
        }

        #endregion

        #region Private Methods

        private void CreateBaseTextBox()
        {
            if (baseTextBox != null) return;

            baseTextBox = new PromptedTextBox();

            baseTextBox.BorderStyle = BorderStyle.None;
            baseTextBox.Font = MetroFonts.TextBox(metroTextBoxSize, metroTextBoxWeight);
            baseTextBox.Location = new Point(3, 3);
            baseTextBox.Size = new Size(Width - 6, Height - 6);

            Size = new Size(baseTextBox.Width + 6, baseTextBox.Height + 6);

            baseTextBox.TabStop = true;
            //baseTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;

            Controls.Add(baseTextBox);
        }

        private void AddEventHandler()
        {
            baseTextBox.AcceptsTabChanged += BaseTextBoxAcceptsTabChanged;

            baseTextBox.CausesValidationChanged += BaseTextBoxCausesValidationChanged;
            baseTextBox.ChangeUICues += BaseTextBoxChangeUiCues;
            baseTextBox.Click += BaseTextBoxClick;
            baseTextBox.ClientSizeChanged += BaseTextBoxClientSizeChanged;
            baseTextBox.ContextMenuChanged += BaseTextBoxContextMenuChanged;
            baseTextBox.ContextMenuStripChanged += BaseTextBoxContextMenuStripChanged;
            baseTextBox.CursorChanged += BaseTextBoxCursorChanged;

            baseTextBox.KeyDown += BaseTextBoxKeyDown;
            baseTextBox.KeyPress += BaseTextBoxKeyPress;
            baseTextBox.KeyUp += BaseTextBoxKeyUp;

            baseTextBox.SizeChanged += BaseTextBoxSizeChanged;

            baseTextBox.TextChanged += BaseTextBoxTextChanged;
        }

        private void UpdateBaseTextBox()
        {
            if (baseTextBox == null) return;

            baseTextBox.Font = MetroFonts.TextBox(metroTextBoxSize, metroTextBoxWeight);
            baseTextBox.Location = new Point(3, 3);
            baseTextBox.Size = new Size(Width - 6, Height - 6);
        }

        #endregion

        #region PromptedTextBox

        private class PromptedTextBox : TextBox
        {
            private const int OCM_COMMAND = 0x2111;
            private const int WM_PAINT = 15;

            private bool drawPrompt;

            private string promptText = "";
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [DefaultValue("")]
            public string PromptText
            {
                get { return promptText; }
                set
                {
                    promptText = value.Trim();
                    Invalidate();
                }
            }

            private void DrawTextPrompt()
            {
                using (Graphics graphics = CreateGraphics())
                {
                    DrawTextPrompt(graphics);
                }
            }

            private void DrawTextPrompt(Graphics g)
            {
                TextFormatFlags flags = TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis;
                Rectangle clientRectangle = ClientRectangle;

                switch (TextAlign)
                {
                    case HorizontalAlignment.Left:
                        clientRectangle.Offset(1, 1);
                        break;

                    case HorizontalAlignment.Right:
                        flags |= TextFormatFlags.Right;
                        clientRectangle.Offset(0, 1);
                        break;

                    case HorizontalAlignment.Center:
                        flags |= TextFormatFlags.HorizontalCenter;
                        clientRectangle.Offset(0, 1);
                        break;
                }

                TextRenderer.DrawText(g, promptText, Font, clientRectangle, SystemColors.GrayText, BackColor, flags);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                if (drawPrompt)
                {
                    DrawTextPrompt(e.Graphics);
                }
            }

            protected override void OnTextAlignChanged(EventArgs e)
            {
                base.OnTextAlignChanged(e);
                Invalidate();
            }

            protected override void OnTextChanged(EventArgs e)
            {
                base.OnTextChanged(e);
                drawPrompt = Text.Length == 0;
            }

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);
                if (((m.Msg == WM_PAINT) || (m.Msg == OCM_COMMAND)) &&
                    (drawPrompt && !GetStyle(ControlStyles.UserPaint)))
                {
                    DrawTextPrompt();
                }
            }

        }

        #endregion
    }
}
