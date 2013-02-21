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
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

using MetroFramework.Components;
using MetroFramework.Design;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;

namespace MetroFramework.Controls
{
    public enum MetroLabelMode
    {
        Default,
        Selectable
    }

    [Designer(typeof(MetroLabelDesigner))]
    [ToolboxBitmap(typeof(Label))]
    public class MetroLabel : Label, IMetroControl
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

        private DoubleBufferedTextBox baseTextBox;

        private bool useStyleColors;
        [Category("Metro Appearance")]
        public bool UseStyleColors
        {
            get { return useStyleColors; }
            set { useStyleColors = value; Refresh(); }
        }

        private MetroLabelSize metroLabelSize = MetroLabelSize.Medium;
        [Category("Metro Appearance")]
        public MetroLabelSize FontSize
        {
            get { return metroLabelSize; }
            set { metroLabelSize = value; Refresh(); }
        }

        private MetroLabelWeight metroLabelWeight = MetroLabelWeight.Light;
        [Category("Metro Appearance")]
        public MetroLabelWeight FontWeight
        {
            get { return metroLabelWeight; }
            set { metroLabelWeight = value; Refresh(); }
        }

        private MetroLabelMode labelMode = MetroLabelMode.Default;
        [Category("Metro Appearance")]
        public MetroLabelMode LabelMode
        {
            get { return labelMode; }
            set { labelMode = value; }
        }

        private bool useCustomBackground = false;
        [Category("Metro Appearance")]
        public bool CustomBackground
        {
            get { return useCustomBackground; }
            set { useCustomBackground = value; }
        }

        #endregion

        #region Constructor

