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
