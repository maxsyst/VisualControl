using System;
using System.Collections.Generic;
using System.Globalization;

namespace VueExample.StatisticsCoreRework.Models
{
    public class DirtyCellsShort
    {
        public string StatName { get; set; }
        public string Type { get; set; }
        public string LowBorder { get; set; } = "0.0";
        public string TopBorder { get; set; } = "0.0";
        public List<long> BadDirtyCells { get; set; } = new List<long>();
        public string GoodDiesPercentage { get; set; } = "0.0";
        public DirtyCellsShort()
        {
        }
        public DirtyCellsShort(string statName, string lowBorder, string topBorder)
        {
            StatName = statName;
            LowBorder = lowBorder;
            TopBorder = topBorder;
        }

        public DirtyCellsShort SetBadDies(List<long> badDiesId, int diesCount)
        {
            BadDirtyCells = badDiesId;
            GoodDiesPercentage = Convert.ToString(Math.Ceiling((1.0 - badDiesId.Count / (diesCount + 0.0)) * 100), CultureInfo.InvariantCulture);
            return this;
        }
    }
}