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

        private bool useStyleColors = false;
        [Category("Metro Appearance")]
        public bool UseStyleColors
        {
            get { return useStyleColors; }
            set { useStyleColors = value; }
        }

        private MetroLabelSize metroLabelSize = MetroLabelSize.Medium;
        [Category("Metro Appearance")]
        public MetroLabelSize FontSize
        {
            get { return metroLabelSize; }
            set { metroLabelSize = value; }
        }

        private MetroLabelWeight metroLabelWeight = MetroLabelWeight.Light;
        [Category("Metro Appearance")]
        public MetroLabelWeight FontWeight
        {
            get { return metroLabelWeight; }
            set { metroLabelWeight = value; }
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
        }

        #endregion

        #region Paint Methods

        protected override void OnPaint(PaintEventArgs e)
        {
            Color backColor, foreColor;

            if (Parent != null)
                backColor = Parent.BackColor;
            else
                backColor = MetroPaint.BackColor.Form(Theme);

            if (!Enabled)
            {
                foreColor = MetroPaint.ForeColor.Label.Disabled(Theme);
            }
            else
            {
                foreColor = !useStyleColors ? MetroPaint.ForeColor.Label.Normal(Theme) : MetroPaint.GetStyleColor(Style);
            }

            var actualRectangle = ClientRectangle;
            var flags = MetroPaint.GetTextFormatFlags(TextAlign);
            var font = MetroFonts.Label(metroLabelSize, metroLabelWeight);

            // Handle resizing when autosizing is enabled.
            if (AutoSize)
            {
                var proposedSize = new Size(int.MaxValue, int.MinValue);
                var actualSize = TextRenderer.MeasureText(e.Graphics, Text, font, proposedSize, flags);
                actualRectangle = new Rectangle(ClientRectangle.X, ClientRectangle.Y, actualSize.Width, actualSize.Height);
                Size = actualSize;
            }
           
            e.Graphics.Clear(backColor);
            TextRenderer.DrawText(e.Graphics, Text, font, actualRectangle, foreColor, backColor, flags);

          

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
