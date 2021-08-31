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
            return new SingleParameterStatisticCalculated {
                Minimum = CalculateMinimum(values),
                Maximum = CalculateMaximum(values),
                ExpectedValue = CalculateExpectedValue(values),
                Median = CalculateMedian(values),
                StandartDeviation = CalculateStandartDeviation(values),
                StatisticsName = statName,
                ShortStatisticsName = $"{statName.Split(' ').FirstOrDefault()}",
                Unit = unit
            };
        }

        private SingleParameterStatisticCalculated SingleParameterStatisticCalculatedBuilder(SingleParameterStatisticValues singleParameterStatisticValues, double divider) {
            var (statName, unit, dividerProfile, _, values) = singleParameterStatisticValues;
            if(dividerProfile == DividerProfile.WithoutDivider) 
            {
                return new SingleParameterStatisticCalculated {
                                                        Minimum = CalculateMinimum(values),
                                                        Maximum = CalculateMaximum(values),
                                                        ExpectedValue = CalculateExpectedValue(values),
                                                        Median = CalculateMedian(values),
                                                        StandartDeviation = CalculateStandartDeviation(values),
                                                        StatisticsName = statName,
                                                        ShortStatisticsName = $"{statName.Split(' ').FirstOrDefault()}",
                                                        Unit = unit
                                                    };
            }
            if(dividerProfile == DividerProfile.WithDivider) 
            {
                return new SingleParameterStatisticCalculated {
                                                        Minimum = CalculateMinimum(values, divider),
                                                        Maximum = CalculateMaximum(values, divider),
                                                        ExpectedValue = CalculateExpectedValue(values, divider),
                                                        Median = CalculateMedian(values, divider),
                                                        StandartDeviation = CalculateStandartDeviation(values, divider),
                                                        StatisticsName = statName,
                                                        ShortStatisticsName = $"{statName.Split(' ').FirstOrDefault()}",
                                                        Unit = $"{unit}/мм" 
                                                    };
            }
            if(dividerProfile == DividerProfile.ROnFamily) 
            {
                return new SingleParameterStatisticCalculated {
                                                        Minimum = CalculateMinimum(values, 1/divider),
                                                        Maximum = CalculateMaximum(values, 1/divider),
                                                        ExpectedValue = CalculateExpectedValue(values, 1/divider),
                                                        Median = CalculateMedian(values, 1/divider),
                                                        StandartDeviation = CalculateStandartDeviation(values, 1/divider),
                                                        StatisticsName = statName,
                                                        ShortStatisticsName = $"{statName.Split(' ').FirstOrDefault()}",
                                                        Unit = $"{unit}*мм" 
                                                    };
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