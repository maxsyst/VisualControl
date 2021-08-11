using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using VueExample.Cache.Redis;
using VueExample.Entities;
using VueExample.Models.SRV6;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.Services;

namespace VueExample.Providers.Srv6.CachedServices
{
    public class DieValueCachedService : IDieValueService
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly DieValueService _dieValueService;
        public DieValueCachedService(ICacheProvider cacheProvider, DieValueService dieValueService)
        {
            _cacheProvider = cacheProvider;
            _dieValueService = dieValueService;
        }
        public async Task CreateDieGraphics(List<DieGraphics> dieGraphics)
        {
            await _dieValueService.CreateDieGraphics(dieGraphics);
        }

        public async Task<Dictionary<string, List<DieValue>>> GetDieValuesByMeasurementRecording(int measurementRecordingId)
        {
            var dieValueDictionary = await _cacheProvider.GetFromCache<Dictionary<string, List<DieValue>>>($"DIEVALUE:MEASUREMENTRECORDINGID:{measurementRecordingId}");
            if(dieValueDictionary is null) {
                dieValueDictionary = await _dieValueService.GetDieValuesByMeasurementRecording(measurementRecordingId);
                await _cacheProvider.SetCache<Dictionary<string, List<DieValue>>>($"DIEVALUE:MEASUREMENTRECORDINGID:{measurementRecordingId}", dieValueDictionary, new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(30)));
            }
            return dieValueDictionary;
        }

        public async Task<List<long?>> GetSelectedDiesByMeasurementRecordingId(int measurementRecordingId)
        {
            return await _dieValueService.GetSelectedDiesByMeasurementRecordingId(measurementRecordingId);
        }
    }
}