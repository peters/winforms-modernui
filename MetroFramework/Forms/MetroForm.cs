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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.ComponentModel;
using System.Collections.Generic;
using System.Security;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using MetroFramework.Components;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;
using MetroFramework.Native;

namespace MetroFramework.Forms
{

    public enum TextAlign
    {
        Left,
        Center,
        Right
    }

    public enum ShadowType
    {
        None,
        Flat,
        DropShadow
    }
   
    public class MetroForm : Form, IMetroForm, IDisposable
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

        protected MetroFlatDropShadow metroFlatShadowForm = null;
        protected MetroRealisticDropShadow metroRealisticShadowForm = null;

        private bool isInitialized = false;

        private TextAlign textAlign = TextAlign.Left;
        [Browsable(true)]
        [Category("Metro Appearance")]
        public TextAlign TextAlign
        {
            get { return textAlign; }
            set { textAlign = value; }
        }

        [Browsable(false)]
        public override Color BackColor
        {
            get { return MetroPaint.BackColor.Form(Theme); }
        }

        [DefaultValue(FormBorderStyle.None)]
        [Browsable(false)]
        public new FormBorderStyle FormBorderStyle
        {
            get 
            {
                return FormBorderStyle.None;
            }
            set
            {
                base.FormBorderStyle = FormBorderStyle.None;
            }
        }

        private bool isMovable = true;
        [Category("Metro Appearance")]
        public bool Movable
        {
            get { return isMovable; }
            set { isMovable = value; }
        }

        public new Padding Padding
        {
            get { return base.Padding; }
            set
            {
                if (!DisplayHeader)
                {
                    if (value.Top < 30)
                    {
                        value.Top = 30;
                    }
                }
                else
                {
                    if (value.Top < 60)
                    {
                        value.Top = 60;
                    }
                }

                base.Padding = value;
            }
        }

        private bool displayHeader = true;
        [Category("Metro Appearance")]
        public bool DisplayHeader
        {
            get { return displayHeader; }
            set 
            { 
                displayHeader = value;

                if (displayHeader)
                    Padding = new Padding(20, 60, 20, 20);
                else
                    Padding = new Padding(20, 30, 20, 20);

                Invalidate();
            }
        }

        private bool isResizable = true;
        [Category("Metro Appearance")]
        public bool Resizable
        {
            get { return isResizable; }
            set { isResizable = value; }
        }

        private ShadowType shadowType = ShadowType.Flat;
        [Category("Metro Appearance")]
        public ShadowType ShadowType
        {
            get { return shadowType; }
            set { shadowType = value; }
        }

        private int borderWidth = 5;

        public new FormWindowState WindowState
        {
            get { return base.WindowState; }
            set
            {
                if (value == FormWindowState.Maximized)
                {
                    HandleMaximizeWindow();
                    return;
                }

                base.WindowState = value;
            }
        }

        #endregion

        #region Constructor

