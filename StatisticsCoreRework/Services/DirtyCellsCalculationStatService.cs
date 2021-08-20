using System.Globalization;
using System;
using System.Linq;
using VueExample.StatisticsCoreRework.Models;
using System.Collections.Generic;

namespace VueExample.StatisticsCoreRework.Services
{
    public class DirtyCellsCalculationStatService
    {
        public DirtyCellsShort CalculateShort(string k, SingleParameterStatisticValues singleParameterStatisticValues)
        {
            return new DirtyCellsShortStat(singleParameterStatisticValues.StatisticName, k).SetBadDies(CalculateBadDies(k, singleParameterStatisticValues));
        }

        private List<long> CalculateBadDies(string k, SingleParameterStatisticValues singleParameterStatisticValues) 
        {
            var kDouble = Convert.ToDouble(k, CultureInfo.InvariantCulture);
            var points = singleParameterStatisticValues.DieStatDictionary.Values.Select(x => Convert.ToDouble(x, CultureInfo.InvariantCulture)).ToList();
            var quartile1Double = MathNet.Numerics.Statistics.Statistics.LowerQuartile(points);
            var quartile3Double = MathNet.Numerics.Statistics.Statistics.UpperQuartile(points);
            var iqr = MathNet.Numerics.Statistics.Statistics.InterquartileRange(points);
            return singleParameterStatisticValues.DieStatDictionary .ToDictionary(kv => kv.Key, kv => Convert.ToDouble(kv.Value, CultureInfo.InvariantCulture))
                                                                    .Where(x => x.Value < quartile1Double - kDouble * iqr && x.Value > quartile3Double + kDouble * iqr || Double.IsNaN(x.Value))
                                                                    .Select(kv => kv.Key)
                                                                    .ToList();
        }
    }
}