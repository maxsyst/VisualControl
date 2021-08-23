using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using VueExample.Cache.Redis;
using VueExample.StatisticsCoreRework.Abstract;
using VueExample.StatisticsCoreRework.Models;
using VueExample.StatisticsCoreRework.Services;

namespace VueExample.StatisticsCoreRework.CachedServices
{
    public class DirtyCellsCalculationStatCachedService : IDirtyCellsCalculationStatService
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly DirtyCellsCalculationStatService  _dirtyCellsCalculationStatService;
        public DirtyCellsCalculationStatCachedService(ICacheProvider cacheProvider, DirtyCellsCalculationStatService dirtyCellsCalculationStatService)
        {
            _cacheProvider = cacheProvider;
            _dirtyCellsCalculationStatService = dirtyCellsCalculationStatService;
        }

        public async Task<DirtyCellsShort> CalculateShort(int measurementRecordingId, string keyGraphicState, string k, SingleParameterStatisticValues singleParameterStatisticValues)
        {
            var dirtyCellsShort = await _cacheProvider.GetFromCache<DirtyCellsShort>($"MEASUREMENTRECORDINGID:{measurementRecordingId}:KGS:{keyGraphicState}:STATNAME:{singleParameterStatisticValues.StatisticName}:DC:STAT:K:{k}");
            if(dirtyCellsShort is null) {
                dirtyCellsShort = _dirtyCellsCalculationStatService.CalculateShort(k, singleParameterStatisticValues);
                await _cacheProvider.SetCache<DirtyCellsShort>($"MEASUREMENTRECORDINGID:{measurementRecordingId}:KGS:{keyGraphicState}:STATNAME:{singleParameterStatisticValues.StatisticName}:DC:STAT:K:{k}", dirtyCellsShort, new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(30)));
            }
            return dirtyCellsShort;
        }
    }
}