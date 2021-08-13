using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using VueExample.Cache.Redis;
using VueExample.Exceptions;
using VueExample.Models.SRV6;
using VueExample.StatisticsCore.Abstract;
using VueExample.StatisticsCore.DataModels;
using VueExample.StatisticsCore.DirtyCellsCore;
using VueExample.StatisticsCore.Services;

namespace VueExample.StatisticsCore.CachedService
{
    public class StatisticCachedService : IStatisticService, IStatisticCacheService
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

        public async Task<Dictionary<string, List<SingleParameterStatistic>>> GetSingleParameterStatisticByDieValues(ConcurrentDictionary<string, List<DieValue>> dieValues, int measurementRecordingId, int? stageId, double divider, double k)
        {
            var statDictionary = await _cacheProvider.GetFromCache<Dictionary<string, List<SingleParameterStatistic>>>($"SPSTAT:MEASUREMENTRECORDINGID:{measurementRecordingId}:FULL:K:{k}");            
            if(statDictionary is null) 
            {
                statDictionary = await _statisticService.GetSingleParameterStatisticByDieValues(dieValues, measurementRecordingId, stageId, divider, k);
                await _cacheProvider.SetCache<Dictionary<string, List<SingleParameterStatistic>>>($"SPSTAT:MEASUREMENTRECORDINGID:{measurementRecordingId}:FULL", statDictionary, new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(30)));
                var cacheTasks = new List<Task>();
                foreach (var stat in statDictionary)
                {
                    cacheTasks.Add(_cacheProvider.SetCache<List<SingleParameterStatistic>>($"SPSTAT:MEASUREMENTRECORDINGID:{measurementRecordingId}:KGS:{stat.Key}:K:{k}", stat.Value, new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(30))));
                }
                await Task.WhenAll(cacheTasks);
            }
            return statDictionary;
        }

        public async Task<List<SingleParameterStatistic>> GetSingleParameterStatisticByMeasurementRecordingIdAndKeyGraphicState(int measurementRecordingId, double k, string keyGraphicState)
        {
            var singleParameterStatisticList = await _cacheProvider.GetFromCache<List<SingleParameterStatistic>>($"SPSTAT:MEASUREMENTRECORDINGID:{measurementRecordingId}:KGS:{keyGraphicState}:K:{k}");           
            if(singleParameterStatisticList is null) 
            {
                throw new CollectionIsEmptyException();
            } 
            return singleParameterStatisticList;
        }

        public async Task<List<SingleStatisticData>> GetStatisticsDataByGraphicState(List<long?> dieList, string keyGraphicState, List<DieValue> dieValuesList, double divider, List<SingleParameterStatistic> singleParameterStatisticsList)
        {
            return await _statisticService.GetStatisticsDataByGraphicState(dieList, keyGraphicState, dieValuesList, divider, singleParameterStatisticsList);
        }
    }
}