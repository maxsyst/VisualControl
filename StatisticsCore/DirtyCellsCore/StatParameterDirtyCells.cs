using System;
using System.Collections.Generic;

namespace VueExample.StatisticsCore.DirtyCellsCore
{
    public class StatParameterDirtyCells
    {
        public string StatParameterName { get; set; }
        public Dictionary<int, Limitation> Math { get; set; }
        public Dictionary<string, Limitation> Fixed { get; set; }

        public StatParameterDirtyCells(string name)
        {
            StatParameterName = name;
            Math = new Dictionary<int, Limitation>();
            Fixed = new Dictionary<string, Limitation>();
        }
    }
    
}