using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MetroFramework.Drawing;
using MetroFramework.Components;

namespace MetroFramework.Interfaces
{
    public interface IMetroControl
    {
        MetroColorStyle Style { get; set; }
        MetroThemeStyle Theme { get; set; }

        MetroStyleManager StyleManager { get; set; }
    }
}
