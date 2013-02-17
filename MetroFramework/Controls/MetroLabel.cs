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

    public enum LabelMode
    {
        Text,
        Selectable
    }

    [Designer(typeof(MetroLabelDesigner))]
    [ToolboxBitmap(typeof(Label))]
    public class MetroLabel : Label, IMetroControl
    {

        #region Variables
        /// <summary>
        /// 
        /// </summary>
        private TextBox _baseTextBox;
        #endregion

        #region Interface

        private MetroColorStyle _metroStyle = MetroColorStyle.Blue;
        [Category("Metro Appearance")]
        public MetroColorStyle Style
        {
            get
            {
                return StyleManager != null ? StyleManager.Style : _metroStyle;
            }
            set { _metroStyle = value; }
        }

        private MetroThemeStyle _metroTheme = MetroThemeStyle.Light;
        [Category("Metro Appearance")]
        public MetroThemeStyle Theme
        {
            get
            {
                return StyleManager != null ? StyleManager.Theme : _metroTheme;
            }
            set { _metroTheme = value; }
        }

        [Browsable(false)]
        public MetroStyleManager StyleManager { get; set; }

        #endregion

        #region Fields

        private bool _useStyleColors;
        [Category("Metro Appearance")]
        public bool UseStyleColors
        {
            get { return _useStyleColors; }
            set { 
                _useStyleColors = value;
                if (_labelMode == LabelMode.Selectable)
                {
                    UpdateBaseTextBox();
                }
            }
        }

        private MetroLabelSize _metroLabelSize = MetroLabelSize.Medium;
        [Category("Metro Appearance")]
        public MetroLabelSize FontSize
        {
            get { return _metroLabelSize; }
            set
            {
                _metroLabelSize = value;
                if (_labelMode == LabelMode.Selectable)
                {
                    UpdateBaseTextBox();
                }
            }
        }

        private MetroLabelWeight _metroLabelWeight = MetroLabelWeight.Light;
        [Category("Metro Appearance")]
        public MetroLabelWeight FontWeight
        {
            get { return _metroLabelWeight; }
            set
            {
                _metroLabelWeight = value;
                if (_labelMode == LabelMode.Selectable)
                {
                    UpdateBaseTextBox();
                }
            }
        }

        private LabelMode _labelMode = LabelMode.Text;
        [Category("Metro Appearance")]
        public LabelMode LabelMode
        {
            get { return _labelMode; }
            set
            {
                _labelMode = value;
            }
        }

        #endregion

        #region Constructor

        public MetroLabel()
        {
            StyleManager = null;
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
            Color foreColor;

            Color backColor = Parent != null ? Parent.BackColor : MetroPaint.BackColor.Form(Theme);

            if (!Enabled)
            {
                foreColor = MetroPaint.ForeColor.Label.Disabled(Theme);
            }
            else
            {
                foreColor = !_useStyleColors ? MetroPaint.ForeColor.Label.Normal(Theme) : MetroPaint.GetStyleColor(Style);
            }
        
            e.Graphics.Clear(backColor);
            
            switch (LabelMode)
            {
                case LabelMode.Text:
                    if (_baseTextBox != null)
                    {
                        DestroyBaseTextbox();
                    }
                    TextRenderer.DrawText(e.Graphics, Text, MetroFonts.Label(_metroLabelSize, _metroLabelWeight), 
                        ClientRectangle, foreColor, backColor, MetroPaint.GetTextFormatFlags(TextAlign));
                    break;
                case LabelMode.Selectable:
                    if (_baseTextBox == null)
                    {
                        CreateBaseTextBox();
                    }
                    UpdateBaseTextBox();
                    break;
            }
            
        }

        #endregion

        #region Overridden Methods

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size preferredSize;
            base.GetPreferredSize(proposedSize);

            using (var g = CreateGraphics())
            {
                proposedSize = new Size(int.MaxValue, int.MaxValue);
                preferredSize = TextRenderer.MeasureText(g, Text, MetroFonts.Label(_metroLabelSize, _metroLabelWeight), proposedSize, MetroPaint.GetTextFormatFlags(TextAlign));
            }

            return preferredSize;
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Invalidate();
        }
        #endregion

        #region Private methods
        private void CreateBaseTextBox()
        {
            if (_baseTextBox != null) return;
            _baseTextBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = MetroFonts.Label(_metroLabelSize, _metroLabelWeight),
                Location = new Point(3, 3),
                Text = Text,
                ReadOnly = true,
                BackColor =  Parent != null ? Parent.BackColor : MetroPaint.BackColor.Form(Theme),
                ForeColor = !_useStyleColors ? MetroPaint.ForeColor.Label.Normal(Theme) : MetroPaint.GetStyleColor(Style),
                Size = GetPreferredSize(Size.Empty),
                Multiline = true
            };
            _baseTextBox.DoubleClick += BaseTextBoxOnDoubleClick;
            _baseTextBox.Click += BaseTextBoxOnClick;
            Controls.Add(_baseTextBox);
        }

        /// <summary>
        /// 
        /// </summary>
        private void DestroyBaseTextbox()
        {
            if (_baseTextBox == null) return;
            Controls.Remove(_baseTextBox);
            _baseTextBox.DoubleClick -= BaseTextBoxOnDoubleClick;
            _baseTextBox.Click -= BaseTextBoxOnClick;
            _baseTextBox.Dispose();
            _baseTextBox = null;
        }
        /// <summary>
        /// 
        /// </summary>
        private void UpdateBaseTextBox()
        {
            if (_baseTextBox == null) return;
            _baseTextBox.Font = MetroFonts.Label(_metroLabelSize, _metroLabelWeight);
            _baseTextBox.Text = Text;
            Size = GetPreferredSize(Size.Empty);
        }
        #endregion

        #region Textbox events
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void BaseTextBoxOnClick(object sender, EventArgs eventArgs)
        {
            Native.WinCaret.HideCaret(_baseTextBox.Handle);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void BaseTextBoxOnDoubleClick(object sender, EventArgs eventArgs)
        {
            _baseTextBox.SelectAll();
            Native.WinCaret.HideCaret(_baseTextBox.Handle);
        }
        #endregion
    }
}
