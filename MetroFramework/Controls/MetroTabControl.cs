using System;
using System.ComponentModel;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;
using MetroFramework.Components;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;

namespace MetroFramework.Controls
{

    [ToolboxBitmap(typeof(TabControl))]
    public class MetroTabControl : TabControl, IMetroControl
    {
        #region Variables
        /// <summary>
        /// 
        /// </summary>
        private const int TabBottomBorderHeight = 3;

        #endregion

        #region Interface

        private MetroColorStyle _metroStyle = MetroColorStyle.Blue;

        [Category("Metro Appearance")]
        public MetroColorStyle Style
        {
            get { return StyleManager != null ? StyleManager.Style : _metroStyle; }
            set { _metroStyle = value; }
        }

        private MetroThemeStyle _metroTheme = MetroThemeStyle.Light;

        [Category("Metro Appearance")]
        public MetroThemeStyle Theme
        {
            get { return StyleManager != null ? StyleManager.Theme : _metroTheme; }
            set { _metroTheme = value; }
        }

        [Browsable(false)]
        public MetroStyleManager StyleManager { get; set; }

        private bool _useStyleColors;

        [Category("Metro Appearance")]
        public bool UseStyleColors
        {
            get { return _useStyleColors; }
            set { _useStyleColors = value; }
        }

        private MetroLabelSize _metroLabelSize = MetroLabelSize.Medium;

        [Category("Metro Appearance")]
        public MetroLabelSize FontSize
        {
            get { return _metroLabelSize; }
            set { _metroLabelSize = value; }
        }

        private MetroLabelWeight _metroLabelWeight = MetroLabelWeight.Light;

        [Category("Metro Appearance")]
        public MetroLabelWeight FontWeight
        {
            get { return _metroLabelWeight; }
            set { _metroLabelWeight = value; }
        }

        private ContentAlignment _textAlign = ContentAlignment.MiddleLeft;

        [Category("Metro Appearance")]
        public ContentAlignment TextAlign
        {
            get { return _textAlign; }
            set { _textAlign = value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        public MetroTabControl()
        {
            SetStyle(ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor, true);
        }

        #endregion

        #region Fields

        private Color _backColor = Color.Empty;

        [Browsable(false)]
        public override Color BackColor
        {
            get
            {
                if (_backColor.Equals(Color.Empty))
                {
                    return Parent == null ? DefaultBackColor : Parent.BackColor;
                }
                return _backColor;
            }
            set
            {
                if (_backColor.Equals(value))
                {
                    return;
                }
                _backColor = value;
                Invalidate();
                base.OnBackColorChanged(EventArgs.Empty);
            }
        }

        private bool _mirror;

        [Category("Metro Appearance"), Description("Draw tab pages right to left."), DefaultValue(false)]
        public bool Mirror
        {
            get { return _mirror; }
            set
            {
                if (_mirror == value)
                {
                    return;
                }
                _mirror = value;
                UpdateStyles();
            }
        }
        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="graphics"></param>
        private void DrawTabBottomBorder(int index, Graphics graphics)
        {
            using (var bgBrush = MetroPaint.GetStyleBrush(Style))
            {
                graphics.FillRectangle(bgBrush, GetTabRect(0).X + DisplayRectangle.X, GetTabRect(index).Bottom,
                                       Width - (Width - DisplayRectangle.Width + DisplayRectangle.X),
                                       TabBottomBorderHeight);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="graphics"></param>
        private void DrawTabSelected(int index, Graphics graphics)
        {
            using (var selectionBrush = new SolidBrush(MetroPaint.GetStyleColor(MetroColorStyle.Silver)))
            {
                var selectedTabRect = GetTabRect(index);
                var textAreaRect = MeasureText(TabPages[index].Text);
                graphics.FillRectangle(selectionBrush, new Rectangle
                {
                    X = selectedTabRect.X + DisplayRectangle.X,
                    Y = selectedTabRect.Bottom,
                    Width = textAreaRect.Width + (index == 0 ? DisplayRectangle.X : 0),
                    Height = TabBottomBorderHeight
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Size MeasureText(string text)
        {
            Size preferredSize;
            using (var g = CreateGraphics())
            {
                var proposedSize = new Size(int.MaxValue, int.MaxValue);
                preferredSize = TextRenderer.MeasureText(g, text, MetroFonts.Label(_metroLabelSize, _metroLabelWeight),
                                                         proposedSize,
                                                         MetroPaint.GetTextFormatFlags(TextAlign) |
                                                         TextFormatFlags.NoPadding);
            }
            return preferredSize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="graphics"></param>
        private void DrawTab(int index, Graphics graphics)
        {

            Color foreColor;
            var backColor = Parent != null ? Parent.BackColor : MetroPaint.BackColor.Form(Theme);
            var tabPage = TabPages[index];
            var tabRect = GetTabRect(index);

            if (!Enabled)
            {
                foreColor = MetroPaint.ForeColor.Label.Disabled(Theme);
            }
            else
            {
                foreColor = !_useStyleColors ? MetroPaint.ForeColor.Label.Normal(Theme) : MetroPaint.GetStyleColor(Style);
            }

            if (index == 0)
            {
                tabRect.X = DisplayRectangle.X;
            }

            TextRenderer.DrawText(graphics, tabPage.Text, MetroFonts.Label(_metroLabelSize, _metroLabelWeight),
                                  tabRect, foreColor, backColor, MetroPaint.GetTextFormatFlags(TextAlign));
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
            e.Graphics.Clear(BackColor);
            for (var index = 0; index < TabPages.Count; index++)
            {
                if (index != SelectedIndex)
                {
                    DrawTab(index, e.Graphics);
                }
            }
            if (SelectedIndex <= -1)
            {
                return;
            }
            DrawTabBottomBorder(SelectedIndex, e.Graphics);
            DrawTab(SelectedIndex, e.Graphics);
            DrawTabSelected(SelectedIndex, e.Graphics);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Invalidate();
        }

        protected override void OnParentBackColorChanged(EventArgs e)
        {
            base.OnParentBackColorChanged(e);
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }

        #endregion

        #region Overrides
        /// <summary>
        /// 
        /// </summary>
        protected override CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                const int WS_EX_LAYOUTRTL = 0x400000;
                const int WS_EX_NOINHERITLAYOUT = 0x100000;
                var cp = base.CreateParams;
                if (_mirror) // RightToLeftLayout
                {
                    cp.ExStyle = cp.ExStyle | WS_EX_LAYOUTRTL | WS_EX_NOINHERITLAYOUT;
                }
                return cp;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private new Rectangle GetTabRect(int index)
        {
            return (index < 0) ? new Rectangle() : base.GetTabRect(index);
        }
        #endregion

    }
}