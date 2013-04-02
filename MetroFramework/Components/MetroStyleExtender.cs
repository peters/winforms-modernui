using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;

namespace MetroFramework.Components
{
    /// <summary>
    ///     Extend legacy controls with an <c>ApplyMetroTheme</c> property.
    /// </summary>
    /// <remarks>
    ///     The theme is applied to <see cref="Control.BackColor"/> and <see cref="Control.ForeColor"/> only.
    /// </remarks>
	[ProvideProperty("ApplyMetroTheme",typeof(Control))] // we can provide more than one property if we like
    public class MetroStyleExtender : Component, IExtenderProvider, IMetroComponent
	{

        // see http://www.codeproject.com/Articles/4683/Getting-to-know-IExtenderProvider

        // TODO: Need something more performant here if we extend > 10 controls
        private readonly List<Control> _extendedControls = new List<Control>();

        public MetroStyleExtender()
        {}

        public MetroStyleExtender(IContainer parent)
            : this()
        {
            Debug.Assert(parent!=null);
            parent.Add(this);
        }

        private void UpdateTheme(MetroThemeStyle theme)
        {
            Color backColor = MetroPaint.BackColor.Form(theme);
            Color foreColor = MetroPaint.ForeColor.Label.Normal(theme);
            foreach (var c in _extendedControls)
            {
                try
                {
                    c.BackColor = backColor;
                }
                catch
                {
                }
                try
                {
                    c.ForeColor = foreColor;
                }
                catch
                {
                }
            }
        }

        #region IExtenderProvider implementation

        bool IExtenderProvider.CanExtend(object target)
		{
		    return target is Control && !(target is IMetroControl);
		}

        [DefaultValue(false)]
        [Category("Metro Appearance")]
        [Description("Apply Metro Theme background and foreground colors.")]
        public bool GetApplyMetroTheme(Control control)
		{
		    return control != null && _extendedControls.Contains(control);
		}

        public void SetApplyMetroTheme(Control control, bool value)
        {
            if (control == null) return;
            if (_extendedControls.Contains(control))
            {
                if (!value) _extendedControls.Remove(control);
            }
            else
            {
                if (value) _extendedControls.Add(control);
            }
        }

        #endregion

        #region IMetroComponent implementation

        [Browsable(false)]
	    MetroColorStyle IMetroComponent.Style
	    {
            get { throw new NotSupportedException(); } 
            set { /* ignore */ }
	    }
        [Browsable(false)]
	    MetroThemeStyle IMetroComponent.Theme
	    {
            get { throw new NotSupportedException(); } 
            set { UpdateTheme(value); }
	    }
        [Browsable(false)]
	    MetroStyleManager IMetroComponent.StyleManager
	    {
            get { throw new NotSupportedException(); }
            set { /* ignore */ }
        }

        #endregion

	}
}
