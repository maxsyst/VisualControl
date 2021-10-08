using System;
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
    public class SingleParameterCachedServiceGR4 : ISingleParameterStatisticService
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly SingleParameterServiceGR4 _singleParameterService;
        public SingleParameterCachedServiceGR4(ICacheProvider cacheProvider, SingleParameterServiceGR4 singleParameterService)
        {
            _cacheProvider = cacheProvider;
            _singleParameterService = singleParameterService;
        }

        public async Task<Dictionary<string, SingleParameterStatisticValues>> CreateSingleParameterStatisticsList(List<DieValue> dieValues, Graphic graphic, string keyGraphicState, int measurementRecordingId)
        {
            var dict = await _cacheProvider.GetFromCache<Dictionary<string, SingleParameterStatisticValues>>($"SPS:MEASUREMENTRECORDINGID:{measurementRecordingId}:KGS:{keyGraphicState}");
            if(dict is null) 
            {
                dict = _singleParameterService.CreateSingleParameterStatisticsList(dieValues, graphic, keyGraphicState);
                await _cacheProvider.SetCache<Dictionary<string, SingleParameterStatisticValues>>($"SPS:MEASUREMENTRECORDINGID:{measurementRecordingId}:KGS:{keyGraphicState}", dict, new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(30)));
            }
            return dict;
        }
    }
}