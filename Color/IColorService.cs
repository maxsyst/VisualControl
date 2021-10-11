using System.Collections.Generic;

namespace VueExample.Color
{
    public interface IColorService
    {
        string GetRandomHexColor();
        string GetHexColorByDieId (long? die);
        List<Color> GetGradientColors();
    }
}