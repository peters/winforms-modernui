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
using System.Security;
using System.Windows.Forms;

using MetroFramework.Components;
using MetroFramework.Interfaces;
using MetroFramework.Drawing;
using MetroFramework.Native;

namespace MetroFramework.Controls
{
    [ToolboxBitmap(typeof(Panel))]
    public class MetroPanel : Panel, IMetroControl
    {
        #region Interface

        [Category("Action")]
        public event EventHandler<ScrollEventArgs> VerticalScrolled;

        [Category("Action")]
        public event EventHandler<ScrollEventArgs> HorizontalScrolled;

        [Category(MetroDefaults.PropertyCategory.Appearance)]
        public event EventHandler<MetroPaintEventArgs> CustomPaintBackground;
        protected virtual void OnCustomPaintBackground(MetroPaintEventArgs e)
        {
            if (GetStyle(ControlStyles.UserPaint) && CustomPaintBackground != null)
            {
                CustomPaintBackground(this, e);
            }
        }

        [Category(MetroDefaults.PropertyCategory.Appearance)]
        public event EventHandler<MetroPaintEventArgs> CustomPaint;
        protected virtual void OnCustomPaint(MetroPaintEventArgs e)
        {
            if (GetStyle(ControlStyles.UserPaint) && CustomPaint != null)
            {
                CustomPaint(this, e);
            }
        }

        [Category(MetroDefaults.PropertyCategory.Appearance)]
        public event EventHandler<MetroPaintEventArgs> CustomPaintForeground;
        protected virtual void OnCustomPaintForeground(MetroPaintEventArgs e)
        {
            if (GetStyle(ControlStyles.UserPaint) && CustomPaintForeground != null)
            {
                CustomPaintForeground(this, e);
            }
        }

        private MetroColorStyle metroStyle = MetroColorStyle.Default;
        [Category(MetroDefaults.PropertyCategory.Appearance)]
        [DefaultValue(MetroColorStyle.Default)]
        public MetroColorStyle Style
        {
            get
            {
                if (DesignMode || metroStyle != MetroColorStyle.Default)
                {
                    return metroStyle;
                }

                if (StyleManager != null && metroStyle == MetroColorStyle.Default)
                {
                    return StyleManager.Style;
                }
                if (StyleManager == null && metroStyle == MetroColorStyle.Default)
                {
                    return MetroDefaults.Style;
                }

                return metroStyle;
            }
            set { metroStyle = value; }
        }

        private MetroThemeStyle metroTheme = MetroThemeStyle.Default;
        [Category(MetroDefaults.PropertyCategory.Appearance)]
        [DefaultValue(MetroThemeStyle.Default)]
        public MetroThemeStyle Theme
        {
            get
            {
                if (DesignMode || metroTheme != MetroThemeStyle.Default)
                {
                    return metroTheme;
                }

                if (StyleManager != null && metroTheme == MetroThemeStyle.Default)
                {
                    return StyleManager.Theme;
                }
                if (StyleManager == null && metroTheme == MetroThemeStyle.Default)
                {
                    return MetroDefaults.Theme;
                }

                return metroTheme;
            }
            set { metroTheme = value; }
        }

        private MetroStyleManager metroStyleManager = null;
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MetroStyleManager StyleManager
        {
            get { return metroStyleManager; }
            set { metroStyleManager = value; }
        }

        private bool useCustomBackColor = false;
        [DefaultValue(false)]
        [Category(MetroDefaults.PropertyCategory.Appearance)]
        public bool UseCustomBackColor
        {
            get { return useCustomBackColor; }
            set { useCustomBackColor = value; }
        }

        private bool useCustomForeColor = false;
        [DefaultValue(false)]
        [Category(MetroDefaults.PropertyCategory.Appearance)]
        public bool UseCustomForeColor
        {
            get { return useCustomForeColor; }
            set { useCustomForeColor = value; }
        }

        private bool useStyleColors = false;
        [DefaultValue(false)]
        [Category(MetroDefaults.PropertyCategory.Appearance)]
        public bool UseStyleColors
        {
            get { return useStyleColors; }
            set { useStyleColors = value; }
        }

