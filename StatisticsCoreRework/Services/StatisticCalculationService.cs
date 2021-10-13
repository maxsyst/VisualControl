using System;
using System.Collections.Generic;
using System.Linq;
using VueExample.StatisticsCoreRework.Abstract;
using VueExample.StatisticsCoreRework.Models;

namespace VueExample.StatisticsCoreRework.Services
{
    public class StatisticCalculationService : IStatisticCalculationService
    {
        public SingleParameterStatisticCalculated Calculate(SingleParameterStatisticValues singleParameterStatisticValues, double divider)
        {
            var (statName, unit, dividerProfile, _, values) = singleParameterStatisticValues;
            return SingleParameterStatisticCalculatedBuilder(singleParameterStatisticValues, divider);
        }

        private SingleParameterStatisticCalculated SingleParameterStatisticCalculatedBuilder(SingleParameterStatisticValues singleParameterStatisticValues, double divider) {
            var (statName, unit, dividerProfile, _, values) = singleParameterStatisticValues;
            if(dividerProfile == DividerProfile.WithoutDivider || Math.Abs(divider - 1.0) < 1E-6) 
            {
                return new SingleParameterStatisticCalculated(expectedValue: CalculateExpectedValue(values), 
                                                              maximum: CalculateMaximum(values), 
                                                              minimum: CalculateMinimum(values),
                                                              standartDeviation: CalculateStandartDeviation(values),
                                                              statisticsName: statName,
                                                              shortStatisticsName: $"{statName.Split(' ').FirstOrDefault()}",
                                                              unit: unit,
                                                              median: CalculateMedian(values));
            }
            if(dividerProfile == DividerProfile.WithDivider) 
            {
                 return new SingleParameterStatisticCalculated(expectedValue: CalculateExpectedValue(values, divider),
                                                              maximum: CalculateMaximum(values, divider),
                                                              minimum: CalculateMinimum(values, divider),
                                                              standartDeviation: CalculateStandartDeviation(values, divider),
                                                              statisticsName: statName,
                                                              shortStatisticsName: $"{statName.Split(' ').FirstOrDefault()}",
                                                              unit: $"{unit}/мм",
                                                              median: CalculateMedian(values, divider));
            }
            if(dividerProfile == DividerProfile.ROnFamily) 
            {
                return new SingleParameterStatisticCalculated(expectedValue: CalculateExpectedValue(values, 1/divider),
                                                              maximum: CalculateMaximum(values, 1/divider),
                                                              minimum: CalculateMinimum(values, 1/divider),
                                                              standartDeviation: CalculateStandartDeviation(values, 1/divider),
                                                              statisticsName: statName,
                                                              shortStatisticsName: $"{statName.Split(' ').FirstOrDefault()}",
                                                              unit: $"{unit}*мм",
                                                              median: CalculateMedian(values, 1/divider));
            }
            return new SingleParameterStatisticCalculated();
        }
        private string ToStringD(double d)
        {
            if ((Math.Abs(d) >= 10000 || Math.Abs(d) < 1E-2) && Math.Abs(d - 0) > 1E-20)
            {
                return d.ToString("0.00E0");
            }
            return d.ToString("0.000");
        }
        private string CalculateMinimum(List<double> list, double divider = 1.0) => list.Count == 0 ? String.Empty : ToStringD(list.Min() / divider);
        private string CalculateMaximum(List<double> list, double divider = 1.0) => list.Count == 0 ? String.Empty : ToStringD(list.Max() / divider);
        private string CalculateExpectedValue(List<double> list, double divider = 1.0) => list.Count == 0 ? String.Empty : ToStringD(list.Average() / divider);
        private string CalculateMedian(List<double> list, double divider = 1.0) => list.Count == 0 ? String.Empty : ToStringD(MathNet.Numerics.Statistics.Statistics.Median(list) / divider);

        private string CalculateStandartDeviation(List<double> list, double divider = 1.0)
        {
            if (list.Count == 0)
            {
                return String.Empty;
            }
            var standartdeviation = MathNet.Numerics.Statistics.Statistics.StandardDeviation(list) / divider;
            if (Double.IsNaN(standartdeviation))
            {
                standartdeviation = 0.0;
            }
            return ToStringD(standartdeviation);
        }

    }
}