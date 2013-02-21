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
            else if (e.Control is IMetroComponent)
            {
                ((IMetroComponent)e.Control).Style = Style;
                ((IMetroComponent)e.Control).Theme = Theme;
                ((IMetroComponent)e.Control).StyleManager = this;
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

            if (ownerForm.ContextMenuStrip != null && ownerForm.ContextMenuStrip is IMetroComponent)
            {
                ((IMetroComponent)ownerForm.ContextMenuStrip).Style = Style;
                ((IMetroComponent)ownerForm.ContextMenuStrip).Theme = Theme;
                ((IMetroComponent)ownerForm.ContextMenuStrip).StyleManager = this;
            }

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

                if (c.ContextMenuStrip != null && c.ContextMenuStrip is IMetroComponent)
                {
                    ((IMetroComponent)c.ContextMenuStrip).Style = Style;
                    ((IMetroComponent)c.ContextMenuStrip).Theme = Theme;
                    ((IMetroComponent)c.ContextMenuStrip).StyleManager = this;
                }
                else if (c is IMetroComponent)
                {
                    ((IMetroComponent)c.ContextMenuStrip).Style = Style;
                    ((IMetroComponent)c.ContextMenuStrip).Theme = Theme;
                    ((IMetroComponent)c.ContextMenuStrip).StyleManager = this;
                }

                if (c is TabControl)
                {
                    foreach (TabPage tp in ((TabControl)c).TabPages)
                    {
                        if (tp is IMetroControl)
                        {
                            ((IMetroControl)c).Style = Style;
                            ((IMetroControl)c).Theme = Theme;
                            ((IMetroControl)c).StyleManager = this;
                        }

                        if (tp.Controls.Count > 0)
                            UpdateControlCollection(tp.Controls);
                    }
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
