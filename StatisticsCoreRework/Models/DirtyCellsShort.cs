using System;
using System.Collections.Generic;
using System.Globalization;

namespace VueExample.StatisticsCoreRework.Models
{
    public class DirtyCellsShort
    {
        public string StatName { get; set; }
        public string Type { get; set; }
        public List<long> BadDirtyCells { get; set; } = new List<long>();
        public string GoodDiesPercentage { get; set; } = "0.0";
        public DirtyCellsShort()
        {
            
        }
        public DirtyCellsShort(string statName)
        {
            StatName = statName;
        }

        public DirtyCellsShort SetBadDies(List<long> badDiesId, int diesCount)
        {
            BadDirtyCells = badDiesId;
            GoodDiesPercentage = Convert.ToString(Math.Ceiling((1.0 - badDiesId.Count / (diesCount + 0.0)) * 100), CultureInfo.InvariantCulture);
            return this;
        }
    }
    
}