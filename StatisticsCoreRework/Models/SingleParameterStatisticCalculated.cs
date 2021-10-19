using System.Linq;
using System;

namespace VueExample.StatisticsCoreRework.Models
{
    public class SingleParameterStatisticCalculated
    {
        public SingleParameterStatisticCalculated()
        {
        }
        public SingleParameterStatisticCalculated(string statisticsName, string shortStatisticsName, string unit)
        {
            this.StatisticsName = statisticsName;
            this.ShortStatisticsName = shortStatisticsName;
            this.Unit = unit;
            this.State = shortStatisticsName.Split("$$").ElementAtOrDefault(1) is null ? 0 : Convert.ToInt32(shortStatisticsName.Split("$$")[1].Split('A')[1]);
        }
        public SingleParameterStatisticCalculated(string expectedValue, string maximum, string minimum, string standartDeviation, string statisticsName, string shortStatisticsName, string unit, string median)
        {
            this.ExpectedValue = expectedValue;
            this.Maximum = maximum;
            this.Minimum = minimum;
            this.StandartDeviation = standartDeviation;
            this.StatisticsName = statisticsName;
            this.ShortStatisticsName = shortStatisticsName;
            this.Unit = unit;
            this.Median = median;
            this.State = shortStatisticsName.Split("$$").ElementAtOrDefault(1) is null ? 0 : Convert.ToInt32(shortStatisticsName.Split("$$")[1].Split('A')[1]);
        }
        public string ExpectedValue { get; set; } = "?";
        public string Maximum { get; set; } = "?";
        public string Minimum { get; set; } = "?";
        public string StandartDeviation { get; set; } = "?";
        public string StatisticsName { get; set; }
        public string ShortStatisticsName { get; set; }
        public string Unit { get; set; }
        public string Median { get; set; } = "?";
        public int State { get; set; } = 0;
    }
}