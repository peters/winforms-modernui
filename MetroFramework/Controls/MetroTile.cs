using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

using MetroFramework.Drawing;
using MetroFramework.Design;
using MetroFramework.Components;
using MetroFramework.Interfaces;

namespace MetroFramework.Controls
{
    [Designer(typeof(MetroTileDesigner))]
    [ToolboxBitmap(typeof(Button))]
    public class MetroTile : Button, IMetroControl
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

        [Browsable(false)]
        public override Color BackColor
        {
            get
            {
                return MetroPaint.BackColor.Form(Theme);
            }
        }

        private int tileCount = 0;
        public int TileCount
        {
            get { return tileCount; }
            set { tileCount = value; }
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
        }

        #endregion

        #region Paint Methods

        protected override void OnPaint(PaintEventArgs e)
        {
            Color backColor, foreColor;

            backColor = MetroPaint.GetStyleColor(Style);

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

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            if (!isPressed)
            {
                e.Graphics.Clear(MetroPaint.GetStyleColor(Style));
            }
            else
            {
                e.Graphics.Clear(MetroPaint.BackColor.Form(Theme));
                
                using (SolidBrush b = MetroPaint.GetStyleBrush(Style))
                {
                    Point[] polyPoints = new Point[] { new Point(0,0), new Point(Width-1,2),new Point(Width-1,Height-2),new Point(0,Height) };
                    e.Graphics.FillPolygon(b, polyPoints);
                }
            }

            if (TileCount > 0)
            {
                Size countSize = TextRenderer.MeasureText(TileCount.ToString(), MetroFonts.TileCount);

                //using (SolidBrush b = MetroPaint.GetStyleBrush(Style))
                //{
                //    b.Color = ControlPaint.Dark(b.Color);

                //    int ellSize = Math.Max(countSize.Width, countSize.Height);
                //    e.Graphics.FillEllipse(b, new Rectangle(new Point(Width - ellSize - 5, 5), new Size(ellSize, ellSize)));
                //}

                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                TextRenderer.DrawText(e.Graphics, TileCount.ToString(), MetroFonts.TileCount, new Point(Width - countSize.Width, 0), foreColor);
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            }

            Size textSize = TextRenderer.MeasureText(Text, MetroFonts.Tile);
            TextRenderer.DrawText(e.Graphics, Text, MetroFonts.Tile, new Point(0, Height-textSize.Height), foreColor);
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
