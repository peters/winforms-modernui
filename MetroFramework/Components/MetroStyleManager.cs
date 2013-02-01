using System;
using System.ComponentModel;
using System.Windows.Forms;
using MetroFramework.Interfaces;

namespace MetroFramework.Components
{
    public class MetroStyleManager : Component, ICloneable
    {
        private Form ownerForm = null;
        public Form OwnerForm
        {
            get { return ownerForm; }
            set 
            {
                if (ownerForm != null)
                    return;

                ownerForm = value;
                ownerForm.ControlAdded += new ControlEventHandler(NewControlOnOwnerForm);
                
                UpdateOwnerForm();
            }
        }

        private MetroColorStyle metroStyle = MetroColorStyle.Blue;
        public MetroColorStyle Style
        {
            get { return metroStyle; }
            set 
            { 
                metroStyle = value;
                UpdateOwnerForm();
            }
        }

        private MetroThemeStyle metroTheme = MetroThemeStyle.Light;
        public MetroThemeStyle Theme
        {
            get { return metroTheme; }
            set 
            {
                metroTheme = value;
                UpdateOwnerForm();
            }
        }

        public MetroStyleManager()
        {

        }

        public MetroStyleManager(Form ownerForm)
        {
            this.OwnerForm = ownerForm;
        }

        private void NewControlOnOwnerForm(object sender, ControlEventArgs e)
        {
            if (e.Control is IMetroControl)
            {
                ((IMetroControl)e.Control).Style = Style;
                ((IMetroControl)e.Control).Theme = Theme;
                ((IMetroControl)e.Control).StyleManager = this;
            }
            else
            {
                UpdateOwnerForm();
            }
        }

        public void UpdateOwnerForm()
        {
            if (ownerForm == null)
                return;

            if (ownerForm is IMetroForm)
            {
                ((IMetroForm)ownerForm).Style = Style;
                ((IMetroForm)ownerForm).Theme = Theme;
                ((IMetroForm)ownerForm).StyleManager = this;
            }

            if (ownerForm.Controls.Count > 0)
                UpdateControlCollection(ownerForm.Controls);

            ownerForm.Refresh();
        }

        private void UpdateControlCollection(Control.ControlCollection controls)
        {
            foreach (Control c in controls)
            {
                if (c is IMetroControl)
                {
                    ((IMetroControl)c).Style = Style;
                    ((IMetroControl)c).Theme = Theme;
                    ((IMetroControl)c).StyleManager = this;
                }

                if (c is Panel || c is GroupBox || c is ContainerControl)
                {
                    UpdateControlCollection(c.Controls);
                }
                else
                {
                    if (c.Controls.Count > 0)
                        UpdateControlCollection(c.Controls);
                }

            }
        }

        public object Clone()
        {
            MetroStyleManager newStyleManager = new MetroStyleManager();
            newStyleManager.metroTheme = this.Theme;
            newStyleManager.metroStyle = this.Style;
            newStyleManager.ownerForm = null;

            return newStyleManager;
        }
    }
}
