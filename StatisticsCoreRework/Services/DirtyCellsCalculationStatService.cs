using System.Globalization;
using System;
using System.Linq;
using VueExample.StatisticsCoreRework.Models;
using System.Collections.Generic;
using VueExample.Extensions;

namespace VueExample.StatisticsCoreRework.Services
{
    public class DirtyCellsCalculationStatService
    {
        private string _calculatedLowBorder;
        private string _calculatedTopBorder;
        public DirtyCellsShort CalculateShort(string k, SingleParameterStatisticValues singleParameterStatisticValues)
        {
            var calculated = CalculateBadDies(k, singleParameterStatisticValues);
            return new DirtyCellsShortStat(singleParameterStatisticValues.StatisticName, _calculatedLowBorder, _calculatedTopBorder, k).SetBadDies(calculated, singleParameterStatisticValues.DieStatDictionary.Keys.Count);
        }

        private List<long> CalculateBadDies(string k, SingleParameterStatisticValues singleParameterStatisticValues)
        {
            var kDouble = Convert.ToDouble(k, CultureInfo.InvariantCulture);
            var points = singleParameterStatisticValues.DieStatDictionary.Values.Select(x => Convert.ToDouble(x, CultureInfo.InvariantCulture)).ToList();
            var quartile1Double = MathNet.Numerics.Statistics.Statistics.LowerQuartile(points);
            var quartile3Double = MathNet.Numerics.Statistics.Statistics.UpperQuartile(points);
            var iqr = MathNet.Numerics.Statistics.Statistics.InterquartileRange(points);
            _calculatedLowBorder = (quartile1Double - kDouble * iqr).ToGoodFormat();
            _calculatedTopBorder = (quartile3Double + kDouble * iqr).ToGoodFormat();
            return singleParameterStatisticValues.DieStatDictionary .ToDictionary(kv => kv.Key, kv => Convert.ToDouble(kv.Value, CultureInfo.InvariantCulture))
                                                                    .Where(x => x.Value < quartile1Double - (kDouble * iqr) || x.Value > quartile3Double + (kDouble * iqr) || Double.IsNaN(x.Value))
                                                                    .Select(kv => kv.Key)
                                                                    .ToList();
        }
    }
}