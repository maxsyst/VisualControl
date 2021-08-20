using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using VueExample.Cache.Redis;
using VueExample.StatisticsCoreRework.Abstract;
using VueExample.StatisticsCoreRework.Models;
using VueExample.StatisticsCoreRework.Services;

namespace VueExample.StatisticsCoreRework.CachedServices
{
    public class DirtyCellsCalculationFxdCachedService : IDirtyCellsCalculationFxdService
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly DirtyCellsCalculationFxdService _dirtyCellsCalculationFxdService;
        public DirtyCellsCalculationFxdCachedService(ICacheProvider cacheProvider, DirtyCellsCalculationFxdService dirtyCellsCalculationFxdService)
        {
            _cacheProvider = cacheProvider;
            _dirtyCellsCalculationFxdService = dirtyCellsCalculationFxdService;
        }

        public async Task<DirtyCellsShort> CalculateShort(int measurementRecordingId, string keyGraphicState, string lowBorder, string topBorder, SingleParameterStatisticValues singleParameterStatisticValues)
        {
           var dirtyCellsShort = await _cacheProvider.GetFromCache<DirtyCellsShort>($"MEASUREMENTRECORDINGID:{measurementRecordingId}:KGS:{keyGraphicState}:STATNAME:{singleParameterStatisticValues.StatisticName}:DC:FXD:LB:{lowBorder}:TB:{topBorder}");
           if(dirtyCellsShort  is null) {
                dirtyCellsShort = _dirtyCellsCalculationFxdService.CalculateShort(lowBorder, topBorder, singleParameterStatisticValues);
                await _cacheProvider.SetCache<DirtyCellsShort>($"MEASUREMENTRECORDINGID:{measurementRecordingId}:KGS:{keyGraphicState}:STATNAME:{singleParameterStatisticValues.StatisticName}:DC:FXD:LB:{lowBorder}:TB:{topBorder}", dirtyCellsShort, new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(30)));
            }
            return dirtyCellsShort;   
        }
    }
}