using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MetroFramework.Components;
using MetroFramework.Design;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;
using MetroFramework.Native;

namespace MetroFramework.Controls
{
    public enum ScrollBarOrientation
    {
        Horizontal,
        Vertical
    }

    [Designer(typeof (MetroScrollBarDesigner)), DefaultEvent("Scroll"), DefaultProperty("Value")]
    public class MetroScrollBar : Control, IMetroControl
    {
        #region Interface

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

        #region Events

        public event ScrollEventHandler Scroll;

        private void OnScroll(ScrollEventType type, int oldValue, int newValue, ScrollOrientation orientation)
        {
            if (Scroll == null)
            {
                return;
            }
            switch (orientation)
            {
                case ScrollOrientation.HorizontalScroll:
                    if (type != ScrollEventType.EndScroll && _isFirstScrollEventHorizontal)
                    {
                        type = ScrollEventType.First;
                    } else if (!_isFirstScrollEventHorizontal && type == ScrollEventType.EndScroll)
                    {
                        _isFirstScrollEventHorizontal = true;
                    }
                    break;
                case ScrollOrientation.VerticalScroll:
                    if (type != ScrollEventType.EndScroll && _isFirstScrollEventVertical)
                    {
                        type = ScrollEventType.First;
                    }
                    else if (!_isFirstScrollEventHorizontal && type == ScrollEventType.EndScroll)
                    {
                        _isFirstScrollEventVertical = true;
                    }
                    break;
            }
            Scroll(this, new ScrollEventArgs(type, oldValue, newValue, orientation));
        }

        #endregion

        #region Fields

        private bool _isFirstScrollEventVertical = true;
        private bool _isFirstScrollEventHorizontal = true;

        private bool _inUpdate;

        private ScrollBarOrientation _orientation = ScrollBarOrientation.Vertical;
        private ScrollOrientation _scrollOrientation = ScrollOrientation.VerticalScroll;

        private Rectangle _clickedBarRectangle;
        private Rectangle _thumbRectangle;

        private bool _topBarClicked;
        private bool _bottomBarClicked;
        private bool _thumbClicked;

        private int _minimum;
        private int _maximum = 100;
        private int _smallChange = 1;
        private int _largeChange = 10;
        private int _value;

        private int _thumbWidth = 6;
        private int _thumbHeight;

        private int _thumbBottomLimitBottom;
        private int _thumbBottomLimitTop;
        private int _thumbTopLimit;
        private int _thumbPosition;

        private int _trackPosition;

        private readonly Timer _progressTimer = new Timer();

        private int _mouseWheelBarPartitions = 10;

        public int MouseWheelBarPartitions
        {
            get
            {
                return _mouseWheelBarPartitions;
            }
            set
            {
                if (value > 0)
                {
                    _mouseWheelBarPartitions = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("value","MouseWheelBarPartitions has to be greather than zero");
                }
            }
        }

        private bool _isHovered;
        private bool _isPressed;

        private bool _useBarColor;

        [Category("Metro Appearance")]
        public bool UseBarColor
        {
            get
            {
                return _useBarColor;
            }
            set
            {
                _useBarColor = value;
            }
        }

        #endregion

        #region Properties

        public ScrollBarOrientation Orientation
        {
            get
            {
                return _orientation;
            }

            set
            {
                if (value == _orientation)
                {
                    return;
                }

                _orientation = value;

                _scrollOrientation = value == ScrollBarOrientation.Vertical ?
                                         ScrollOrientation.VerticalScroll : ScrollOrientation.HorizontalScroll;

                if (DesignMode)
                {
                    Size = new Size(Height, Width);
                }

                SetUpScrollBar();
            }
        }

        public int Minimum
        {
            get
            {
                return _minimum;
            }

            set
            {
                if (_minimum == value || value < 0 || value >= _maximum)
                {
                    return;
                }

                _minimum = value;
                if (_value < value)
                {
                    _value = value;
                }

                if (_largeChange > _maximum - _minimum)
                {
                    _largeChange = _maximum - _minimum;
                }

                SetUpScrollBar();

                if (_value < value)
                {
                    Value = value;
                }
                else
                {
                    ChangeThumbPosition(GetThumbPosition());
                    Refresh();
                }
            }
        }

