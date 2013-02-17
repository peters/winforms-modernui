using System;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using MetroFramework.Components;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;
using MetroFramework.Native;

namespace MetroFramework.Forms
{

    #region Enums
    public enum TextAlign
    {
        Left,
        Center,
        Right
    }
    #endregion
   
    public class MetroForm : Form, IMetroForm
    {

        #region Variables
        private const int FirstButtonSpacerWidth = 40;
        private const int TopBottomMinMaximizeHitboxRange = 50;
        private bool _isMouseXyWithinTopHeaderArea;
        private bool _isInitialized;
        private readonly bool _isVistaOrHigher = IsWinVistaOrHigher();
        #endregion

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

        private TextAlign _textAlign = TextAlign.Left;

        [Browsable(true)]
        [Category("Appearance")]
        public TextAlign TextAlign
        {
            get { return _textAlign; }
            set { _textAlign = value; }
        }

        [Browsable(false)]
        public override Color BackColor
        {
            get
            {
                return MetroPaint.BackColor.Form(Theme);
            }
        }

        [Browsable(false)]
        public new FormBorderStyle FormBorderStyle
        {
            get
            {
                if (DesignMode)
                {
                    base.FormBorderStyle = FormBorderStyle.None;
                }

                return base.FormBorderStyle;
            }
            set
            {
                if (DesignMode)
                {
                    base.FormBorderStyle = FormBorderStyle.None;
                    return;
                }

                base.FormBorderStyle = value;
            }
        }

        private bool isMovable = true;
        public bool Movable
        {
            get { return isMovable; }
            set { isMovable = value; }
        }

        private DwmApi.MARGINS dwmMargins;
        private bool isMarginOk;

        private bool isAeroEnabled;
        public bool AeroEnabled
        {
            get { return isAeroEnabled; }
        }

        private int borderWidth = 5;

        #endregion

        #region Constructor

        public MetroForm()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);

            Padding = new Padding(20, 60, 20, 20);

