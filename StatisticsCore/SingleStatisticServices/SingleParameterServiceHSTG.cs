using System.Linq;
using System.Collections.Generic;
using VueExample.Models.SRV6;
using VueExample.StatisticsCore.DataModels;
using VueExample.Providers.Srv6;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace VueExample.StatisticsCore.SingleStatisticServices
{
    public class SingleParameterServiceHSTG : VueExample.StatisticsCore.SingleStatisticServices.Abstract.SingleStatisticsServiceAbstract
    {
        private readonly StatParameterService _statisticService;
        public SingleParameterServiceHSTG(StatParameterService statisticService)
        {
            _statisticService = statisticService;
        }
        public override List<SingleParameterStatistic> CreateSingleParameterStatisticsList(List<DieValue> dieValues, Graphic graphic, int? stageId, double divider, double k)
        {
            var dieCommonListDictionary = new ConcurrentDictionary<long?, string>();
            var statisticsItem = new Statistics();
            var xList = dieValues.FirstOrDefault().XList;
            var singleParameterStatisticsList = new List<SingleParameterStatistic>();
            Parallel.ForEach(dieValues, gdv => 
            {
                dieCommonListDictionary.TryAdd(gdv.DieId, gdv.YList.FirstOrDefault());
            });         
            var statisticList = statisticsItem.GetStatistics(dieCommonListDictionary.Values.ToList(), graphic);
            Parallel.ForEach(statisticList, async stat => 
            {
                singleParameterStatisticsList.Add(new SingleParameterStatistic(stat.StatisticsName, dieCommonListDictionary.Keys.ToList(), stat.FullList, k).CalculateDirtyCellsFixed(await _statisticService.GetByStatParameterIdAndStageId(stat.ParameterID, stageId)));
            });
            return singleParameterStatisticsList;
          
        }

        public override List<SingleStatisticData> CreateSingleStatisticData(List<long?> dieIdList, Graphic graphic, ConcurrentDictionary<long?, DieValue> dieValuesList, double divider, List<VueExample.StatisticsCore.SingleParameterStatistic> singleParameterStatisticsList)
        {
            var statisticsItem = new Statistics();
            var selectedDieList = new List<long?>();
            var valueList = new List<string>();
            foreach (var dieId in dieIdList)
            {
                valueList.Add (dieValuesList[dieId].YList.FirstOrDefault());
                selectedDieList.Add(dieId);
            }
            var statistics = statisticsItem.GetStatistics(valueList, graphic);
            var singleStatisticDataList = StatisticDataMapping(statistics, selectedDieList, singleParameterStatisticsList);
            return singleStatisticDataList;
        }
    }
}