        public int Maximum
        {
            get
            {
                return _maximum;
            }

            set
            {
                if (value == _maximum || value < 1 || value <= _minimum)
                {
                    return;
                }

                _maximum = value;

                if (_largeChange > _maximum - _minimum)
                {
                    _largeChange = _maximum - _minimum;
                }

                SetUpScrollBar();

                if (_value > value)
                {
                    Value = _maximum;
                }
                else
                {
                    ChangeThumbPosition(GetThumbPosition());

                    Refresh();
                }
            }
        }

        public int SmallChange
        {
            get
            {
                return _smallChange;
            }

            set
            {
                if (value == _smallChange || value < 1 || value >= _largeChange)
                {
                    return;
                }

                _smallChange = value;

                SetUpScrollBar();
            }
        }

        public int LargeChange
        {
            get
            {
                return _largeChange;
            }

            set
            {
                if (value == _largeChange || value < _smallChange || value < 2)
                {
                    return;
                }

                if (value > _maximum - _minimum)
                {
                    _largeChange = _maximum - _minimum;
                }
                else
                {
                    _largeChange = value;
                }

                SetUpScrollBar();
            }
        }

        public int Value
        {
            get
            {
                return _value;
            }

            set
            {
                if (_value == value || value < _minimum || value > _maximum)
                {
                    return;
                }

                _value = value;

                ChangeThumbPosition(GetThumbPosition());

                OnScroll(ScrollEventType.ThumbPosition, -1, _value, _scrollOrientation);

                Refresh();
            }
        }

        #endregion

        #region Constructor

        public MetroScrollBar()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.Selectable |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint, true);

            Width = 6;
            Height = 200;

            SetUpScrollBar();

