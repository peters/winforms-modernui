using MetroFramework.Components;

namespace MetroFramework.Interfaces
{
    public interface IMetroForm
    {
        MetroColorStyle Style { get; set; }
        MetroThemeStyle Theme { get; set; }

        MetroStyleManager StyleManager { get; set; }
    }
}