        public MetroForm()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);

            Name = "MetroForm";
            Padding = new Padding(20, 60, 20, 20);
            StartPosition = FormStartPosition.CenterScreen;

            RemoveCloseButton();
            FormBorderStyle = FormBorderStyle.None;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                RemoveShadow();
            }

            base.Dispose(disposing);
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
                Rectangle topRect = new Rectangle(0, 0, Width, borderWidth);
                e.Graphics.FillRectangle(b, topRect);
            }

            if (displayHeader)
            {
                // Assuming padding 20px on left/right; 20px from top; max 40px height
                Rectangle bounds = new Rectangle(20, 20, ClientRectangle.Width - 2*20, 40);
                TextFormatFlags flags = TextFormatFlags.EndEllipsis | GetTextFormatFlags();
                TextRenderer.DrawText(e.Graphics, Text, MetroFonts.Title, bounds, foreColor, flags);
            }

            if (Resizable && (SizeGripStyle == SizeGripStyle.Auto || SizeGripStyle == SizeGripStyle.Show))
            {
                using (SolidBrush b = new SolidBrush(MetroPaint.ForeColor.Button.Disabled(Theme)))
                {
                    Size resizeHandleSize = new Size(2, 2);
                    e.Graphics.FillRectangles(b, new Rectangle[] {
                        new Rectangle(new Point(ClientRectangle.Width-6,ClientRectangle.Height-6), resizeHandleSize),
                        new Rectangle(new Point(ClientRectangle.Width-10,ClientRectangle.Height-10), resizeHandleSize),
                        new Rectangle(new Point(ClientRectangle.Width-10,ClientRectangle.Height-6), resizeHandleSize),
                        new Rectangle(new Point(ClientRectangle.Width-6,ClientRectangle.Height-10), resizeHandleSize),
                        new Rectangle(new Point(ClientRectangle.Width-14,ClientRectangle.Height-6), resizeHandleSize),
                        new Rectangle(new Point(ClientRectangle.Width-6,ClientRectangle.Height-14), resizeHandleSize)
                    });
                }
            }
        }

        private TextFormatFlags GetTextFormatFlags()
        {
            switch (TextAlign)
            {
                case TextAlign.Left: return TextFormatFlags.Left;
                case TextAlign.Center: return TextFormatFlags.HorizontalCenter;
                case TextAlign.Right: return TextFormatFlags.Right;
            }
            throw new InvalidOperationException();
        }

        #endregion

        #region Management Methods

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!(this is MetroTaskWindow))
                MetroTaskWindow.ForceClose();

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            RemoveShadow();

            base.OnClosed(e);
        }

        private void RemoveShadow()
        {
            if (metroFlatShadowForm != null)
            {
                if (!metroFlatShadowForm.IsDisposed)
                {
                    metroFlatShadowForm.Visible = false;
                    Owner = metroFlatShadowForm.Owner;
                    metroFlatShadowForm.Owner = null;
                    metroFlatShadowForm.Dispose();
                    metroFlatShadowForm = null;
                }
            }
            if (metroRealisticShadowForm != null)
            {
                if (!metroRealisticShadowForm.IsDisposed)
                {
                    metroRealisticShadowForm.Visible = false;
                    Owner = metroRealisticShadowForm.Owner;
                    metroRealisticShadowForm.Owner = null;
                    metroRealisticShadowForm.Dispose();
                    metroRealisticShadowForm = null;
                }
            }
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            if (isInitialized)
            {
                Refresh();
            }
        }

        [SecuritySafeCritical]
        public bool FocusMe()
        {
            return WinApi.SetForegroundWindow(Handle);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!DesignMode)
            {
                switch (StartPosition)
                {
                    case FormStartPosition.CenterParent:
                        CenterToParent();
                        break;
                    case FormStartPosition.CenterScreen:
                        CenterToScreen();
                        break;
                }
            }

            if (metroFlatShadowForm == null && !DesignMode && shadowType == ShadowType.Flat)
            {
                metroFlatShadowForm = new MetroFlatDropShadow(this);
            }
            if (metroRealisticShadowForm == null && !DesignMode && shadowType == ShadowType.DropShadow)
            {
                metroRealisticShadowForm = new MetroRealisticDropShadow(this);
            }
        }

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

            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }

            if (DesignMode) return;

            Refresh();
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            UpdateWindowButtonPosition();
        }

        //private const int CS_DROPSHADOW = 0x20000;
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams cp = base.CreateParams;
        //        cp.ClassStyle |= CS_DROPSHADOW;
        //        return cp;
        //    }
        //}

        protected override void WndProc(ref Message m)
        {
            if (MaximizeBox == true)
            {
                if (!WndProc_Movable(m))
                {
                    return;
                }

                if (m.Msg == (int)WinApi.Messages.WM_SYSCOMMAND)
                {
                    if ((m.WParam.ToInt32() & 0xFFF0) == (int)WinApi.Messages.SC_MAXIMIZE)
                    {
                        WindowState = FormWindowState.Maximized;
                        return;
                    }
                    else if ((m.WParam.ToInt32() & 0xFFF0) == (int)WinApi.Messages.SC_RESTORE)
                    {
                        return;
                    }
                }

                base.WndProc(ref m);
            }

            if (!DesignMode)
            {
                if (MaximizeBox == false)
                {
                    if (m.Msg == (int)WinApi.Messages.WM_LBUTTONDBLCLK)
                    {
                        return;
                    }
                }
                if (!WndProc_Movable(m))
                {
                    return;
                }
                if (m.Msg == (int)WinApi.Messages.WM_NCHITTEST)
                {
                    m.Result = HitTestNCA(m.HWnd, m.WParam, m.LParam);
                }
            }

            if (MaximizeBox == false)
            {
                base.WndProc(ref m);
            }
        }

        private bool WndProc_Movable(Message m)
        {
            if (Movable) return true;

            if (m.Msg == (int)WinApi.Messages.WM_SYSCOMMAND)
            {
                int moveCommand = m.WParam.ToInt32() & 0xfff0;
                if (moveCommand == (int)WinApi.Messages.SC_MOVE)
                {
                    return false;
                }
            }

            return true;
        }

        private IntPtr HitTestNCA(IntPtr hwnd, IntPtr wparam, IntPtr lparam)
        {
            //Point vPoint = PointToClient(new Point((int)lparam & 0xFFFF, (int)lparam >> 16 & 0xFFFF));
            //Point vPoint = PointToClient(new Point((Int16)lparam, (Int16)((int)lparam >> 16)));
            Point vPoint = new Point((Int16)lparam, (Int16)((int)lparam >> 16));
            int vPadding = Math.Max(Padding.Right, Padding.Bottom);

            if (Resizable)
            {
                if (RectangleToScreen(new Rectangle(ClientRectangle.Width - vPadding, ClientRectangle.Height - vPadding, vPadding, vPadding)).Contains(vPoint))
                    return (IntPtr)WinApi.HitTest.HTBOTTOMRIGHT;
            }

            if (RectangleToScreen(new Rectangle(5, 5, ClientRectangle.Width - 10, 50)).Contains(vPoint))
                return (IntPtr)WinApi.HitTest.HTCAPTION;

            return (IntPtr)WinApi.HitTest.HTCLIENT;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left && Movable)
            {
                if (WindowState == FormWindowState.Maximized) return;
                if (Width - borderWidth > e.Location.X && e.Location.X > borderWidth && e.Location.Y > borderWidth)
                {
                    MoveControl();                    
                }
            }
        }

        [SecuritySafeCritical]
        private void MoveControl()
        {
            WinApi.ReleaseCapture();
            WinApi.SendMessage(Handle, (int)WinApi.Messages.WM_NCLBUTTONDOWN, (int)WinApi.HitTest.HTCAPTION, 0);
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
            newButton.Size = new Size(25, 20);
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
                        btn.Text = isMaximized ? "2" : "1";
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
            if (!ControlBox) return;

            Dictionary<int, WindowButtons> priorityOrder = new Dictionary<int, WindowButtons>(3) { {0, WindowButtons.Close}, {1, WindowButtons.Maximize}, {2, WindowButtons.Minimize} };

            Point firstButtonLocation = new Point(ClientRectangle.Width - borderWidth - 25, borderWidth);
            int lastDrawedButtonPosition = firstButtonLocation.X - 25;

            MetroFormButton firstButton = null;

            if (windowButtonList.Count == 1)
            {
                foreach (KeyValuePair<WindowButtons, MetroFormButton> button in windowButtonList)
                {
                    button.Value.Location = firstButtonLocation;
                }
            }
            else
            {
                foreach (KeyValuePair<int, WindowButtons> button in priorityOrder)
                {
                    bool buttonExists = windowButtonList.ContainsKey(button.Value);

                    if (firstButton == null && buttonExists)
                    {
                        firstButton = windowButtonList[button.Value];
                        firstButton.Location = firstButtonLocation;
                        continue;
                    }

                    if (firstButton == null || !buttonExists) continue;

                    windowButtonList[button.Value].Location = new Point(lastDrawedButtonPosition, borderWidth);
                    lastDrawedButtonPosition = lastDrawedButtonPosition - 25;
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
                {
                    if (Parent is IMetroForm)
                    {
                        backColor = MetroPaint.BackColor.Form(Theme);
                    }
                    else if (Parent is IMetroControl)
                    {
                        backColor = MetroPaint.GetStyleColor(Style);
                    }
                    else
                    {
                        backColor = Parent.BackColor;
                    }
                }
                else
                {
                    backColor = MetroPaint.BackColor.Form(Theme);
                }

                if (isHovered && !isPressed && Enabled)
                {
                    foreColor = MetroPaint.ForeColor.Button.Normal(Theme);
                    backColor = MetroPaint.BackColor.Button.Normal(Theme);
                }
                else if (isHovered && isPressed && Enabled)
                {
                    foreColor = MetroPaint.ForeColor.Button.Press(Theme);
                    backColor = MetroPaint.GetStyleColor(Style);
                }
                else if (!Enabled)
                {
                    foreColor = MetroPaint.ForeColor.Button.Disabled(Theme);
                    backColor = MetroPaint.BackColor.Button.Disabled(Theme);
                }
                else
                {
                    foreColor = MetroPaint.ForeColor.Button.Normal(Theme);
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

        #region Flat DropShadow Form

        protected class MetroFlatDropShadow : Form
        {
            private Form shadowTargetForm;
            private Point Offset = new Point(-6, -6);
            private bool isBringingToFront;
            private Bitmap getShadow;
            private Timer timer = new Timer();
            private long lastResizedOn = 0;

            [SecuritySafeCritical]
            public MetroFlatDropShadow(Form parentForm)
            {
                shadowTargetForm = parentForm;

                FormBorderStyle = FormBorderStyle.None;
                ShowInTaskbar = false;

                uint wl = WinApi.GetWindowLong(this.Handle, -20);
                wl = wl | 0x80000 | 0x20;
                WinApi.SetWindowLong(this.Handle, -20, wl);

                StartPosition = parentForm.StartPosition;

                parentForm.Activated += shadowTargetForm_Activated; 
                Deactivate += MetroRealisticDropShadow_Deactivated;
                parentForm.Move += shadowTargetForm_Move;
                parentForm.Resize += shadowTargetForm_Resize;
                parentForm.ResizeEnd += shadowTargetForm_ResizeEnd;
                parentForm.VisibleChanged += shadowTargetForm_VisibleChanged;

                if (parentForm.Owner != null)
                    Owner = parentForm.Owner;

                parentForm.Owner = this;

                Bounds = GetBounds();

                Load += MetroRealisticDropShadow_Load;
            }

            private void MetroRealisticDropShadow_Load(object sendr, EventArgs e)
            {
                timer.Interval = 50;
                timer.Tick += timer_Tick;
                timer.Start();
            }

            private void timer_Tick(object sendr, EventArgs e)
            {
                timer.Tick -= timer_Tick;
                timer.Stop();
                getShadow = DrawBlurBorder();
                SetBitmap(getShadow, 255);
            }

            private Rectangle GetBounds()
            {
                return new Rectangle((shadowTargetForm).Location.X + Offset.X, (shadowTargetForm).Location.Y + Offset.Y, shadowTargetForm.ClientRectangle.Width + Math.Abs(Offset.X * 2), shadowTargetForm.ClientRectangle.Height + Math.Abs(Offset.Y * 2));
            }

            private void shadowTargetForm_VisibleChanged(object sender, EventArgs e)
            {
                Visible = shadowTargetForm.Visible;
            }

            private void shadowTargetForm_Activated(object o, EventArgs e)
            {
                Visible = (shadowTargetForm.WindowState == FormWindowState.Normal);
                if (Visible) Show();

                if (isBringingToFront)
                {
                    isBringingToFront = false;
                    return;
                }

                this.BringToFront();
            }

            private void MetroRealisticDropShadow_Deactivated(object o, EventArgs e)
            {
                isBringingToFront = true;
            }

            private void shadowTargetForm_Move(object o, EventArgs e)
            {
                if (o is Form)
                    this.Bounds = GetBounds();
            }

            private void shadowTargetForm_Resize(object o, EventArgs e)
            {
                if (o is Form)
                    this.Bounds = GetBounds();

                Visible = false;

                long delta = DateTime.Now.Ticks - lastResizedOn;
                if (delta > 100000)
                {
                    lastResizedOn = DateTime.Now.Ticks;
                    Invalidate();
                }
            }

            private void shadowTargetForm_ResizeEnd(object o, EventArgs e)
            {
                Visible = true;

                lastResizedOn = 0;
                Invalidate();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                getShadow = DrawBlurBorder();
                SetBitmap(getShadow, 255);
            }

            [SecuritySafeCritical]
            private void SetBitmap(Bitmap bitmap, byte opacity)
            {
                if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                    throw new ApplicationException("The bitmap must be 32ppp with alpha-channel.");

                IntPtr screenDc = WinApi.GetDC(IntPtr.Zero);
                IntPtr memDc = WinApi.CreateCompatibleDC(screenDc);
                IntPtr hBitmap = IntPtr.Zero;
                IntPtr oldBitmap = IntPtr.Zero;

                try
                {
                    hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
                    oldBitmap = WinApi.SelectObject(memDc, hBitmap);

                    WinApi.Size size = new WinApi.Size(bitmap.Width, bitmap.Height);
                    WinApi.Point pointSource = new WinApi.Point(0, 0);
                    WinApi.Point topPos = new WinApi.Point(Left, Top);
                    WinApi.BLENDFUNCTION blend = new WinApi.BLENDFUNCTION();
                    blend.BlendOp = WinApi.AC_SRC_OVER;
                    blend.BlendFlags = 0;
                    blend.SourceConstantAlpha = opacity;
                    blend.AlphaFormat = WinApi.AC_SRC_ALPHA;

                    WinApi.UpdateLayeredWindow(Handle, screenDc, ref topPos, ref size, memDc, ref pointSource, 0, ref blend, WinApi.ULW_ALPHA);
                }
                finally
                {
                    WinApi.ReleaseDC(IntPtr.Zero, screenDc);
                    if (hBitmap != IntPtr.Zero)
                    {
                        WinApi.SelectObject(memDc, oldBitmap);
                        WinApi.DeleteObject(hBitmap);
                    }
                    WinApi.DeleteDC(memDc);
                }
            }

            private Bitmap DrawBlurBorder()
            {
                return (Bitmap)DrawOutsetShadow(Color.Black, new Rectangle(0, 0, ClientRectangle.Width, ClientRectangle.Height));
            }

            private Image DrawOutsetShadow(Color color, Rectangle shadowCanvasArea)
            {
                Rectangle rOuter = shadowCanvasArea;
                Rectangle rInner = new Rectangle(shadowCanvasArea.X + (-Offset.X - 1), shadowCanvasArea.Y + (-Offset.Y - 1), shadowCanvasArea.Width - (-Offset.X * 2 - 1), shadowCanvasArea.Height - (-Offset.Y * 2 - 1));

                Bitmap img = new Bitmap(rOuter.Width, rOuter.Height, PixelFormat.Format32bppArgb);
                Graphics g = Graphics.FromImage(img);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                using (Brush bgBrush = new SolidBrush(Color.FromArgb(30, Color.Black)))
                {
                    g.FillRectangle(bgBrush, rOuter);
                }
                using (Brush bgBrush = new SolidBrush(Color.FromArgb(60, Color.Black)))
                {
                    g.FillRectangle(bgBrush, rInner);
                }

                g.Flush();
                g.Dispose();

                return img;
            }
        }

        #endregion

        #region Windows7 DropShadow Form

        protected class MetroRealisticDropShadow : Form
        {
            private Form shadowTargetForm;
            private Point Offset = new Point(-15, -15);
            private bool isBringingToFront;
            private Bitmap getShadow;
            private Timer timer = new Timer();
            private long lastResizedOn = 0;

            [SecuritySafeCritical]
            public MetroRealisticDropShadow(Form parentForm)
            {
                shadowTargetForm = parentForm;

                FormBorderStyle = FormBorderStyle.None;
                ShowInTaskbar = false;

                uint wl = WinApi.GetWindowLong(this.Handle, -20);
                wl = wl | 0x80000 | 0x20;
                WinApi.SetWindowLong(this.Handle, -20, wl);

                StartPosition = parentForm.StartPosition;

                parentForm.Activated += shadowTargetForm_Activated; 
                Deactivate += MetroRealisticDropShadow_Deactivated;
                parentForm.Move += shadowTargetForm_Move;
                parentForm.Resize += shadowTargetForm_Resize;
                parentForm.ResizeEnd += shadowTargetForm_ResizeEnd;
                parentForm.VisibleChanged += shadowTargetForm_VisibleChanged;

                if (parentForm.Owner != null)
                    Owner = parentForm.Owner;

                parentForm.Owner = this;

                Bounds = GetBounds();

                Load += MetroRealisticDropShadow_Load;
            }

            private void MetroRealisticDropShadow_Load(object sendr, EventArgs e)
            {
                timer.Interval = 50;
                timer.Tick += timer_Tick;
                timer.Start();
            }

            private void timer_Tick(object sendr, EventArgs e)
            {
                timer.Tick -= timer_Tick;
                timer.Stop();
                getShadow = DrawBlurBorder();
                SetBitmap(getShadow, 255);
            }

            private Rectangle GetBounds()
            {
                return new Rectangle((shadowTargetForm).Location.X + Offset.X, (shadowTargetForm).Location.Y + Offset.Y, shadowTargetForm.ClientRectangle.Width + Math.Abs(Offset.X * 2), shadowTargetForm.ClientRectangle.Height + Math.Abs(Offset.Y * 2));
            }

            private void shadowTargetForm_VisibleChanged(object sender, EventArgs e)
            {
                Visible = shadowTargetForm.Visible;
            }

            private void shadowTargetForm_Activated(object o, EventArgs e)
            {
                Visible = (shadowTargetForm.WindowState == FormWindowState.Normal);
                if (Visible) Show();

                if (isBringingToFront)
                {
                    isBringingToFront = false;
                    return;
                }

                this.BringToFront();
            }

            private void MetroRealisticDropShadow_Deactivated(object o, EventArgs e)
            {
                isBringingToFront = true;
            }

            private void shadowTargetForm_Move(object o, EventArgs e)
            {
                if (o is Form)
                    this.Bounds = GetBounds();
            }

            private void shadowTargetForm_Resize(object o, EventArgs e)
            {
                if (o is Form)
                    this.Bounds = GetBounds();

                Visible = false;

                long delta = DateTime.Now.Ticks - lastResizedOn;
                if (delta > 100000)
                {
                    lastResizedOn = DateTime.Now.Ticks;
                    Invalidate();
                }
            }

            private void shadowTargetForm_ResizeEnd(object o, EventArgs e)
            {
                Visible = true;

                lastResizedOn = 0;
                Invalidate();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                getShadow = DrawBlurBorder();
                SetBitmap(getShadow, 255);
            }

            [SecuritySafeCritical]
            private void SetBitmap(Bitmap bitmap, byte opacity)
            {
                if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                    throw new ApplicationException("The bitmap must be 32ppp with alpha-channel.");

                IntPtr screenDc = WinApi.GetDC(IntPtr.Zero);
                IntPtr memDc = WinApi.CreateCompatibleDC(screenDc);
                IntPtr hBitmap = IntPtr.Zero;
                IntPtr oldBitmap = IntPtr.Zero;

                try
                {
                    hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
                    oldBitmap = WinApi.SelectObject(memDc, hBitmap);

                    WinApi.Size size = new WinApi.Size(bitmap.Width, bitmap.Height);
                    WinApi.Point pointSource = new WinApi.Point(0, 0);
                    WinApi.Point topPos = new WinApi.Point(Left, Top);
                    WinApi.BLENDFUNCTION blend = new WinApi.BLENDFUNCTION();
                    blend.BlendOp = WinApi.AC_SRC_OVER;
                    blend.BlendFlags = 0;
                    blend.SourceConstantAlpha = opacity;
                    blend.AlphaFormat = WinApi.AC_SRC_ALPHA;

                    WinApi.UpdateLayeredWindow(Handle, screenDc, ref topPos, ref size, memDc, ref pointSource, 0, ref blend, WinApi.ULW_ALPHA);
                }
                finally
                {
                    WinApi.ReleaseDC(IntPtr.Zero, screenDc);
                    if (hBitmap != IntPtr.Zero)
                    {
                        WinApi.SelectObject(memDc, oldBitmap);
                        WinApi.DeleteObject(hBitmap);
                    }
                    WinApi.DeleteDC(memDc);
                }
            }

            private Bitmap DrawBlurBorder()
            {
                return (Bitmap)DrawOutsetShadow(0, 0, 40, 1, Color.Black, new Rectangle(1, 1, ClientRectangle.Width, ClientRectangle.Height));
            }

            private Image DrawOutsetShadow(int hShadow, int vShadow, int blur, int spread, Color color, Rectangle shadowCanvasArea)
            {
                Rectangle rOuter = shadowCanvasArea;
                Rectangle rInner = shadowCanvasArea;
                rInner.Offset(hShadow, vShadow);
                rInner.Inflate(-blur, -blur);
                rOuter.Inflate(spread, spread);
                rOuter.Offset(hShadow, vShadow);

                Rectangle originalOuter = rOuter;

                Bitmap img = new Bitmap(originalOuter.Width, originalOuter.Height, PixelFormat.Format32bppArgb);
                Graphics g = Graphics.FromImage(img);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                var currentBlur = 0;
                do
                {
                    var transparency = (rOuter.Height - rInner.Height) / (double)(blur * 2 + spread * 2);
                    var shadowColor = Color.FromArgb(((int)(200 * (transparency * transparency))), color);
                    var rOutput = rInner;
                    rOutput.Offset(-originalOuter.Left, -originalOuter.Top);

                    DrawRoundedRectangle(g, rOutput, currentBlur, Pens.Transparent, shadowColor);
                    rInner.Inflate(1, 1);
                    currentBlur = (int)((double)blur * (1 - (transparency * transparency)));

                } while (rOuter.Contains(rInner));

                g.Flush();
                g.Dispose();

                return img;
            }

            private void DrawRoundedRectangle(Graphics g, Rectangle bounds, int cornerRadius, Pen drawPen, Color fillColor)
            {
                int strokeOffset = Convert.ToInt32(Math.Ceiling(drawPen.Width));
                bounds = Rectangle.Inflate(bounds, -strokeOffset, -strokeOffset);

                var gfxPath = new GraphicsPath();

                if (cornerRadius > 0)
                {
                    gfxPath.AddArc(bounds.X, bounds.Y, cornerRadius, cornerRadius, 180, 90);
                    gfxPath.AddArc(bounds.X + bounds.Width - cornerRadius, bounds.Y, cornerRadius, cornerRadius, 270, 90);
                    gfxPath.AddArc(bounds.X + bounds.Width - cornerRadius, bounds.Y + bounds.Height - cornerRadius, cornerRadius, cornerRadius, 0, 90);
                    gfxPath.AddArc(bounds.X, bounds.Y + bounds.Height - cornerRadius, cornerRadius, cornerRadius, 90, 90);
                }
                else
                {
                    gfxPath.AddRectangle(bounds);
                }

                gfxPath.CloseAllFigures();

                if (cornerRadius > 5)
                {
                    using (SolidBrush b = new SolidBrush(fillColor))
                    {
                        g.FillPath(b, gfxPath);
                    }
                }
                if (drawPen != Pens.Transparent)
                {
                    using (Pen p = new Pen(drawPen.Color))
                    {
                        p.EndCap = p.StartCap = LineCap.Round;
                        g.DrawPath(p, gfxPath);
                    }
                }
            }
        }

        #endregion

        #region Helper Methods

        [SecuritySafeCritical]
        public void RemoveCloseButton()
        {
            IntPtr hMenu = WinApi.GetSystemMenu(Handle, false);
            if (hMenu == IntPtr.Zero) return;

            int n = WinApi.GetMenuItemCount(hMenu);
            if (n <= 0) return;

            WinApi.RemoveMenu(hMenu, (uint)(n - 1), WinApi.MfByposition | WinApi.MfRemove);
            WinApi.RemoveMenu(hMenu, (uint)(n - 2), WinApi.MfByposition | WinApi.MfRemove);
            WinApi.DrawMenuBar(Handle);
        }

        private Rectangle MeasureText(Graphics g, Rectangle clientRectangle, Font font, string text, TextFormatFlags flags)
        {
            var proposedSize = new Size(int.MaxValue, int.MinValue);
            var actualSize = TextRenderer.MeasureText(g, text, font, proposedSize, flags);
            return new Rectangle(clientRectangle.X, clientRectangle.Y, actualSize.Width, actualSize.Height);
        }

        private bool isMaximized = false;
        private Size RestoreSize = Size.Empty;
        private Point RestorePoint = Point.Empty;

        private void HandleMaximizeWindow()
        {
            if (!MaximizeBox || !Resizable)
            {
                return;
            }

            if (isMaximized)
            {
                Size = RestoreSize;
                Location = RestorePoint;
                isMaximized = false;
                OnResizeEnd(EventArgs.Empty);
            }
            else
            {
                RestoreSize = Size;
                RestorePoint = Location;
                Size = Screen.FromPoint(Location).WorkingArea.Size;
                Location = Screen.FromPoint(Location).WorkingArea.Location;
                isMaximized = true;
            }

            if (windowButtonList != null)
            {
                foreach (KeyValuePair<WindowButtons, MetroFormButton> button in windowButtonList)
                {
                    if (button.Key == WindowButtons.Maximize)
                    {
                        if (isMaximized)
                        {
                            button.Value.Text = "2";
                        }
                        else
                        {
                            button.Value.Text = "1";
                        }
                    }
                }
            }
        }

        #endregion
    }
}
