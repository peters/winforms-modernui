using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MetroFramework.Components;
using MetroFramework.Design;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;

namespace MetroFramework.Controls
{
    [Designer(typeof(MetroProgressBarDesigner))]
    [ToolboxBitmap(typeof(ProgressBar))]
    public partial class MetroProgressBar : ProgressBar, IMetroControl
    {

        #region IMetroControl
        /// <summary>
        /// 
        /// </summary>
        public new MetroColorStyle Style { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public MetroThemeStyle Theme { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public MetroStyleManager StyleManager { get; set; }
        #endregion

        #region Fields

        private MetroLabelSize _metroLabelSize = MetroLabelSize.Medium;

        [Category("Metro Appearance")]
        public MetroLabelSize FontSize
        {
            get
            {
                return _metroLabelSize;
            }
            set
            {
                _metroLabelSize = value;
            }
        }

        private MetroLabelWeight _metroLabelWeight = MetroLabelWeight.Light;

        [Category("Metro Appearance")]
        public MetroLabelWeight FontWeight
        {
            get
            {
                return _metroLabelWeight;
            }
            set
            {
                _metroLabelWeight = value;
            }
        }

        private ContentAlignment _textAlign = ContentAlignment.MiddleRight;

        [Category("Metro Appearance")]
        public ContentAlignment TextAlign
        {
            get
            {
                return _textAlign;
            }
            set
            {
                _textAlign = value;
            }
        }

        [Category("Metro Appearance"),
        Description("Hide percent value from appearing on progressbar"),
        DefaultValue(false)]
        public bool HideProgressText { get; set; }

        private bool _useStyleColors;
        [Category("Metro Appearance")]
        public bool UseStyleColors
        {
            get { return _useStyleColors; }
            set { _useStyleColors = value; }
        }

        private ProgressBarStyle _progressBarStyle = ProgressBarStyle.Continuous;
        [Category("Metro Appearance")]
        public ProgressBarStyle ProgressBarStyle
        {
            get
            {
                _progressBarStyle = ProgressBarStyle.Continuous;
                Invalidate();
                return _progressBarStyle;
            }
            set
            {
                Invalidate();
                _progressBarStyle = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public new int Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                if (value > Maximum) return;
                base.Value = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        [Browsable(false)]
        public double ProgressTotalPercent
        {
            get
            {
                return (1 - (double)(Maximum - Value) / Maximum) * 100;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        public double ProgressTotalValue
        {
            get
            {
                return 1 - (double)(Maximum - Value) / Maximum;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        public string ProgressPercentText
        {
            get
            {
                return string.Format("{0}%", Math.Round(ProgressTotalPercent));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private double ProgressBarWidth
        {
            get
            {
                return ((double)Value / Maximum) * ClientRectangle.Width;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private double ProgressBarRemainingWidth
        {
            get
            {
                return ProgressBarWidth > 0 ? ((double)(Maximum - Value) / Maximum) * ClientRectangle.Width : 0;
            }
        }
        #endregion

        #region Constructor

        public MetroProgressBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);
        }

        #endregion

        #region Private methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphics"></param>
        private void DrawProgressCurrent(Graphics graphics)
        {
            using (var bgBrush = new SolidBrush(MetroPaint.GetStyleColor(MetroColorStyle.Orange)))
            {
                graphics.FillRectangle(bgBrush, ClientRectangle.X, ClientRectangle.X, (int)ProgressBarWidth, ClientRectangle.Height);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphics"></param>
        private void DrawProgressText(IDeviceContext graphics)
        {
            
            if (HideProgressText) return;

            Color foreColor;

            var backColor = Color.Transparent;

            if (!Enabled)
            {
                foreColor = MetroPaint.ForeColor.Label.Disabled(Theme);
            }
            else
            {
                foreColor = !_useStyleColors ? MetroPaint.ForeColor.Label.Normal(Theme) : MetroPaint.GetStyleColor(Style);
            }

            var remaining = (int) (ProgressBarWidth + ProgressBarRemainingWidth);
            if (ProgressTotalPercent == 0)
            {
                remaining = Width;
            }
            TextRenderer.DrawText(graphics, ProgressPercentText, MetroFonts.Label(_metroLabelSize, _metroLabelWeight),
                new Rectangle(ClientRectangle.X, ClientRectangle.Y, remaining,
                ClientRectangle.Height), foreColor, backColor, MetroPaint.GetTextFormatFlags(TextAlign));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphics"></param>
        private void DrawProgressRemaining(Graphics graphics)
        {
            using (var bgBrush = new SolidBrush(MetroPaint.GetStyleColor(MetroColorStyle.Blue)))
            {
                var x = ProgressTotalPercent == 0 ? 0 : (int)ProgressBarWidth;
                var width = ProgressTotalPercent == 0 ? Width : (int) ProgressBarRemainingWidth;
                graphics.FillRectangle(bgBrush, ClientRectangle.X + x, ClientRectangle.X, width, ClientRectangle.Height);
            }
        }

        #endregion

        #region Paint methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            switch (_progressBarStyle)
            {
                case ProgressBarStyle.Continuous:
                    DrawProgressCurrent(e.Graphics);
                    DrawProgressRemaining(e.Graphics);
                    DrawProgressText(e.Graphics);
                    break;
                case ProgressBarStyle.Blocks:
                case ProgressBarStyle.Marquee:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="proposedSize"></param>
        /// <returns></returns>
        public override Size GetPreferredSize(Size proposedSize)
        {
            Size preferredSize;

            using (Graphics g = CreateGraphics())
            {
                proposedSize = new Size(int.MaxValue, int.MaxValue);
                preferredSize = TextRenderer.MeasureText(g, ProgressPercentText, MetroFonts.Label(_metroLabelSize, _metroLabelWeight), proposedSize, MetroPaint.GetTextFormatFlags(TextAlign));
            }
            return preferredSize;
        }
        #endregion

    }
}
