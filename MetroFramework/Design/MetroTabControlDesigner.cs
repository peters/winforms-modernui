using System.Collections;
using System.Windows.Forms.Design;

namespace MetroFramework.Design
{

    internal class MetroTabControlDesigner : ControlDesigner
    {
        public override SelectionRules SelectionRules
        {
            get
            {
                return SelectionRules.AllSizeable;
            }
        }
        protected override void PreFilterProperties(IDictionary properties)
        {
            properties.Remove("Appearance");
            properties.Remove("Multiline"); // Todo: Implement me
            properties.Remove("HotTrack");  // Todo: Implement me

            properties.Remove("ImeMode");
            properties.Remove("Padding");
            properties.Remove("FlatAppearance");
            properties.Remove("FlatStyle");
            properties.Remove("AutoEllipsis");
            properties.Remove("UseCompatibleTextRendering");            

            properties.Remove("Image");
            properties.Remove("ImageAlign");
            properties.Remove("ImageIndex");
            properties.Remove("ImageKey");
            properties.Remove("ImageList");
            properties.Remove("TextImageRelation");
            
            properties.Remove("BackColor");
            properties.Remove("BackgroundImage");
            properties.Remove("BackgroundImageLayout");
            properties.Remove("UseVisualStyleBackColor");

            properties.Remove("Font");
            properties.Remove("ForeColor");
            properties.Remove("RightToLeft");

            base.PreFilterProperties(properties);
        }
    }
}
