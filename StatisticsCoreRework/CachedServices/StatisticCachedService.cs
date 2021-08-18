using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using VueExample.Cache.Redis;
using VueExample.Models.SRV6;
using VueExample.StatisticsCoreRework.Abstract;
using VueExample.StatisticsCoreRework.Models;
using VueExample.StatisticsCoreRework.Services;

namespace VueExample.StatisticsCoreRework.CachedServices
{
    public class StatisticCachedService : IStatisticService
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly StatisticService _statisticService;
        public StatisticCachedService(ICacheProvider cacheProvider, StatisticService statisticService) 
            => (_cacheProvider, _statisticService) = (cacheProvider, statisticService);
        public async Task<Dictionary<string, Dictionary<string, SingleParameterStatisticValues>>> GetSingleParameterStatisticByDieValues(ConcurrentDictionary<string, List<DieValue>> dieValues, int measurementRecordingId)
        {
            var dict = await _cacheProvider.GetFromCache<Dictionary<string, Dictionary<string, SingleParameterStatisticValues>>>($"SPS:MEASUREMENTRECORDINGID:{measurementRecordingId}");
            if(dict is null) 
            {
                dict = await _statisticService.GetSingleParameterStatisticByDieValues(dieValues, measurementRecordingId);
                await _cacheProvider.SetCache<Dictionary<string, Dictionary<string, SingleParameterStatisticValues>>>($"SPS:MEASUREMENTRECORDINGID:{measurementRecordingId}", dict, new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(30)));
            }
            return dict;
        }
    }
}