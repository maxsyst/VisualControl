using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using VueExample.Cache.Redis;
using VueExample.StatisticsCoreRework.Abstract;
using VueExample.StatisticsCoreRework.Models;
using VueExample.StatisticsCoreRework.Services;

namespace VueExample.StatisticsCoreRework.CachedServices
{
    public class DirtyCellsCachedService : IDirtyCellsService
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly DirtyCellsService _dirtyCellsService;
        public DirtyCellsCachedService(ICacheProvider cacheProvider, DirtyCellsService dirtyCellsService)
        {
            _cacheProvider = cacheProvider;
            _dirtyCellsService = dirtyCellsService;
        }
        public async Task<MeasurementRecordingDirtyCellsSnapshot> GetDirtyCellsInitialSnapShotByMeasurementRecordingId(int measurementRecordingId)
        {
            var snapshot = await _cacheProvider.GetFromCache<MeasurementRecordingDirtyCellsSnapshot>($"MEASUREMENTRECORDINGID:{measurementRecordingId}:DCSNAPSHOT");
            if(snapshot is null)
            {
                snapshot = await _dirtyCellsService.GetDirtyCellsInitialSnapShotByMeasurementRecordingId(measurementRecordingId);
                await _cacheProvider.SetCache<MeasurementRecordingDirtyCellsSnapshot>($"MEASUREMENTRECORDINGID:{measurementRecordingId}:DCSNAPSHOT", snapshot, new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(30)));
            }
            return snapshot;
        }

        public async Task<SingleGraphicDirtyCells> GetDirtyCellsShortsByKeyGraphicState(int measurementRecordingId, string keyGraphicState, List<DirtyCellsProfile> dirtyCellsProfiles)
        {
           return await _dirtyCellsService.GetDirtyCellsShortsByKeyGraphicState(measurementRecordingId, keyGraphicState, dirtyCellsProfiles);
        }
    }
}