        [Browsable(false)]
        [Category(MetroDefaults.PropertyCategory.Behaviour)]
        [DefaultValue(false)]
        public bool UseSelectable
        {
            get { return GetStyle(ControlStyles.Selectable); }
            set { SetStyle(ControlStyles.Selectable, value); }
        }

        #endregion

        #region Fields

        public MetroScrollBar VerticalMetroScrollbar = new MetroScrollBar(MetroScrollOrientation.Vertical);
        private MetroScrollBar HorizontalMetroScrollbar = new MetroScrollBar(MetroScrollOrientation.Horizontal);

        private bool showHorizontalScrollbar = false;
        [DefaultValue(false)]
        [Category(MetroDefaults.PropertyCategory.Appearance)]
        public bool HorizontalScrollbar
        {
            get { return showHorizontalScrollbar; }
            set { showHorizontalScrollbar = value; }
        }

        [Category(MetroDefaults.PropertyCategory.Appearance)]
        public int HorizontalScrollbarSize
        {
            get { return HorizontalMetroScrollbar.ScrollbarSize; }
            set { HorizontalMetroScrollbar.ScrollbarSize = value; }
        }

        [Category(MetroDefaults.PropertyCategory.Appearance)]
        public bool HorizontalScrollbarBarColor
        {
            get { return HorizontalMetroScrollbar.UseBarColor; }
            set { HorizontalMetroScrollbar.UseBarColor = value; }
        }

        [Category(MetroDefaults.PropertyCategory.Appearance)]
        public bool HorizontalScrollbarHighlightOnWheel
        {
            get { return HorizontalMetroScrollbar.HighlightOnWheel; }
            set { HorizontalMetroScrollbar.HighlightOnWheel = value; }
        }

        private bool showVerticalScrollbar = false;
        [DefaultValue(false)]
        [Category(MetroDefaults.PropertyCategory.Appearance)]
        public bool VerticalScrollbar
        {
            get { return showVerticalScrollbar; }
            set { showVerticalScrollbar = value; }
        }

        [Category(MetroDefaults.PropertyCategory.Appearance)]
        public int VerticalScrollbarSize
        {
            get { return VerticalMetroScrollbar.ScrollbarSize; }
            set { VerticalMetroScrollbar.ScrollbarSize = value; }
        }

        [Category(MetroDefaults.PropertyCategory.Appearance)]
        public bool VerticalScrollbarBarColor
        {
            get { return VerticalMetroScrollbar.UseBarColor; }
            set { VerticalMetroScrollbar.UseBarColor = value; }
        }

        [Category(MetroDefaults.PropertyCategory.Appearance)]
        public bool VerticalScrollbarHighlightOnWheel
        {
            get { return VerticalMetroScrollbar.HighlightOnWheel; }
            set { VerticalMetroScrollbar.HighlightOnWheel = value; }
        }

        [Category(MetroDefaults.PropertyCategory.Appearance)]
        public new bool AutoScroll
        {
            get
            {
                return base.AutoScroll;
            }
            set
            {
                showHorizontalScrollbar = value;
                showVerticalScrollbar = value;

                base.AutoScroll = value;
            }
        }

        #endregion

        #region Constructor

        public MetroPanel()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);

            Controls.Add(VerticalMetroScrollbar);
            Controls.Add(HorizontalMetroScrollbar);

            VerticalMetroScrollbar.UseBarColor = true;
            HorizontalMetroScrollbar.UseBarColor = true;

            VerticalMetroScrollbar.Visible = false;
            HorizontalMetroScrollbar.Visible = false;

