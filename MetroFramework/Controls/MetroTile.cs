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

using MetroFramework.Drawing;
using MetroFramework.Components;
using MetroFramework.Interfaces;

namespace MetroFramework.Controls
{
    [Designer("MetroFramework.Design.MetroTileDesigner, " + AssemblyRef.MetroFrameworkDesignSN)]
    [ToolboxBitmap(typeof(Button))]
    public class MetroTile : Button, IContainerControl, IMetroControl
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

        private Control activeControl = null;
        [Browsable(false)]
        public Control ActiveControl
        {
            get { return activeControl; }
            set { activeControl = value; }
        }

        public bool ActivateControl(Control ctrl)
        {
            if (Controls.Contains(ctrl))
            {
                ctrl.Select();
                activeControl = ctrl;
                return true;
            }

            return false;
        }

        #endregion

        #region Fields

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

        private bool paintTileCount = true;
        [DefaultValue(true)]
        [Category("Metro Appearance")]
        public bool PaintTileCount
        {
            get { return paintTileCount; }
            set { paintTileCount = value; }
        }

        private int tileCount = 0;
        [DefaultValue(0)]
        public int TileCount
        {
            get { return tileCount; }
            set { tileCount = value; }
        }

        [DefaultValue(ContentAlignment.BottomLeft)]
        public new ContentAlignment TextAlign
        {
            get { return base.TextAlign; }
            set { base.TextAlign = value; }
        }

        private Image tileImage = null;
        [DefaultValue(null)]
        [Category("Metro Appearance")]
        public Image TileImage
        {
            get { return tileImage; }
            set { tileImage = value; }
        }

        private bool useTileImage = false;
        [DefaultValue(false)]
        [Category("Metro Appearance")]
        public bool UseTileImage
        {
            get { return useTileImage; }
            set { useTileImage = value; }
        }

        private ContentAlignment tileImageAlign = ContentAlignment.TopLeft;
        [DefaultValue(ContentAlignment.TopLeft)]
        [Category("Metro Appearance")]
        public ContentAlignment TileImageAlign
        {
            get { return tileImageAlign; }
            set { tileImageAlign = value; }
        }

        private MetroTileTextSize tileTextFontSize = MetroTileTextSize.Medium;
        [DefaultValue(MetroTileTextSize.Medium)]
        [Category("Metro Appearance")]
        public MetroTileTextSize TileTextFontSize
        {
            get { return tileTextFontSize; }
            set { tileTextFontSize = value; Refresh(); }
        }

        private MetroTileTextWeight tileTextFontWeight = MetroTileTextWeight.Light;
        [DefaultValue(MetroTileTextWeight.Light)]
        [Category("Metro Appearance")]
        public MetroTileTextWeight TileTextFontWeight
        {
            get { return tileTextFontWeight; }
            set { tileTextFontWeight = value; Refresh(); }
        }

        private bool isHovered = false;
        private bool isPressed = false;
        private bool isFocused = false;

        #endregion

        #region Constructor

