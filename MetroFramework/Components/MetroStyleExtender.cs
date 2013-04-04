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
    public sealed class MetroStyleExtender : Component, IExtenderProvider, IMetroComponent
	{
        private readonly List<Control> extendedControls = new List<Control>();

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

            foreach (Control ctrl in extendedControls)
            {
                if (ctrl != null)
                {
                    try
                    {
                        ctrl.BackColor = backColor;
                    }
                    catch { }

                    try
                    {
                        ctrl.ForeColor = foreColor;
                    }
                    catch { }
                }
            }
        }

        #region IExtenderProvider implementation

        bool IExtenderProvider.CanExtend(object target)
		{
		    return target is Control && !(target is IMetroControl || target is IMetroForm);
		}

        [DefaultValue(false)]
        [Category("Metro Appearance")]
        [Description("Apply Metro Theme BackColor and ForeColor.")]
        public bool GetApplyMetroTheme(Control control)
		{
		    return control != null && extendedControls.Contains(control);
		}

        public void SetApplyMetroTheme(Control control, bool value)
        {
            if (control == null)
            {
                return;
            }

            if (extendedControls.Contains(control))
            {
                if (!value)
                {
                    extendedControls.Remove(control);
                }
            }
            else
            {
                if (value)
                {
                    extendedControls.Add(control);
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