            _progressTimer.Interval = 20;
            _progressTimer.Tick += ProgressTimerTick;
        }

        public bool HitTest(Point point)
        {
            return _thumbRectangle.Contains(point);
        }

        #endregion

        #region Update Methods

        public void BeginUpdate()
        {
            WinApi.SendMessage(Handle, (int) WinApi.Messages.WM_SETREDRAW, false, 0);
            _inUpdate = true;
        }

        public void EndUpdate()
        {
            WinApi.SendMessage(Handle, (int)WinApi.Messages.WM_SETREDRAW, true, 0);
            _inUpdate = false;
            SetUpScrollBar();
            Refresh();
        }

        #endregion

        #region Paint Methods

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // no painting here
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Color thumbColor, barColor;

            Color backColor = Parent != null ? Parent.BackColor : MetroPaint.BackColor.Form(Theme);

            if (_isHovered && !_isPressed && Enabled)
            {
                thumbColor = MetroPaint.BackColor.ScrollBar.Thumb.Hover(Theme);
                barColor = MetroPaint.BackColor.ScrollBar.Bar.Hover(Theme);
            }
            else if (_isHovered && _isPressed && Enabled)
            {
                thumbColor = MetroPaint.BackColor.ScrollBar.Thumb.Press(Theme);
                barColor = MetroPaint.BackColor.ScrollBar.Bar.Press(Theme);
            }
            else if (!Enabled)
            {
                thumbColor = MetroPaint.BackColor.ScrollBar.Thumb.Disabled(Theme);
                barColor = MetroPaint.BackColor.ScrollBar.Bar.Disabled(Theme);
            }
            else
            {
                thumbColor = MetroPaint.BackColor.ScrollBar.Thumb.Normal(Theme);
                barColor = MetroPaint.BackColor.ScrollBar.Bar.Normal(Theme);
            }

            e.Graphics.Clear(backColor);
            DrawScrollBar(e.Graphics, backColor, thumbColor, barColor);
        }

        private void DrawScrollBar(Graphics g, Color backColor, Color thumbColor, Color barColor)
        {
            if (_useBarColor)
            {
                using (var b = new SolidBrush(barColor))
                {
                    g.FillRectangle(b, ClientRectangle);
                }
            }

            using (var b = new SolidBrush(backColor))
            {
                var thumbRect = new Rectangle(_thumbRectangle.X - 1, _thumbRectangle.Y - 1, _thumbRectangle.Width + 2, _thumbRectangle.Height + 2);
                g.FillRectangle(b, thumbRect);
            }

            using (var b = new SolidBrush(thumbColor))
            {
                g.FillRectangle(b, _thumbRectangle);
            }
        }

        #endregion

        #region Focus Methods

        protected override void OnGotFocus(EventArgs e)
        {
            Invalidate();

            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            _isHovered = false;
            _isPressed = false;
            Invalidate();

            base.OnLostFocus(e);
        }

        protected override void OnEnter(EventArgs e)
        {
            Invalidate();

            base.OnEnter(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            _isHovered = false;
            _isPressed = false;
            Invalidate();

            base.OnLeave(e);
        }

        #endregion

        #region Mouse Methods

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            int v = e.Delta/120*(_maximum - _minimum)/_mouseWheelBarPartitions;

            if (_orientation == ScrollBarOrientation.Vertical)
            {
                Value -= v;
            }
            else
            {
                Value += v;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isPressed = true;
                Invalidate();
            }

            base.OnMouseDown(e);

            Focus();

            switch (e.Button)
            {
                case MouseButtons.Left:
                {
                    var mouseLocation = e.Location;

                    if (_thumbRectangle.Contains(mouseLocation))
                    {
                        _thumbClicked = true;
                        _thumbPosition = _orientation == ScrollBarOrientation.Vertical ? mouseLocation.Y - _thumbRectangle.Y : mouseLocation.X - _thumbRectangle.X;

                        Invalidate(_thumbRectangle);
                    }
                    else
                    {
                        _trackPosition =
                            _orientation == ScrollBarOrientation.Vertical ?
                                mouseLocation.Y : mouseLocation.X;

                        if (_trackPosition <
                            (_orientation == ScrollBarOrientation.Vertical ?
                                 _thumbRectangle.Y : _thumbRectangle.X))
                        {
                            _topBarClicked = true;
                        }
                        else
                        {
                            _bottomBarClicked = true;
                        }

                        ProgressThumb(true);
                    }
                }
                    break;
                case MouseButtons.Right:
                    _trackPosition =
                        _orientation == ScrollBarOrientation.Vertical ? e.Y : e.X;
                    break;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isPressed = false;

            base.OnMouseUp(e);

            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (_thumbClicked)
                    {
                        _thumbClicked = false;

                        OnScroll(
                            ScrollEventType.EndScroll,
                            -1,
                            _value,
                            _scrollOrientation
                            );
                    }
                    else if (_topBarClicked)
                    {
                        _topBarClicked = false;
                        StopTimer();
                    }
                    else if (_bottomBarClicked)
                    {
                        _bottomBarClicked = false;
                        StopTimer();
                    }
                    Invalidate();
                    break;
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true;
            Invalidate();

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false;
            Invalidate();

            base.OnMouseLeave(e);

            ResetScrollStatus();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // moving and holding the left mouse button
            if (e.Button == MouseButtons.Left)
            {
                // Update the thumb position, if the new location is within the bounds.
                if (_thumbClicked)
                {
                    int oldScrollValue = _value;

                    int pos = _orientation == ScrollBarOrientation.Vertical ?
                                  e.Location.Y : e.Location.X;

                    // The thumb is all the way to the top
                    if (pos <= (_thumbTopLimit + _thumbPosition))
                    {
                        ChangeThumbPosition(_thumbTopLimit);

                        _value = _minimum;
                        Invalidate();
                    }
                    else if (pos >= (_thumbBottomLimitTop + _thumbPosition))
                    {
                        // The thumb is all the way to the bottom
                        ChangeThumbPosition(_thumbBottomLimitTop);

                        _value = _maximum;
                        Invalidate();
                    }
                    else
                    {
                        // The thumb is between the ends of the track.
                        ChangeThumbPosition(pos - _thumbPosition);

                        int pixelRange, thumbPos;

                        // calculate the value - first some helper variables
                        // dependent on the current orientation
                        if (_orientation == ScrollBarOrientation.Vertical)
                        {
                            pixelRange = Height - _thumbHeight;
                            thumbPos = _thumbRectangle.Y;
                        }
                        else
                        {
                            pixelRange = Width - _thumbWidth;
                            thumbPos = _thumbRectangle.X;
                        }

                        float perc = 0f;

                        if (pixelRange != 0)
                        {
                            // percent of the new position
                            perc = (thumbPos)/(float) pixelRange;
                        }

                        // the new value is somewhere between max and min, starting
                        // at min position
                        _value = Convert.ToInt32((perc*(_maximum - _minimum)) + _minimum);
                    }

                    // raise scroll event if new value different
                    if (oldScrollValue != _value)
                    {
                        OnScroll(ScrollEventType.ThumbTrack, oldScrollValue, _value, _scrollOrientation);

                        Refresh();
                    }
                }
            }
            else if (!ClientRectangle.Contains(e.Location))
            {
                ResetScrollStatus();
            }
            else if (e.Button == MouseButtons.None) // only moving the mouse
            {
                if (_thumbRectangle.Contains(e.Location))
                {
                    Invalidate(_thumbRectangle);
                }
                else if (ClientRectangle.Contains(e.Location))
                {
                    Invalidate();
                }
            }
        }

        #endregion

        #region Keyboard Methods

        protected override void OnKeyDown(KeyEventArgs e)
        {
            _isHovered = true;
            _isPressed = true;
            Invalidate();

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            _isHovered = false;
            _isPressed = false;
            Invalidate();

            base.OnKeyUp(e);
        }

        #endregion

        #region Management Methods

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (DesignMode)
            {
                if (_orientation == ScrollBarOrientation.Vertical)
                {
                    if (height < 10)
                    {
                        height = 10;
                    }

                    width = 6;
                }
                else
                {
                    if (width < 10)
                    {
                        width = 10;
                    }

                    height = 6;
                }
            }

            base.SetBoundsCore(x, y, width, height, specified);

            if (DesignMode)
            {
                SetUpScrollBar();
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            SetUpScrollBar();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            var keyUp = Keys.Up;
            var keyDown = Keys.Down;

            if (_orientation == ScrollBarOrientation.Horizontal)
            {
                keyUp = Keys.Left;
                keyDown = Keys.Right;
            }

            if (keyData == keyUp)
            {
                Value -= _smallChange;

                return true;
            }

            if (keyData == keyDown)
            {
                Value += _smallChange;

                return true;
            }

            if (keyData == Keys.PageUp)
            {
                Value = GetValue(false, true);

                return true;
            }

            if (keyData == Keys.PageDown)
            {
                if (_value + _largeChange > _maximum)
                {
                    Value = _maximum;
                }
                else
                {
                    Value += _largeChange;
                }

                return true;
            }

            if (keyData == Keys.Home)
            {
                Value = _minimum;

                return true;
            }

            if (keyData == Keys.End)
            {
                Value = _maximum;

                return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Invalidate();
        }

        private void SetUpScrollBar()
        {
            if (_inUpdate)
            {
                return;
            }

            if (_orientation == ScrollBarOrientation.Vertical)
            {
                _thumbWidth = Width > 0 ? Width : 6;
                _thumbHeight = GetThumbSize();

                _clickedBarRectangle = ClientRectangle;
                _clickedBarRectangle.Inflate(-1, -1);

                _thumbRectangle = new Rectangle(
                    ClientRectangle.X,
                    ClientRectangle.Y,
                    _thumbWidth,
                    _thumbHeight
                    );

                _thumbPosition = _thumbRectangle.Height/2;
                _thumbBottomLimitBottom = ClientRectangle.Bottom;
                _thumbBottomLimitTop = _thumbBottomLimitBottom - _thumbRectangle.Height;
                _thumbTopLimit = ClientRectangle.Y;
            }
            else
            {
                _thumbHeight = 6;
                _thumbWidth = GetThumbSize();

                _clickedBarRectangle = ClientRectangle;
                _clickedBarRectangle.Inflate(-1, -1);

                _thumbRectangle = new Rectangle(
                    ClientRectangle.X,
                    ClientRectangle.Y,
                    _thumbWidth,
                    _thumbHeight
                    );

                _thumbPosition = _thumbRectangle.Width/2;
                _thumbBottomLimitBottom = ClientRectangle.Right;
                _thumbBottomLimitTop = _thumbBottomLimitBottom - _thumbRectangle.Width;
                _thumbTopLimit = ClientRectangle.X;
            }

            ChangeThumbPosition(GetThumbPosition());

            Refresh();
        }

        private void ResetScrollStatus()
        {
            _bottomBarClicked = _topBarClicked = false;

            StopTimer();
            Refresh();
        }

        private void ProgressTimerTick(object sender, EventArgs e)
        {
            ProgressThumb(true);
        }

        private int GetValue(bool smallIncrement, bool up)
        {
            int newValue;

            if (up)
            {
                newValue = _value - (smallIncrement ? _smallChange : _largeChange);

                if (newValue < _minimum)
                {
                    newValue = _minimum;
                }
            }
            else
            {
                newValue = _value + (smallIncrement ? _smallChange : _largeChange);

                if (newValue > _maximum)
                {
                    newValue = _maximum;
                }
            }

            return newValue;
        }

        private int GetThumbPosition()
        {
            int pixelRange;

            if (_orientation == ScrollBarOrientation.Vertical)
            {
                pixelRange = Height - _thumbHeight;
            }
            else
            {
                pixelRange = Width - _thumbWidth;
            }

            int realRange = _maximum - _minimum;
            float perc = 0f;

            if (realRange != 0)
            {
                perc = (_value - (float) _minimum)/realRange;
            }

            return Math.Max(_thumbTopLimit, Math.Min(_thumbBottomLimitTop, Convert.ToInt32((perc*pixelRange))));
        }

        private int GetThumbSize()
        {
            int trackSize =
                _orientation == ScrollBarOrientation.Vertical ?
                    Height : Width;

            if (_maximum == 0 || _largeChange == 0)
            {
                return trackSize;
            }

            float newThumbSize = (_largeChange*(float) trackSize)/_maximum;

            return Convert.ToInt32(Math.Min(trackSize, Math.Max(newThumbSize, 10f)));
        }

        private void EnableTimer()
        {
            if (!_progressTimer.Enabled)
            {
                _progressTimer.Interval = 600;
                _progressTimer.Start();
            }
            else
            {
                _progressTimer.Interval = 10;
            }
        }

        private void StopTimer()
        {
            _progressTimer.Stop();
        }

        private void ChangeThumbPosition(int position)
        {
            if (_orientation == ScrollBarOrientation.Vertical)
            {
                _thumbRectangle.Y = position;
            }
            else
            {
                _thumbRectangle.X = position;
            }
        }

        private void ProgressThumb(bool enableTimer)
        {
            int scrollOldValue = _value;
            var type = ScrollEventType.First;
            int thumbSize, thumbPos;

            if (_orientation == ScrollBarOrientation.Vertical)
            {
                thumbPos = _thumbRectangle.Y;
                thumbSize = _thumbRectangle.Height;
            }
            else
            {
                thumbPos = _thumbRectangle.X;
                thumbSize = _thumbRectangle.Width;
            }

            if ((_bottomBarClicked && (thumbPos + thumbSize) < _trackPosition))
            {
                type = ScrollEventType.LargeIncrement;

                _value = GetValue(false, false);

                if (_value == _maximum)
                {
                    ChangeThumbPosition(_thumbBottomLimitTop);

                    type = ScrollEventType.Last;
                }
                else
                {
                    ChangeThumbPosition(Math.Min(_thumbBottomLimitTop, GetThumbPosition()));
                }
            }
            else if ((_topBarClicked && thumbPos > _trackPosition))
            {
                type = ScrollEventType.LargeDecrement;

                _value = GetValue(false, true);

                if (_value == _minimum)
                {
                    ChangeThumbPosition(_thumbTopLimit);

                    type = ScrollEventType.First;
                }
                else
                {
                    ChangeThumbPosition(Math.Max(_thumbTopLimit, GetThumbPosition()));
                }
            }

            if (scrollOldValue != _value)
            {
                OnScroll(type, scrollOldValue, _value, _scrollOrientation);

                Invalidate();

                if (enableTimer)
                {
                    EnableTimer();
                }
            }
        }

        #endregion
    }
}