        public MetroTile()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);

            TextAlign = ContentAlignment.BottomLeft;
        }

        #endregion

        #region Paint Methods

        protected override void OnPaint(PaintEventArgs e)
        {
            Color backColor, foreColor;

            backColor = MetroPaint.GetStyleColor(Style);

            if (useCustomBackground)
            {
                backColor = BackColor;
            }

            if (isHovered && !isPressed && Enabled)
            {
                foreColor = MetroPaint.ForeColor.Tile.Hover(Theme);
            }
            else if (isHovered && isPressed && Enabled)
            {
                foreColor = MetroPaint.ForeColor.Tile.Press(Theme);
            }
            else if (!Enabled)
            {
                foreColor = MetroPaint.ForeColor.Tile.Disabled(Theme);
            }
            else
            {
                foreColor = MetroPaint.ForeColor.Tile.Normal(Theme);
            }

            if (useCustomForeColor)
            {
                foreColor = ForeColor;
            }

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            if (!isPressed)
            {
                e.Graphics.Clear(backColor);
            }
            else
            {
                e.Graphics.Clear(MetroPaint.BackColor.Form(Theme));
                
                using (SolidBrush b = new SolidBrush(backColor))
                {
                    Point[] polyPoints = new Point[] { new Point(0,0), new Point(Width-1,2),new Point(Width-1,Height-2),new Point(0,Height) };
                    e.Graphics.FillPolygon(b, polyPoints);
                }
            }

            if (useTileImage)
            {
                if (tileImage != null)
                {
                    Rectangle rect;
                    switch (tileImageAlign)
                    {
                        case ContentAlignment.BottomLeft:
                            rect = new Rectangle(new Point(0, Height - TileImage.Height), new System.Drawing.Size(TileImage.Width, TileImage.Height));
                            break;

                        case ContentAlignment.BottomCenter:
                            rect = new Rectangle(new Point(Width / 2 - TileImage.Width / 2, Height - TileImage.Height), new System.Drawing.Size(TileImage.Width, TileImage.Height));
                            break;

                        case ContentAlignment.BottomRight:
                            rect = new Rectangle(new Point(Width - TileImage.Width, Height - TileImage.Height), new System.Drawing.Size(TileImage.Width, TileImage.Height));
                            break;

                        case ContentAlignment.MiddleLeft:
                            rect = new Rectangle(new Point(0, Height / 2 - TileImage.Height / 2), new System.Drawing.Size(TileImage.Width, TileImage.Height));
                            break;

                        case ContentAlignment.MiddleCenter:
                            rect = new Rectangle(new Point(Width / 2 - TileImage.Width / 2, Height / 2 - TileImage.Height / 2), new System.Drawing.Size(TileImage.Width, TileImage.Height));
                            break;

                        case ContentAlignment.MiddleRight:
                            rect = new Rectangle(new Point(Width - TileImage.Width, Height / 2 - TileImage.Height / 2), new System.Drawing.Size(TileImage.Width, TileImage.Height));
                            break;

                        case ContentAlignment.TopLeft:
                            rect = new Rectangle(new Point(0, 0), new System.Drawing.Size(TileImage.Width, TileImage.Height));
                            break;

                        case ContentAlignment.TopCenter:
                            rect = new Rectangle(new Point(Width / 2 - TileImage.Width / 2, 0), new System.Drawing.Size(TileImage.Width, TileImage.Height));
                            break;

                        case ContentAlignment.TopRight:
                            rect = new Rectangle(new Point(Width - TileImage.Width, 0), new System.Drawing.Size(TileImage.Width, TileImage.Height));
                            break;

                        default:
                            rect = new Rectangle(new Point(0, 0), new System.Drawing.Size(TileImage.Width, TileImage.Height));
                            break;
                    }

                    e.Graphics.DrawImage(TileImage, rect);
                }
            }

            if (TileCount > 0 && paintTileCount)
            {
                Size countSize = TextRenderer.MeasureText(TileCount.ToString(), MetroFonts.TileCount);

                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                TextRenderer.DrawText(e.Graphics, TileCount.ToString(), MetroFonts.TileCount, new Point(Width - countSize.Width, 0), foreColor);
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            }

            Size textSize = TextRenderer.MeasureText(Text, MetroFonts.Tile(tileTextFontSize, tileTextFontWeight));

            TextFormatFlags flags = TextFormatFlags.LeftAndRightPadding | TextFormatFlags.EndEllipsis;
            Rectangle clientRectangle = ClientRectangle;

            switch (TextAlign)
            {
                case ContentAlignment.BottomCenter:
                    flags |= TextFormatFlags.Bottom;
                    flags |= TextFormatFlags.HorizontalCenter;
                    break;
                    
                case ContentAlignment.BottomRight:
                    flags |= TextFormatFlags.Bottom;
                    flags |= TextFormatFlags.Right;
                    break;

                case ContentAlignment.MiddleLeft:
                    flags |= TextFormatFlags.VerticalCenter;
                    flags |= TextFormatFlags.Left;
                    break;

                case ContentAlignment.MiddleCenter:
                    flags |= TextFormatFlags.VerticalCenter;
                    flags |= TextFormatFlags.HorizontalCenter;
                    break;

                case ContentAlignment.MiddleRight:
                    flags |= TextFormatFlags.VerticalCenter;
                    flags |= TextFormatFlags.Right;
                    break;

                case ContentAlignment.TopLeft:
                    flags |= TextFormatFlags.Top;
                    flags |= TextFormatFlags.Left;
                    break;

                case ContentAlignment.TopCenter:
                    flags |= TextFormatFlags.Top;
                    flags |= TextFormatFlags.HorizontalCenter;
                    break;

                case ContentAlignment.TopRight:
                    flags |= TextFormatFlags.Top;
                    flags |= TextFormatFlags.Right;
                    break;

                default:
                case ContentAlignment.BottomLeft:
                    flags |= TextFormatFlags.Bottom;
                    flags |= TextFormatFlags.Left;
                    break;
            }

            TextRenderer.DrawText(e.Graphics, Text, MetroFonts.Tile(tileTextFontSize,tileTextFontWeight), clientRectangle, foreColor, flags);

            if (false && isFocused)
                ControlPaint.DrawFocusRectangle(e.Graphics, ClientRectangle);
        }

        #endregion

        #region Focus Methods

        protected override void OnGotFocus(EventArgs e)
        {
            isFocused = true;
            Invalidate();

            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            isFocused = false;
            isHovered = false;
            isPressed = false;
            Invalidate();

            base.OnLostFocus(e);
        }

        protected override void OnEnter(EventArgs e)
        {
            isFocused = true;
            Invalidate();

            base.OnEnter(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            isFocused = false;
            isHovered = false;
            isPressed = false;
            Invalidate();

            base.OnLeave(e);
        }

        #endregion

        #region Keyboard Methods

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                isHovered = true;
                isPressed = true;
                Invalidate();
            }

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            isHovered = false;
            isPressed = false;
            Invalidate();

            base.OnKeyUp(e);
        }

        #endregion

        #region Mouse Methods

        protected override void OnMouseEnter(EventArgs e)
        {
            isHovered = true;
            Invalidate();

            base.OnMouseEnter(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isPressed = true;
                Invalidate();
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            isPressed = false;
            Invalidate();

            base.OnMouseUp(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            isHovered = false;
            Invalidate();

            base.OnMouseLeave(e);
        }

        #endregion

        #region Overridden Methods

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Invalidate();
        }

        #endregion
    }
}
