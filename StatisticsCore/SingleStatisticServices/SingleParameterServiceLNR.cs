
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VueExample.Models.SRV6;
using VueExample.Providers.Srv6;
using VueExample.StatisticsCore.DataModels;

namespace VueExample.StatisticsCore.SingleStatisticServices
{
    public class SingleParameterServiceLNR : VueExample.StatisticsCore.SingleStatisticServices.Abstract.SingleStatisticsServiceAbstract
    {
        private readonly StatParameterService _statisticService;
        public SingleParameterServiceLNR(StatParameterService statisticService)
        {
            _statisticService = statisticService;
        }
        public override List<SingleParameterStatistic> CreateSingleParameterStatisticsList(List<DieValue> dieValues, Graphic graphic, int? stageId, double divider, double k)
        {
            var dieCommonListDictionary = new ConcurrentDictionary<long?, List<string>>();
            var statisticsItem = new Statistics();
            var xList = dieValues.FirstOrDefault().XList;
            var singleParameterStatisticsList = new List<SingleParameterStatistic>();
            Parallel.ForEach(dieValues, gdv => 
            {
                dieCommonListDictionary.TryAdd(gdv.DieId, gdv.YList);
            });         
            var statisticList = statisticsItem.GetStatistics(dieValues.FirstOrDefault().XList, dieCommonListDictionary.Values.ToList(), graphic, divider);
            Parallel.ForEach(statisticList, async stat => 
            {
                singleParameterStatisticsList.Add(new SingleParameterStatistic(stat.StatisticsName, dieCommonListDictionary.Keys.ToList(), stat.FullList, k).CalculateDirtyCellsFixed(await _statisticService.GetByStatParameterIdAndStageId(stat.ParameterID, stageId)));
            });
            return singleParameterStatisticsList;
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