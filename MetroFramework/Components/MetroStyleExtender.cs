using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using MetroFramework.Drawing;
using MetroFramework.Interfaces;

namespace MetroFramework.Components
{
	[ProvideProperty("ApplyMetroTheme", typeof(Control))]
    [ProvideProperty("ApplyMetroFont", typeof(Control))]
    public sealed class MetroStyleExtender : Component, IExtenderProvider, IMetroComponent
	{
        private readonly Hashtable extendedControlsTheme = new Hashtable();
        private readonly Hashtable extendedControlsFont = new Hashtable();

        public MetroStyleExtender()
        {
        
        }

        public MetroStyleExtender(IContainer parent)
            : this()
        {
            if (parent != null)
            {
                parent.Add(this);
            }
        }

        private void UpdateTheme(MetroThemeStyle theme)
        {
            Color backColor = MetroPaint.BackColor.Form(theme);
            Color foreColor = MetroPaint.ForeColor.Label.Normal(theme);

            foreach (DictionaryEntry de in extendedControlsTheme)
            {
                Control ctrl = de.Value as Control;
                if (ctrl != null)
                {
                    try
                    {
                        ctrl.BackColor = backColor;
                        ctrl.ForeColor = foreColor;
                    }
                    catch { }
                }
            }

            foreach (DictionaryEntry de in extendedControlsFont)
            {
                Control ctrl = de.Value as Control;
                if (ctrl != null)
                {
                    try
                    {
                        ctrl.Font = MetroFonts.Label(MetroLabelSize.Small, MetroLabelWeight.Light);
                    }
                    catch { }
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
        [Description("Apply Metro Theme BackColor and ForeColor.")]
        public bool GetApplyMetroTheme(Control control)
		{
		    return control != null && extendedControlsTheme.Contains(control.GetHashCode());
		}

        public void SetApplyMetroTheme(Control control, bool value)
        {
            if (control == null)
            {
                return;
            }

            int ctrlHashCode = control.GetHashCode();

            if (extendedControlsTheme.Contains(ctrlHashCode))
            {
                if (!value)
                {
                    extendedControlsTheme.Remove(ctrlHashCode);
                }
            }
            else
            {
                if (value)
                {
                    extendedControlsTheme.Add(ctrlHashCode, control);
                }
            }
        }

        [DefaultValue(false)]
        [Category("Metro Appearance")]
        [Description("Apply Metro Font.")]
        public bool GetApplyMetroFont(Control control)
        {
            return control != null && extendedControlsFont.Contains(control.GetHashCode());
        }

        public void SetApplyMetroFont(Control control, bool value)
        {
            if (control == null)
            {
                return;
            }

            int ctrlHashCode = control.GetHashCode();

            if (extendedControlsFont.Contains(ctrlHashCode))
            {
                if (!value)
                {
                    extendedControlsFont.Remove(ctrlHashCode);
                }
            }
            else
            {
                if (value)
                {
                    extendedControlsFont.Add(ctrlHashCode, control);
                }
            }
        }

        #endregion

        #region IMetroComponent implementation

        [Browsable(false)]
        MetroColorStyle IMetroComponent.Style
	    {
            get { throw new NotSupportedException(); } 
            set { }
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
            set { }
        }

        #endregion
    }
}
