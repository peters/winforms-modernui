using System;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using MetroFramework.Components;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;
using MetroFramework.Native;

namespace MetroFramework.Forms
{
    public class MetroForm : Form, IMetroForm, IMessageFilter
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

            TextRenderer.DrawText(e.Graphics, Text, MetroFonts.Title, new Point(20, 20), foreColor);
        }

        #endregion

        #region Management Methods

        public bool FocusMe()
        {
            return WinApi.SetForegroundWindow(Handle);
        }

        private bool isInitialized = false;

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (!isInitialized)
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

                isInitialized = true;
            }

            if (!DesignMode)
                DwmApi.DwmExtendFrameIntoClientArea(Handle, ref dwmMargins);
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

                int dwmHandled = DwmApi.DwmDefWindowProc(m.HWnd, m.Msg, m.WParam, m.LParam, ref result);
                if (dwmHandled == 1)
                {
                    m.Result = result;
                    return;
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

            if (e.Button == System.Windows.Forms.MouseButtons.Left && Movable)
            {
                if (this.Width - borderWidth > e.Location.X && e.Location.X > borderWidth && e.Location.Y > borderWidth)
                    MoveControl(Handle);
            }
        }

        private void MoveControl(IntPtr hWnd)
        {
            WinApi.ReleaseCapture();
            WinApi.SendMessage(hWnd, (int)WinApi.Messages.WM_NCLBUTTONDOWN, (int)WinApi.HitTest.HTCAPTION, 0);
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
            MetroFormButton btn = sender as MetroFormButton;
            if (btn != null)
            {
                WindowButtons btnFlag = (WindowButtons)btn.Tag;
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
            foreach (KeyValuePair<WindowButtons, MetroFormButton> val in windowButtonList)
            {
                if (val.Key == WindowButtons.Close)
                {
                    val.Value.Location = new Point(Width - 40, 8);
                }
                else if (val.Key == WindowButtons.Minimize)
                {
                    val.Value.Location = new Point(Width - 80, 8);
                }
                else if (val.Key == WindowButtons.Maximize)
                {
                    val.Value.Location = new Point(Width - 60, 8);
                }
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
        
        #region IMessageFilter
        public bool PreFilterMessage(ref Message m)
        {
            switch (m.Msg)
            {
                // Double clicking in the top area should result in either form maximize or 
                // returning back to normal size.
                case (int)WinApi.Messages.WM_LBUTTONDBLCLK:

                    // Get handle for current control clicked.
                    Control target = FromHandle(m.HWnd);

                    // Only valid when target is main form itself.
                    if (target != null && target.Handle != Handle)
                    {
                        return false;
                    }

                    // Mouse x/y coords.
                    var click = new Point((Int16)m.LParam, (Int16)((int)m.LParam >> 16));

                    // Handle window state change if within bounds of click,
                    // which is a hardcoded limit of 50 pixels top down.
                    if (click.Y <= 50)
                    {
                        WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
                    }

                    break;
            }
            return false;
        }
        #endregion

    }
}
