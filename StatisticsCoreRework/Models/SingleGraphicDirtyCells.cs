using System.Collections.Generic;

namespace VueExample.StatisticsCoreRework.Models
{
    public class SingleGraphicDirtyCells
    {
        public string KeyGraphicState { get; set; }
        public List<long> BadDies { get; set; } = new List<long>();
        public string GoodDiesPercentage { get; set; } = "0.0";
        public Dictionary<string, DirtyCellsShort> StatNameDirtyCellsDictionary { get; set; } = new Dictionary<string, DirtyCellsShort>();
    }
}