        public MetroLabel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint |
                     ControlStyles.SupportsTransparentBackColor, true);

            baseTextBox = new DoubleBufferedTextBox();
            baseTextBox.Visible = false;
            Controls.Add(baseTextBox);
        }

        #endregion

        #region Paint Methods

        protected override void OnPaint(PaintEventArgs e)
        {
            Color backColor, foreColor;

            if (useCustomBackground)
                backColor = BackColor;
            else
            {
                backColor = MetroPaint.BackColor.Form(Theme);
                if (Parent is MetroTile)
                {
                    backColor = MetroPaint.GetStyleColor(Style);
                }
            }

            if (!Enabled)
            {
                if (Parent != null)
                {
                    if (Parent is MetroTile)
                    {
                        foreColor = MetroPaint.ForeColor.Tile.Disabled(Theme);
                    }
                    else
                    {
                        foreColor = MetroPaint.ForeColor.Label.Normal(Theme);
                    }
                }
                else
                {
                    foreColor = MetroPaint.ForeColor.Label.Disabled(Theme);
                }
            }
            else
            {
                if (Parent != null)
                {
                    if (Parent is MetroTile)
                    {
                        foreColor = MetroPaint.ForeColor.Tile.Normal(Theme);
                    }
                    else
                    {
                        if (useStyleColors)
                        {
                            foreColor = MetroPaint.GetStyleColor(Style);
                        }
                        else
                        {
                            foreColor = MetroPaint.ForeColor.Label.Normal(Theme);
                        }
                    }
                }
                else
                {
                    if (useStyleColors)
                    {
                        foreColor = MetroPaint.GetStyleColor(Style);
                    }
                    else
                    {
                        foreColor = MetroPaint.ForeColor.Label.Normal(Theme);
                    }
                }
            }

            e.Graphics.Clear(backColor);

            if (LabelMode == MetroLabelMode.Selectable)
            {
                CreateBaseTextBox();
                UpdateBaseTextBox();
            }
            else
            {
                DestroyBaseTextbox();
                TextRenderer.DrawText(e.Graphics, Text, MetroFonts.Label(metroLabelSize, metroLabelWeight), ClientRectangle, foreColor, backColor, MetroPaint.GetTextFormatFlags(TextAlign));
            }            
        }

        #endregion

        #region Overridden Methods

        public override void Refresh()
        {
            if (LabelMode == MetroLabelMode.Selectable)
            {
                UpdateBaseTextBox();
            }
            
            base.Refresh();
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size preferredSize;
            base.GetPreferredSize(proposedSize);

            using (var g = CreateGraphics())
            {
                proposedSize = new Size(int.MaxValue, int.MaxValue);
                preferredSize = TextRenderer.MeasureText(g, Text, MetroFonts.Label(metroLabelSize, metroLabelWeight), proposedSize, MetroPaint.GetTextFormatFlags(TextAlign));
            }

            return preferredSize;
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Invalidate();
        }

        #endregion

        #region Label Selection Mode

        private class DoubleBufferedTextBox : TextBox
        {
            public DoubleBufferedTextBox()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            }
        }

        private void CreateBaseTextBox()
        {
            if (baseTextBox.Visible) return;

            baseTextBox.Visible = true;
            baseTextBox.BorderStyle = BorderStyle.None;
            baseTextBox.Font = MetroFonts.Label(metroLabelSize, metroLabelWeight);
            baseTextBox.Location = new Point(1, 0);
            baseTextBox.Text = Text;
            baseTextBox.ReadOnly = true;

            baseTextBox.Size = GetPreferredSize(Size.Empty);
            baseTextBox.Multiline = true;
            
            baseTextBox.DoubleClick += BaseTextBoxOnDoubleClick;
            baseTextBox.Click += BaseTextBoxOnClick;

            Controls.Add(baseTextBox);
        }

        private void DestroyBaseTextbox()
        {
            if (!baseTextBox.Visible) return;
            
            baseTextBox.DoubleClick -= BaseTextBoxOnDoubleClick;
            baseTextBox.Click -= BaseTextBoxOnClick;
            baseTextBox.Visible = false;
        }

        private void UpdateBaseTextBox()
        {
            if (!baseTextBox.Visible) return;

            baseTextBox.SuspendLayout();

            if (useCustomBackground)
                baseTextBox.BackColor = BackColor;
            else
                baseTextBox.BackColor = MetroPaint.BackColor.Form(Theme);

            if (!Enabled)
            {
                if (Parent != null)
                {
                    if (Parent is MetroTile)
                    {
                        baseTextBox.ForeColor = MetroPaint.ForeColor.Tile.Disabled(Theme);
                    }
                    else
                    {
                        if (useStyleColors)
                        {
                            baseTextBox.ForeColor = MetroPaint.GetStyleColor(Style);
                        }
                        else
                        {
                            baseTextBox.ForeColor = MetroPaint.ForeColor.Label.Disabled(Theme);
                        }
                    }
                }
                else
                {
                    if (useStyleColors)
                    {
                        baseTextBox.ForeColor = MetroPaint.GetStyleColor(Style);
                    }
                    else
                    {
                        baseTextBox.ForeColor = MetroPaint.ForeColor.Label.Disabled(Theme);
                    }
                }
            }
            else
            {
                if (Parent != null)
                {
                    if (Parent is MetroTile)
                    {
                        baseTextBox.ForeColor = MetroPaint.ForeColor.Tile.Normal(Theme);
                    }
                    else
                    {
                        if (useStyleColors)
                        {
                            baseTextBox.ForeColor = MetroPaint.GetStyleColor(Style);
                        }
                        else
                        {
                            baseTextBox.ForeColor = MetroPaint.ForeColor.Label.Normal(Theme);
                        }
                    }
                }
                else
                {
                    if (useStyleColors)
                    {
                        baseTextBox.ForeColor = MetroPaint.GetStyleColor(Style);
                    }
                    else
                    {
                        baseTextBox.ForeColor = MetroPaint.ForeColor.Label.Normal(Theme);
                    }
                }
            }

            baseTextBox.Font = MetroFonts.Label(metroLabelSize, metroLabelWeight);
            baseTextBox.Text = Text;

            Size = GetPreferredSize(Size.Empty);

            baseTextBox.ResumeLayout();
        }

        private void BaseTextBoxOnClick(object sender, EventArgs eventArgs)
        {
            Native.WinCaret.HideCaret(baseTextBox.Handle);
        }

        private void BaseTextBoxOnDoubleClick(object sender, EventArgs eventArgs)
        {
            baseTextBox.SelectAll();
            Native.WinCaret.HideCaret(baseTextBox.Handle);
        }

        #endregion
    }
}
