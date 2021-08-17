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
    public class SingleParameterCachedServiceLNR : ISingleParameterStatisticService
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly SingleParameterServiceLNR _singleParameterService;
        public SingleParameterCachedServiceLNR(ICacheProvider cacheProvider, SingleParameterServiceLNR singleParameterService)
        {
            _cacheProvider = cacheProvider;
            _singleParameterService = singleParameterService;
        }
        public async Task<ConcurrentDictionary<string, SingleParameterStatisticValues>> CreateSingleParameterStatisticsList(List<DieValue> dieValues, Graphic graphic, string keyGraphicState, int measurementRecordingId)
        {
            var dict = await _cacheProvider.GetFromCache<ConcurrentDictionary<string, SingleParameterStatisticValues>>($"SPS:MEASUREMENTRECORDINGID:{measurementRecordingId}:KGS:{keyGraphicState}");
            if(dict is null) 
            {
                dict = _singleParameterService.CreateSingleParameterStatisticsList(dieValues, graphic);
                await _cacheProvider.SetCache<ConcurrentDictionary<string, SingleParameterStatisticValues>>($"SPS:MEASUREMENTRECORDINGID:{measurementRecordingId}:KGS:{keyGraphicState}", dict, new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(30)));
            }
            return dict;
        }
    }
}