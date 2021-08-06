using System.Collections.Generic;

namespace VueExample.StatisticsCore.DirtyCellsCore
{
    public class GraphicDirtyCells
    {
        public string GraphicKey { get; set; }
        public List<StatParameterDirtyCells> statParameterDirtyCells { get; set; } = new List<StatParameterDirtyCells>();
    }
}