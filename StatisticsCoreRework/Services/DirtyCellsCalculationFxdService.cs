using System.Globalization;
using System;
using System.Threading.Tasks;
using VueExample.StatisticsCoreRework.Models;
using System.Linq;
using System.Collections.Generic;

namespace VueExample.StatisticsCoreRework.Services
{
    public class DirtyCellsCalculationFxdService
    {
        public DirtyCellsShort CalculateShort(string lowBorder, string topBorder, SingleParameterStatisticValues singleParameterStatisticValues)
        {
            return new DirtyCellsShortFxd(singleParameterStatisticValues.StatisticName, lowBorder, topBorder)
                       .SetBadDies(CalculateBadDies(lowBorder, topBorder, singleParameterStatisticValues), singleParameterStatisticValues.DieStatDictionary.Keys.Count);
        }

        private List<long> CalculateBadDies(string lowBorder, string topBorder, SingleParameterStatisticValues singleParameterStatisticValues) 
        {
            var lowBorderDouble = Convert.ToDouble(lowBorder, CultureInfo.InvariantCulture);
            var topBorderDouble = Convert.ToDouble(topBorder, CultureInfo.InvariantCulture);
            return singleParameterStatisticValues.DieStatDictionary .ToDictionary(k => k.Key, k => Convert.ToDouble(k.Value, CultureInfo.InvariantCulture))
                                                                    .Where(x => x.Value < lowBorderDouble || x.Value > topBorderDouble || Double.IsNaN(x.Value))
                                                                    .Select(k => k.Key)
                                                                    .ToList();
        }
    }
}