            if (!_isVistaOrHigher)
            {
                RemoveCloseButton(this);
                FormBorderStyle = FormBorderStyle.None;
            }

        }

        #endregion

        #region Paint Methods
        protected override void OnPaint(PaintEventArgs e)
        {
            Color backColor = MetroPaint.BackColor.Form(Theme);
            Color foreColor = MetroPaint.ForeColor.Title(Theme);

            e.Graphics.Clear(backColor);

            using (SolidBrush b = MetroPaint.GetStyleBrush(Style))
            {
                Rectangle topRect = new Rectangle(0, 0, Width, 5);
                e.Graphics.FillRectangle(b, topRect);
            }

            switch (TextAlign)
            {
                case TextAlign.Left:                    
                    TextRenderer.DrawText(e.Graphics, Text, MetroFonts.Title, new Point(20, 20), foreColor);
                    break;
                case TextAlign.Center:
                    TextRenderer.DrawText(e.Graphics, Text, MetroFonts.Title, new Point(ClientRectangle.Width, 20), foreColor, 
                        TextFormatFlags.EndEllipsis | TextFormatFlags.HorizontalCenter);
                    break;
                case TextAlign.Right:
                    var actualSize = MeasureText(e.Graphics, ClientRectangle, MetroFonts.Title, Text,
                                                 TextFormatFlags.RightToLeft);
                    TextRenderer.DrawText(e.Graphics, Text, MetroFonts.Title, new Point(ClientRectangle.Width - actualSize.Width, 20), foreColor,
                        TextFormatFlags.RightToLeft);
                    break;
            }
        }

        #endregion

        #region Management Methods

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            if (!_isVistaOrHigher && _isInitialized)
            {
                Refresh();
            }
        }

        public bool FocusMe()
        {
            return WinApi.SetForegroundWindow(Handle);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (!_isInitialized)
            {
                if (ControlBox)
                {
                    AddWindowButton(WindowButtons.Close);

                    if (MaximizeBox)
                        AddWindowButton(WindowButtons.Maximize);

                    if (MinimizeBox)
                        AddWindowButton(WindowButtons.Minimize);

                    UpdateWindowButtonPosition();
                }

                _isInitialized = true;
            }

            if (DesignMode) return;

            if (_isVistaOrHigher)
            {
                DwmApi.DwmExtendFrameIntoClientArea(Handle, ref dwmMargins);
            }
            else
            {
                Refresh();
            }

        }

        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            UpdateWindowButtonPosition();
        }

        #endregion

        #region DWM Methods

        protected override void WndProc(ref Message m)
        {
            if (!DesignMode)
            {
                IntPtr result = default(IntPtr);

                if (_isVistaOrHigher)
                {
                    int dwmHandled = DwmApi.DwmDefWindowProc(m.HWnd, m.Msg, m.WParam, m.LParam, ref result);
                    if (dwmHandled == 1)
                    {
                        m.Result = result;
                        return;
                    }
                }
                else if (m.Msg == (int)WinApi.Messages.WM_NCPAINT || m.Msg == (int)WinApi.Messages.WM_SIZING || m.Msg == (int)WinApi.Messages.WM_NCACTIVATE)
                {
                    using (var graphics = Graphics.FromHwnd(m.HWnd))
                    {
                        using (SolidBrush b = MetroPaint.GetStyleBrush(Style))
                        {
                            graphics.FillRectangle(b, new Rectangle(0, 0, Width, 5));
                        }
                    }
                }

                if (m.Msg == (int)WinApi.Messages.WM_NCCALCSIZE && (int)m.WParam == 1)
                {
                    WinApi.NCCALCSIZE_PARAMS nccsp = (WinApi.NCCALCSIZE_PARAMS)Marshal.PtrToStructure(m.LParam, typeof(WinApi.NCCALCSIZE_PARAMS));

                    nccsp.rect0.Top += 0;
                    nccsp.rect0.Bottom += 0;
                    nccsp.rect0.Left += 0;
                    nccsp.rect0.Right += 0;

                    if (!isMarginOk)
                    {
                        dwmMargins.cyTopHeight = 0;
                        dwmMargins.cxLeftWidth = borderWidth;
                        dwmMargins.cyBottomHeight = borderWidth;
                        dwmMargins.cxRightWidth = borderWidth;
                        isMarginOk = true;
                    }

                    Marshal.StructureToPtr(nccsp, m.LParam, false);

                    m.Result = IntPtr.Zero;
                } else if (m.Msg == (int) WinApi.Messages.WM_LBUTTONDBLCLK)
                {
                    // Alow the form to be normalized / maximized when
                    // clicked inside our top header area rectangle.
                    if (_isMouseXyWithinTopHeaderArea)
                    {
                        WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
                        _isMouseXyWithinTopHeaderArea = false;
                    }
                }
                else if (m.Msg == (int)WinApi.Messages.WM_NCHITTEST && (int)m.Result == 0)
                {
                    m.Result = HitTestNCA(m.HWnd, m.WParam, m.LParam);
                }
                else
                {
                    base.WndProc(ref m);
                }
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        private IntPtr HitTestNCA(IntPtr hwnd, IntPtr wparam, IntPtr lparam)
        {
            Rectangle testRect = Rectangle.Empty;

            Point p = new Point((Int16)lparam, (Int16)((int)lparam >> 16));


            // Determine if mouse xy is within our range of the "top header" area 
            // which allows for double clicking for either minimize/maximizing form.
            testRect = RectangleToScreen(new Rectangle(0, 0, Width - FirstButtonSpacerWidth, TopBottomMinMaximizeHitboxRange));
            _isMouseXyWithinTopHeaderArea = testRect.Contains(p);
            
            testRect = RectangleToScreen(new Rectangle(0, 0, dwmMargins.cxLeftWidth, dwmMargins.cxLeftWidth));
            if (testRect.Contains(p))
                return new IntPtr((int)WinApi.HitTest.HTTOPLEFT);

            testRect = RectangleToScreen(new Rectangle(Width - dwmMargins.cxRightWidth, 0, dwmMargins.cxRightWidth, dwmMargins.cxRightWidth));
            if (testRect.Contains(p))
                return new IntPtr((int)WinApi.HitTest.HTTOPRIGHT);

            testRect = RectangleToScreen(new Rectangle(0, Height - dwmMargins.cyBottomHeight, dwmMargins.cxLeftWidth, dwmMargins.cyBottomHeight));
            if (testRect.Contains(p))
                return new IntPtr((int)WinApi.HitTest.HTBOTTOMLEFT);

            testRect = RectangleToScreen(new Rectangle(Width - dwmMargins.cxRightWidth, Height - dwmMargins.cyBottomHeight, dwmMargins.cxRightWidth, dwmMargins.cyBottomHeight));
            if (testRect.Contains(p))
                return new IntPtr((int)WinApi.HitTest.HTBOTTOMRIGHT);

            testRect = RectangleToScreen(new Rectangle(0, 0, Width, dwmMargins.cxLeftWidth));
            if (testRect.Contains(p))
                return new IntPtr((int)WinApi.HitTest.HTTOP);

            testRect = RectangleToScreen(new Rectangle(0, dwmMargins.cxLeftWidth, Width, dwmMargins.cyTopHeight - dwmMargins.cxLeftWidth));
            if (testRect.Contains(p))
                return new IntPtr((int)WinApi.HitTest.HTCAPTION);

            testRect = RectangleToScreen(new Rectangle(0, 0, dwmMargins.cxLeftWidth, Height));
            if (testRect.Contains(p))
                return new IntPtr((int)WinApi.HitTest.HTLEFT);

            testRect = RectangleToScreen(new Rectangle(Width - dwmMargins.cxRightWidth, 0, dwmMargins.cxRightWidth, Height));
            if (testRect.Contains(p))
                return new IntPtr((int)WinApi.HitTest.HTRIGHT);

            testRect = RectangleToScreen(new Rectangle(0, Height - dwmMargins.cyBottomHeight, Width, dwmMargins.cyBottomHeight));
            if (testRect.Contains(p))
                return new IntPtr((int)WinApi.HitTest.HTBOTTOM);

            return new IntPtr((int)WinApi.HitTest.HTCLIENT);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left && Movable)
            {
                if (WindowState == FormWindowState.Maximized) return;
                if (Width - borderWidth > e.Location.X && e.Location.X > borderWidth && e.Location.Y > borderWidth)
                {
                    MoveControl(Handle);                    
                }
            }
        }

        private void MoveControl(IntPtr hWnd)
        {
            WinApi.ReleaseCapture();
            WinApi.SendMessage(hWnd, (int)WinApi.Messages.WM_NCLBUTTONDOWN, (int)WinApi.HitTest.HTCAPTION, 0);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Invalidate();
        }

        #endregion

        #region Window Buttons

        private enum WindowButtons
        {
            Minimize,
            Maximize,
            Close
        }

        private Dictionary<WindowButtons, MetroFormButton> windowButtonList;

        private void AddWindowButton(WindowButtons button)
        {
            if (windowButtonList == null)
                windowButtonList = new Dictionary<WindowButtons, MetroFormButton>();

            if (windowButtonList.ContainsKey(button))
                return;

            MetroFormButton newButton = new MetroFormButton();

            if (button == WindowButtons.Close)
            {
                newButton.Text = "r";
            }
            else if (button == WindowButtons.Minimize)
            {
                newButton.Text = "0";
            }
            else if (button == WindowButtons.Maximize)
            {
                if (WindowState == FormWindowState.Normal)
                    newButton.Text = "1";
                else
                    newButton.Text = "2";
            }

            newButton.Tag = button;
            newButton.Size = new Size(30, 20);
            newButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            newButton.Click += new EventHandler(WindowButton_Click);
            Controls.Add(newButton);

            windowButtonList.Add(button, newButton);
        }

        private void WindowButton_Click(object sender, EventArgs e)
        {
            var btn = sender as MetroFormButton;
            if (btn != null)
            {
                var btnFlag = (WindowButtons)btn.Tag;
                if (btnFlag == WindowButtons.Close)
                {
                    Close();
                }
                else if (btnFlag == WindowButtons.Minimize)
                {
                    WindowState = FormWindowState.Minimized;
                }
                else if (btnFlag == WindowButtons.Maximize)
                {
                    if (WindowState == FormWindowState.Normal)
                    {
                        WindowState = FormWindowState.Maximized;
                        btn.Text = "2";
                    }
                    else
                    {
                        WindowState = FormWindowState.Normal;
                        btn.Text = "1";
                    }
                }
            }
        }

        private void UpdateWindowButtonPosition()
        {

            // Do not update when controlbox, since there are
            // no buttons to display.
            if (!ControlBox)
            {
                return;
            }

            // Button drawing priority.
            var priorityOrder = new Dictionary<int, WindowButtons>(3)
                                {
                                    {0, WindowButtons.Close},
                                    {1, WindowButtons.Maximize},
                                    {2, WindowButtons.Minimize}
                                };

            // Position of the first button drawn
            var firstButtonLocation = new Point(Width - FirstButtonSpacerWidth, 8);

            // Number of buttons drawn in total
            var lastDrawedButtonPosition = firstButtonLocation.X - 20;

            // Object representation of the first button drawn
            MetroFormButton firstButton = null;

            // Only one button to draw.
            if (windowButtonList.Count == 1)
            {
                foreach (var button in windowButtonList)
                {
                    button.Value.Location = firstButtonLocation;
                    break;
                }
                return;
            }

            // Draw buttons in priority order
            foreach (var button in priorityOrder)
            {

                var buttonExists = windowButtonList.ContainsKey(button.Value);

                if (firstButton == null && buttonExists)
                {
                    firstButton = windowButtonList[button.Value];
                    firstButton.Location = firstButtonLocation;
                    continue;
                }

                if (firstButton == null || !buttonExists) continue;

                windowButtonList[button.Value].Location = new Point(lastDrawedButtonPosition, 8);
                lastDrawedButtonPosition = lastDrawedButtonPosition - 20;
            }

            Refresh();
        }

        private class MetroFormButton : Button, IMetroControl
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

            private bool isHovered = false;
            private bool isPressed = false;

            #endregion

            #region Constructor

            public MetroFormButton()
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

                if (Parent != null)
                    backColor = Parent.BackColor;
                else
                    backColor = MetroPaint.BackColor.Form(Theme);

                if (isHovered && !isPressed && Enabled)
                {
                    foreColor = MetroPaint.ForeColor.Link.Hover(Theme);
                }
                else if (isHovered && isPressed && Enabled)
                {
                    foreColor = MetroPaint.ForeColor.Link.Press(Theme);
                }
                else if (!Enabled)
                {
                    foreColor = MetroPaint.ForeColor.Link.Disabled(Theme);
                }
                else
                {
                    foreColor = MetroPaint.ForeColor.Link.Normal(Theme);
                }

                e.Graphics.Clear(backColor);
                Font buttonFont = new Font("Webdings", 9.25f);
                TextRenderer.DrawText(e.Graphics, Text, buttonFont, ClientRectangle, foreColor, backColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
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
        }

        #endregion

        #region Helper methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static bool IsWinVistaOrHigher()
        {
            var os = Environment.OSVersion;
            return (os.Platform == PlatformID.Win32NT) && (os.Version.Major >= 6);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="frm"></param>
        public static void RemoveCloseButton(Form frm)
        {
            IntPtr hMenu = WinApi.GetSystemMenu(frm.Handle, false);
            if (hMenu == IntPtr.Zero)
            {
                return;
            }
            int n = WinApi.GetMenuItemCount(hMenu);
            if (n <= 0)
            {
                return;
            }
            WinApi.RemoveMenu(hMenu, (uint)(n - 1), WinApi.MfByposition | WinApi.MfRemove);
            WinApi.RemoveMenu(hMenu, (uint)(n - 2), WinApi.MfByposition | WinApi.MfRemove);
            WinApi.DrawMenuBar(frm.Handle);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="clientRectangle"></param>
        /// <param name="font"></param>
        /// <param name="text"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        private static Rectangle MeasureText(Graphics graphics, Rectangle clientRectangle, Font font, string text, TextFormatFlags flags)
        {
            var proposedSize = new Size(int.MaxValue, int.MinValue);
            var actualSize = TextRenderer.MeasureText(graphics, text, font, proposedSize, flags);
            return new Rectangle(clientRectangle.X, clientRectangle.Y, actualSize.Width, actualSize.Height);
        }
        #endregion

    }
}