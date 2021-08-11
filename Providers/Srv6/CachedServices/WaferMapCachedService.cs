using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using VueExample.Cache.Redis;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.ViewModels;

namespace VueExample.Providers.Srv6.CachedServices
{
    public class WaferMapCachedService : IWaferMapService
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly WaferMapService _waferMapService;
        public WaferMapCachedService(ICacheProvider cacheProvider, WaferMapService waferMapService)
        {
            _cacheProvider = cacheProvider;
            _waferMapService = waferMapService;
        }
        public async Task<FormedMapViewModel> GetFormedMap(WaferMapFieldViewModel waferMapFieldViewModel)
        {
            var formedMap = await _cacheProvider.GetFromCache<FormedMapViewModel>($"FieldHeight:{waferMapFieldViewModel.FieldHeight}:FieldWidth:{waferMapFieldViewModel.FieldWidth}:StreetSize:{waferMapFieldViewModel.StreetSize}:WaferId:{waferMapFieldViewModel.WaferId}");
            if(formedMap is null) {
                formedMap = await _waferMapService.GetFormedMap(waferMapFieldViewModel);
                await _cacheProvider.SetCache<FormedMapViewModel>($"FieldHeight:{waferMapFieldViewModel.FieldHeight}:FieldWidth:{waferMapFieldViewModel.FieldWidth}:StreetSize:{waferMapFieldViewModel.StreetSize}:WaferId:{waferMapFieldViewModel.WaferId}", formedMap, new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(30)));
            }
            return formedMap;                                                     
        }
    }
}