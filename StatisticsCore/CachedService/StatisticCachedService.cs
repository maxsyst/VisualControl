using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Cache.Redis;
using VueExample.Models.SRV6;
using VueExample.StatisticsCore.Abstract;
using VueExample.StatisticsCore.DataModels;
using VueExample.StatisticsCore.DirtyCellsCore;
using VueExample.StatisticsCore.Services;

namespace VueExample.StatisticsCore.CachedService
{
    public class StatisticCachedService : IStatisticService
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly StatisticService _statisticService;
        public StatisticCachedService(ICacheProvider cacheProvider, StatisticService statisticService)
        {
            _cacheProvider = cacheProvider;
            _statisticService = statisticService;
        }
        public DirtyCells GetDirtyCellsBySPSDictionary(ConcurrentDictionary<string, List<SingleParameterStatistic>> singleParameterStatistics, int diesCount)
        {
            return _statisticService.GetDirtyCellsBySPSDictionary(singleParameterStatistics, diesCount);
        }

        public List<GraphicDirtyCells> GetGraphicDirtyCells(Dictionary<string, List<SingleParameterStatistic>> singleParameterStatistics, List<KurbatovParameterModel> kurbatovParameterList)
        {
             return _statisticService.GetGraphicDirtyCells(singleParameterStatistics, kurbatovParameterList);
        }

        public async Task<Dictionary<string, List<SingleParameterStatistic>>> GetSingleParameterStatisticByDieValues(ConcurrentDictionary<string, List<DieValue>> dieValues, int? stageId, double divider, double k)
        {
            return await _statisticService.GetSingleParameterStatisticByDieValues(dieValues, stageId, divider, k);
        }

        public async Task<List<SingleStatisticData>> GetStatisticsDataByGraphicState(List<long?> dieList, string keyGraphicState, List<DieValue> dieValuesList, double divider, List<SingleParameterStatistic> singleParameterStatisticsList)
        {
            return await _statisticService.GetStatisticsDataByGraphicState(dieList, keyGraphicState, dieValuesList, divider, singleParameterStatisticsList);
        }
    }
}