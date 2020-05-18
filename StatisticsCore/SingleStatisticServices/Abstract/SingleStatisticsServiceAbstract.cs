using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using VueExample.Models.SRV6;

namespace VueExample.StatisticsCore.SingleStatisticServices.Abstract
{
    public abstract class SingleStatisticsServiceAbstract
    {
        public abstract List<VueExample.StatisticsCore.DataModels.SingleStatisticData> CreateSingleStatisticData(List<long?> dieList, Graphic graphic, List<DieValue> dieValuesList, double divider, List<VueExample.StatisticsCore.SingleParameterStatistic> singleParameterStatisticsList);
        public abstract List<VueExample.StatisticsCore.SingleParameterStatistic> CreateSingleParameterStatisticsList(List<DieValue> dieValues, Graphic graphic, int? stageId, double divider);
        protected List<VueExample.StatisticsCore.DataModels.SingleStatisticData> StatisticDataMapping (List<Statistics> statisticList, List<long?> dieList, List<VueExample.StatisticsCore.SingleParameterStatistic> singleParameterStatisticList)
        {
           
           var singleStatisticDataList = new List<VueExample.StatisticsCore.DataModels.SingleStatisticData>();
           foreach (var statisticsItem in statisticList)
           {
                var originDirtyCells = singleParameterStatisticList.FirstOrDefault(x => x.Name == statisticsItem.StatisticsName).DirtyCells;
              
                var singleStatisticData = new DataModels.SingleStatisticData();
                var singleParameterStatistic = new SingleParameterStatistic(statisticsItem.StatisticsName, dieList, statisticsItem.FullList, originDirtyCells);
                //Mapping SingleParameterStatistic
                singleStatisticData.LowBorderFixed = Convert.ToString(singleParameterStatistic.LowBorderFixed, CultureInfo.InvariantCulture);
                singleStatisticData.LowBorderStat = singleParameterStatistic.LowBorderStat;
                singleStatisticData.TopBorderFixed = Convert.ToString(singleParameterStatistic.TopBorderFixed, CultureInfo.InvariantCulture);
                singleStatisticData.TopBorderStat = singleParameterStatistic.TopBorderStat;
                singleStatisticData.AverageFixed = Convert.ToString(singleParameterStatistic.AverageFixed, CultureInfo.InvariantCulture);
                singleStatisticData.DirtyCells = singleParameterStatistic.DirtyCells.CalculatePercentage(dieList.Count);
                //Mapping Statistic
                singleStatisticData.Maximum = statisticsItem.Maximum;
                singleStatisticData.Minimum = statisticsItem.Minimum;
                singleStatisticData.Median = statisticsItem.Median;
                singleStatisticData.StandartDeviation = statisticsItem.StandartDeviation;
                singleStatisticData.ParameterID = statisticsItem.ParameterID;
                singleStatisticData.Unit = statisticsItem.Unit;
                singleStatisticData.ExpectedValue = statisticsItem.ExpectedValue;
                singleStatisticData.StatisticsName = statisticsItem.StatisticsName;
                singleStatisticData.ShortStatisticsName = $"{statisticsItem.StatisticsName.Split(' ').FirstOrDefault()}";
                singleStatisticDataList.Add(singleStatisticData);

           } 
           return singleStatisticDataList;

        }

         

        
    }
}