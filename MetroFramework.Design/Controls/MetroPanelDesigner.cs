using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using MetroFramework.Controls;

namespace MetroFramework.Design.Controls
{
    internal class MetroPanelDesigner : System.Windows.Forms.Design.ParentControlDesigner
    {
        public override void Initialize(System.ComponentModel.IComponent component)
        {
            base.Initialize(component);

            if (this.Control is MetroPanel)
            {
                //this.EnableDesignMode(((MetroPanel)this.Control).ScrollablePanel, "ScrollablePanel");
            }
        }
    }
}
