using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using MetroFramework.Components;
using MetroFramework.Design;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;
using MetroFramework.Native;

namespace MetroFramework.Controls
{

    #region MetroTabPageCollectionEditor
    internal class MetroTabPageCollectionEditor : CollectionEditor
    {
        protected override CollectionForm
            CreateCollectionForm()
        {
            var baseForm = base.CreateCollectionForm();
            baseForm.Text = "MetroTabPage Collection Editor";
            return baseForm;
        }

        public MetroTabPageCollectionEditor(Type type)
            : base(type)
        {
        }

        protected override Type CreateCollectionItemType()
        {
            return typeof (TabPage);
        }

        protected override Type[] CreateNewItemTypes()
        {
            return new[]
            {
                typeof (TabPage)
            };
        }
    }

    #endregion

    #region MetroTabPage

    [ToolboxItem(false)]
    class TabPageScrollBar : MetroScrollBar
    {
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            // Use native one instead
            // Todo: Sven, could you investigate how to emulate
            // the default mouse wheel scroll behavior?
        }
    }

    [Designer(typeof (ScrollableControlDesigner))]
    public class TabPage : System.Windows.Forms.TabPage, IMetroControl
    {
        
        #region Variables
        /// <summary>
        /// 
        /// </summary>
        private readonly TabPageScrollBar _verticalScrollbar = new TabPageScrollBar
        {
            Orientation = ScrollBarOrientation.Vertical,
            Width = 10
        };

        /// <summary>
        /// 
        /// </summary>
        private readonly TabPageScrollBar _horizontalScrollbar = new TabPageScrollBar
        {
            Orientation = ScrollBarOrientation.Horizontal,
            Height = 6
        };

        #endregion

        #region Fields

        [Category("Metro Appearance"), Browsable(true), Description("Shode / hide horizontal scrollbar.")]
        public bool HorizontalScrollBarShow { get; set; }

        [Category("Metro Appearance"), Browsable(true), Description("Shode / hide vertical scrollbar.")]
        public bool VerticalScrollBarShow { get; set; }

        [Category("Metro Appearance"), Browsable(true), Description("Allow TabPage to be scrolled.")]
        public new bool AutoScroll
        {
            get
            {
                return base.AutoScroll;
            }
            set
            {
                if (value)
                {
                    HorizontalScrollBarShow = true;
                    VerticalScrollBarShow = true;
                }
                base.AutoScroll = value;
            }
        }
        #endregion

        #region IMetroControl
        /// <summary>
        /// 
        /// </summary>
        public MetroColorStyle Style { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public MetroThemeStyle Theme { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public MetroStyleManager StyleManager { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// http://www.codeproject.com/Articles/226381/Scrolling-Panel
        /// http://cyotek.com/blog/tag/scrollablecontroldesigner
        /// </summary>
        public TabPage()
        {
            SetStyle(ControlStyles.UserPaint |
             ControlStyles.AllPaintingInWmPaint |
             ControlStyles.ResizeRedraw |
             ControlStyles.OptimizedDoubleBuffer |
             ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
        }
        #endregion

        #region Scroll events
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HorizontalScrollbarScroll(object sender, ScrollEventArgs e)
        {
            UpdateScrollBarsPositionWhileScrolling(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void VerticalScrollbarScroll(object sender, ScrollEventArgs e)
        {
            AutoScrollPosition = new Point(0, e.NewValue);
            _verticalScrollbar.Top = ClientRectangle.Top;
            UpdateScrollBarsPositionWhileScrolling(e);
        }
        #endregion

        #region Overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventargs"></param>
        protected override void OnSizeChanged(EventArgs eventargs)
        {
            UpdateScrollBarPositions();
            base.OnSizeChanged(eventargs);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            
            // Do nothing while in design mode.
            if (DesignMode)
            {
                return;
            }

            // Only attempt to set visibility if configured
            // to be visible in the designer.
            if (HorizontalScrollBarShow)
            {
                _horizontalScrollbar.Visible = HorizontalScroll.Visible;                
            }

            // Only attempt to set visibility if configured
            // to be visible in the designer.
            if (VerticalScrollBarShow)
            {
                _verticalScrollbar.Visible = VerticalScroll.Visible;                
            }

            // Update vertical scroll
            if (VerticalScroll.Visible)
            {
                _verticalScrollbar.Minimum = VerticalScroll.Minimum;
                _verticalScrollbar.Maximum = VerticalScroll.Maximum;
                _verticalScrollbar.SmallChange = VerticalScroll.SmallChange;
                _verticalScrollbar.LargeChange = VerticalScroll.LargeChange;
            }

            // Update horizontal scroll
            if (HorizontalScroll.Visible)
            {
                _horizontalScrollbar.Minimum = HorizontalScroll.Minimum;
                _horizontalScrollbar.Maximum = HorizontalScroll.Maximum;
                _horizontalScrollbar.SmallChange = HorizontalScroll.SmallChange;
                _horizontalScrollbar.LargeChange = HorizontalScroll.LargeChange;
            }

            base.OnPaint(e);
       
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                default:
                    if (!DesignMode)
                    {
                        WinApi.ShowScrollBar(Handle, (int)WinApi.ScrollBar.SB_BOTH, 0);                        
                    }
                    base.WndProc(ref m);
                    break;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            _verticalScrollbar.Value = VerticalScroll.Value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _verticalScrollbar.Focus();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// 
        /// </summary>
        private void InitializeComponent()
        {
            // Add scrollbars
            Controls.Add(_verticalScrollbar);
            Controls.Add(_horizontalScrollbar);

            // Scroll events
            _verticalScrollbar.Scroll += VerticalScrollbarScroll;
            _horizontalScrollbar.Scroll += HorizontalScrollbarScroll;

            // Paint scrollbars
            UpdateScrollBarPositions();
        }
        /// <summary>
        /// 
        /// </summary>
        private void UpdateScrollBarPositions()
        {
            if (DesignMode)
            {
                return;
            }
            if (!AutoScroll)
            {
                _verticalScrollbar.Visible = false;
                _horizontalScrollbar.Visible = false;
                return;
            }
            if (VerticalScrollBarShow)
            {
                if (VerticalScroll.Visible || DesignMode)
                {
                    _verticalScrollbar.Location = new Point(ClientRectangle.X + ClientRectangle.Width - _verticalScrollbar.Width, ClientRectangle.Top);
                    _verticalScrollbar.Height = Height;
                }
            }
            else
            {
                _verticalScrollbar.Visible = false;
            }
            if (HorizontalScrollBarShow)
            {
                if (HorizontalScroll.Visible || DesignMode)
                {
                    _horizontalScrollbar.Location = new Point(ClientRectangle.X, ClientRectangle.Top + (Height - _horizontalScrollbar.Height));
                    _horizontalScrollbar.Width = Width;
                }
            }
            else
            {
                _horizontalScrollbar.Visible = false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventArgs"></param>
        private void UpdateScrollBarsPositionWhileScrolling(ScrollEventArgs eventArgs)
        {
            switch (eventArgs.ScrollOrientation)
            {
                case ScrollOrientation.VerticalScroll:
                    switch (eventArgs.Type)
                    {
                        default:
                            if (HorizontalScrollBarShow)
                            {
                                _horizontalScrollbar.Location = new Point(ClientRectangle.X, Height - _horizontalScrollbar.Height);                                
                            }
                            break;
                    }
                    break;
                case ScrollOrientation.HorizontalScroll:
                    switch (eventArgs.Type)
                    {
                        default:
                            if (VerticalScrollBarShow)
                            {
                                _verticalScrollbar.Location = new Point(ClientRectangle.X + ClientRectangle.Width - _verticalScrollbar.Width, ClientRectangle.Top);                                
                            }
                            break;
                    }
                    break;
            }
        }
        #endregion
    }
    #endregion

    #region MetroTabPageCollection

    [ToolboxItem(false), Editor(typeof (MetroTabPageCollectionEditor), typeof (UITypeEditor))]
    public class MetroTabPageCollection : TabControl.TabPageCollection
    {
        public MetroTabPageCollection(TabControl owner) : base(owner)
        {
        }
    }
    #endregion

    /// <summary>
    /// References:
    /// http://bytes.com/topic/c-sharp/answers/576709-adding-custom-tabpages-design-time
    /// http://dotnetrix.co.uk/tabcontrol.htm
    /// </summary>
    [Designer(typeof (MetroTabControlDesigner))]
    public class MetroTabControl : TabControl, IMetroControl
    {
        #region Variables

        /// <summary>
        /// 
        /// </summary>
        private const int TabBottomBorderHeight = 3;
        #endregion

        #region IMetroControl

        private MetroColorStyle _metroStyle = MetroColorStyle.Blue;

        [Category("Metro Appearance")]
        public MetroColorStyle Style
        {
            get
            {
                return StyleManager != null ? StyleManager.Style : _metroStyle;
            }
            set
            {
                _metroStyle = value;
            }
        }

        private MetroThemeStyle _metroTheme = MetroThemeStyle.Light;

        [Category("Metro Appearance")]
        public MetroThemeStyle Theme
        {
            get
            {
                return StyleManager != null ? StyleManager.Theme : _metroTheme;
            }
            set
            {
                _metroTheme = value;
            }
        }

        [Browsable(false)]
        public MetroStyleManager StyleManager { get; set; }

        #endregion

        #region Fields
 
        private bool _useStyleColors;

        [Category("Metro Appearance")]
        public bool UseStyleColors
        {
            get
            {
                return _useStyleColors;
            }
            set
            {
                _useStyleColors = value;
            }
        }

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

        private ContentAlignment _textAlign = ContentAlignment.MiddleLeft;

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

        [Editor(typeof (MetroTabPageCollectionEditor), typeof (UITypeEditor))]
        public new TabPageCollection TabPages
        {
            get
            {
                return base.TabPages;
            }
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
            get
            {
                return _mirror;
            }
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
                graphics.FillRectangle(bgBrush, -2 + GetTabRect(0).X + DisplayRectangle.X, GetTabRect(index).Bottom,
                                       Width - (Width - DisplayRectangle.Width + DisplayRectangle.X) + 4,
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
                    X = -2 + selectedTabRect.X + DisplayRectangle.X,
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Invalidate();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnParentBackColorChanged(EventArgs e)
        {
            base.OnParentBackColorChanged(e);
            Invalidate();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
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
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                    // Hide both scrollbars by default
                default:
                    WinApi.ShowScrollBar(Handle, (int) WinApi.ScrollBar.SB_BOTH, 0);
                    base.WndProc(ref m);
                    break;
            }
        }

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