            VerticalMetroScrollbar.Scroll += VerticalScrollbarScroll;
            HorizontalMetroScrollbar.Scroll += HorizontalScrollbarScroll;
        }

        #endregion

        #region Scroll Events

        private void HorizontalScrollbarScroll(object sender, ScrollEventArgs e)
        {
            AutoScrollPosition = new Point(e.NewValue, VerticalMetroScrollbar.Value);
            UpdateScrollBarPositions();
            if (HorizontalScrolled != null) HorizontalScrolled(this, e);
            
        }

        private void VerticalScrollbarScroll(object sender, ScrollEventArgs e)
        {
            AutoScrollPosition = new Point(HorizontalMetroScrollbar.Value, e.NewValue);
            UpdateScrollBarPositions();
            if (VerticalScrolled != null) VerticalScrolled(this, e);
        }

        #endregion

        #region Overridden Methods

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            try
            {
                Color backColor = BackColor;

                if (!useCustomBackColor)
                {
                    backColor = MetroPaint.BackColor.Form(Theme);
                }

                if (backColor.A == 255 && BackgroundImage == null)
                {
                    e.Graphics.Clear(backColor);
                    return;
                }

                base.OnPaintBackground(e);

                OnCustomPaintBackground(new MetroPaintEventArgs(backColor, Color.Empty, e.Graphics));
            }
            catch
            {
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                if (GetStyle(ControlStyles.AllPaintingInWmPaint))
                {
                    OnPaintBackground(e);
                }

                OnCustomPaint(new MetroPaintEventArgs(Color.Empty, Color.Empty, e.Graphics));
                OnPaintForeground(e);
            }
            catch
            {
                Invalidate();
            }
        }

        protected virtual void OnPaintForeground(PaintEventArgs e)
        {
            if (DesignMode)
            {
                HorizontalMetroScrollbar.Visible = false;
                VerticalMetroScrollbar.Visible = false;
                return;
            }

            UpdateScrollBarPositions();

            if (HorizontalScrollbar)
            {
                HorizontalMetroScrollbar.Visible = HorizontalScroll.Visible;
            }
            if (HorizontalScroll.Visible)
            {
                HorizontalMetroScrollbar.Minimum = HorizontalScroll.Minimum;
                HorizontalMetroScrollbar.Maximum = HorizontalScroll.Maximum;
                HorizontalMetroScrollbar.SmallChange = HorizontalScroll.SmallChange;
                HorizontalMetroScrollbar.LargeChange = HorizontalScroll.LargeChange;
            }

            if (VerticalScrollbar)
            {
                VerticalMetroScrollbar.Visible = VerticalScroll.Visible;
            }
            if (VerticalScroll.Visible)
            {
                VerticalMetroScrollbar.Minimum = VerticalScroll.Minimum;
                VerticalMetroScrollbar.Maximum = VerticalScroll.Maximum;
                VerticalMetroScrollbar.SmallChange = VerticalScroll.SmallChange;
                VerticalMetroScrollbar.LargeChange = VerticalScroll.LargeChange;
            }

            OnCustomPaintForeground(new MetroPaintEventArgs(Color.Empty, Color.Empty, e.Graphics));
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            VerticalMetroScrollbar.Value = Math.Abs(VerticalScroll.Value);
            HorizontalMetroScrollbar.Value = Math.Abs(HorizontalScroll.Value);
        }

        [SecuritySafeCritical]
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (!DesignMode)
            {
                WinApi.ShowScrollBar(Handle, (int)WinApi.ScrollBar.SB_BOTH, 0);
            }
        }

        #endregion

        #region Management Methods

        private void UpdateScrollBarPositions()
        {
            if (DesignMode)
            {
                return;
            }

            if (!AutoScroll)
            {
                VerticalMetroScrollbar.Visible = false;
                HorizontalMetroScrollbar.Visible = false;
                return;
            }

            int _horizontalScrollHeight = HorizontalMetroScrollbar.Height;
            int _verticalScrollWidth = VerticalMetroScrollbar.Width;

            VerticalMetroScrollbar.Location = new Point(ClientRectangle.Width - VerticalMetroScrollbar.Width, ClientRectangle.Y);

            if (!VerticalScrollbar || !this.VerticalScroll.Visible)
            {
                VerticalMetroScrollbar.Visible = false;
                _verticalScrollWidth = 0;
            }

            HorizontalMetroScrollbar.Location = new Point(ClientRectangle.X, ClientRectangle.Height - HorizontalMetroScrollbar.Height);

            if (!HorizontalScrollbar || !this.HorizontalScroll.Visible)
            {
                HorizontalMetroScrollbar.Visible = false;
                _horizontalScrollHeight = 0;
            }

            VerticalMetroScrollbar.Height = ClientRectangle.Height - _horizontalScrollHeight;
            HorizontalMetroScrollbar.Width = ClientRectangle.Width - _verticalScrollWidth;
        }

        #endregion
    }
}
