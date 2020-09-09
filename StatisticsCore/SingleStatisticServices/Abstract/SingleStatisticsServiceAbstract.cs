using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VueExample.Models.SRV6;

namespace VueExample.StatisticsCore.SingleStatisticServices.Abstract
{
    public abstract class SingleStatisticsServiceAbstract
    {
        public abstract List<VueExample.StatisticsCore.DataModels.SingleStatisticData> CreateSingleStatisticData(List<long?> dieList, Graphic graphic, ConcurrentDictionary<long?, DieValue> dieValuesList, double divider, List<VueExample.StatisticsCore.SingleParameterStatistic> singleParameterStatisticsList);
        public abstract List<VueExample.StatisticsCore.SingleParameterStatistic> CreateSingleParameterStatisticsList(List<DieValue> dieValues, Graphic graphic, List<StatParameterForStage> statParameterForStage, double divider, double k);
        protected List<VueExample.StatisticsCore.DataModels.SingleStatisticData> StatisticDataMapping(List<Statistics> statisticList, List<long?> dieList, List<VueExample.StatisticsCore.SingleParameterStatistic> singleParameterStatisticList)
        {           
           var singleStatisticDataList = new ConcurrentDictionary<string, VueExample.StatisticsCore.DataModels.SingleStatisticData>();
           Parallel.ForEach (statisticList, statisticsItem => 
           {
                var origin = singleParameterStatisticList.FirstOrDefault(x => x.Name == statisticsItem.StatisticsName); 
                var singleStatisticData = new DataModels.SingleStatisticData();
                var singleParameterStatistic = new SingleParameterStatistic(statisticsItem.StatisticsName, dieList, statisticsItem.FullList, origin.DirtyCells);
                //Mapping SingleParameterStatistic
                singleStatisticData.LowBorderFixed = origin.LowBorderFixed;
                singleStatisticData.LowBorderStat = origin.LowBorderStat;
                singleStatisticData.TopBorderFixed = origin.TopBorderFixed;
                singleStatisticData.TopBorderStat = origin.TopBorderStat;
                singleStatisticData.AverageFixed = origin.AverageFixed;
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
                singleStatisticDataList.TryAdd(statisticsItem.StatisticsName, singleStatisticData);

           });
           return singleStatisticDataList.Values.ToList();

        }

         

        
    }
}