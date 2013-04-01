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
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using MetroFramework.Interfaces;

namespace MetroFramework.Components
{
    public class MetroStyleManager : Component, ICloneable, ISupportInitialize
    {
        public MetroStyleManager()
        {}

        //public MetroStyleManager(ContainerControl owner)
        //    : this()
        //{
        //    Owner = owner;
        //}

        /// <summary>
        ///     The Container owning components.
        /// </summary>
        private readonly IContainer _parent;

        public MetroStyleManager(IContainer parent)
            : this()
        {
            Debug.WriteLine("MetroStyleManager in IContainer");
            if (parent != null)
            {
                _parent = parent;
                parent.Add(this);
            }
        }

        #region ICloneable

        // What's the use case here?
        // Do we need to clone parent container, too?

        public object Clone()
        {
            MetroStyleManager newStyleManager = new MetroStyleManager();
            newStyleManager.metroTheme = Theme;
            newStyleManager.metroStyle = Style;
            //newStyleManager.owner = null;
            return newStyleManager;
        }

        #endregion

        #region ISupportInitialize

        /// <summary>
        ///     Defer propagating style information until all controls and components have ben added 
        ///     and all properties have been set.
        /// </summary>
        private bool _initializing;

        void ISupportInitialize.BeginInit()
        {
            _initializing = true;
        }

        void ISupportInitialize.EndInit()
        {
            _initializing = false;
            Refresh();
        }

        #endregion

        private ContainerControl owner;
        public ContainerControl Owner
        {
            get { return owner; }
            set
            {
                // We attach to ControlAdded to propagate styles to dynamically added controls

                if (owner != null) 
                {
                    owner.ControlAdded -= ControlAdded;
                }

                owner = value;

                if (value != null)
                {
                    owner.ControlAdded += ControlAdded;
                    if(!_initializing) UpdateControl(value);
                }
            }
        }

        private MetroColorStyle metroStyle = MetroColorStyle.Blue;
        [DefaultValue(MetroColorStyle.Blue)]
        [Category("Metro Appearance")]
        public MetroColorStyle Style
        {
            get { return metroStyle; }
            set 
            { 
                metroStyle = value;
                if (!_initializing) Refresh();
            }
        }

        private MetroThemeStyle metroTheme = MetroThemeStyle.Light;
        [DefaultValue(MetroThemeStyle.Light)]
        [Category("Metro Appearance")]
        public MetroThemeStyle Theme
        {
            get { return metroTheme; }
            set 
            {
                metroTheme = value;
                if (!_initializing) Refresh();
            }
        }

        private void ControlAdded(object sender, ControlEventArgs e)
        {
            Debug.Assert(e.Control!=null);
            if(!_initializing) UpdateControl(e.Control);
        }

        public void Refresh()
        {
            if(owner!=null) UpdateControl(owner);

            // propagate style information to components, i.e. MetroStyleExtender
            if (_parent == null || _parent.Components == null) return;
            foreach (IMetroComponent metroComponent in _parent.Components.OfType<IMetroComponent>())
                ApplyTheme(metroComponent);
        }

        private void UpdateControl(Control c)
        {
            if (c == null) return;

            IMetroControl metroControl = c as IMetroControl;
            if (metroControl != null ) ApplyTheme(metroControl);

            IMetroComponent metroComponent = c as IMetroComponent;
            if(metroComponent!=null) ApplyTheme(metroComponent);

            TabControl tabControl = c as TabControl;
            if (tabControl != null)
            {
                foreach (TabPage tp in ((TabControl)c).TabPages)
                    UpdateControl(tp);
            }

            if (c.Controls != null)
            {
                foreach (Control child in c.Controls)
                    UpdateControl(child);
            }

            if (c.ContextMenuStrip != null) UpdateControl(c.ContextMenuStrip);

            c.Refresh();
        }

        private void ApplyTheme(IMetroControl control)
        {
            control.Style = Style;
            control.Theme = Theme;
            control.StyleManager = this;
        }

        private void ApplyTheme(IMetroComponent component)
        {
            component.Style = Style;
            component.Theme = Theme;
            component.StyleManager = this;
        }

    }
}
