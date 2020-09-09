
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VueExample.Models.SRV6;
using VueExample.StatisticsCore.DataModels;

namespace VueExample.StatisticsCore.SingleStatisticServices
{
    public class SingleParameterServiceLNR : VueExample.StatisticsCore.SingleStatisticServices.Abstract.SingleStatisticsServiceAbstract
    {
        public SingleParameterServiceLNR()
        {

        }
        public override List<SingleParameterStatistic> CreateSingleParameterStatisticsList(List<DieValue> dieValues, Graphic graphic, List<StatParameterForStage> statParameterForStage, double divider, double k)
        {
            var dieCommonListDictionary = new ConcurrentDictionary<long?, List<string>>();
            var statisticsItem = new Statistics();
            var xList = dieValues.FirstOrDefault().XList;
            var singleParameterStatisticsList = new ConcurrentDictionary<string, SingleParameterStatistic>();
            Parallel.ForEach(dieValues, gdv => 
            {
                dieCommonListDictionary.TryAdd(gdv.DieId, gdv.YList);
            });         
            var statisticList = statisticsItem.GetStatistics(dieValues.FirstOrDefault().XList, dieCommonListDictionary.Values.ToList(), graphic, divider);
            Parallel.ForEach(statisticList, stat => 
            {
                if(stat.ParameterID > 0) 
                {
                    singleParameterStatisticsList.TryAdd(stat.StatisticsName, new SingleParameterStatistic(stat.StatisticsName, dieCommonListDictionary.Keys.ToList(), stat.FullList, k).CalculateDirtyCellsFixed(statParameterForStage.FirstOrDefault(x => x.StatisticParameterId == stat.ParameterID )));
                }
                else
                {
                    singleParameterStatisticsList.TryAdd(stat.StatisticsName, new SingleParameterStatistic(stat.StatisticsName, dieCommonListDictionary.Keys.ToList(), stat.FullList, k));
                }
            });
            return singleParameterStatisticsList.Values.ToList();
        }

        public override List<SingleStatisticData> CreateSingleStatisticData(List<long?> dieIdList, Graphic graphic, ConcurrentDictionary<long?, DieValue> dieValuesList, double divider, List<VueExample.StatisticsCore.SingleParameterStatistic> singleParameterStatisticsList)
        {
            var dieCommonListDictionary = new ConcurrentDictionary<long?, List<string>>();
            var statisticsItem = new Statistics();
            var xList = dieValuesList.FirstOrDefault().Value.XList;
            Parallel.ForEach(dieIdList, dieId => 
            {
                dieCommonListDictionary.TryAdd(dieId, dieValuesList[dieId].YList);
            });         
            var statistics = statisticsItem.GetStatistics(xList, dieCommonListDictionary.Values.ToList(), graphic, divider);
            var singleStatisticDataList = StatisticDataMapping(statistics, dieCommonListDictionary.Keys.ToList(), singleParameterStatisticsList);
            return singleStatisticDataList;
        }
    }
}