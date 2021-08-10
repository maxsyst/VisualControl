using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VueExample.Providers.Srv6.Interfaces;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using VueExample.Models.SRV6;
using VueExample.StatisticsCore.Abstract;
using LazyCache;
using Microsoft.Extensions.Caching.Distributed;

namespace VueExample.Controllers
{
    [Route("api/[controller]")]
    public class DirtyCellsController : Controller
    {
        private readonly IAppCache _cache;
        private readonly IDieValueService _dieValueService;
        private readonly IKurbatovParameterService _kurbatovParameterService;
        private readonly IWaferProvider _waferProvider;
        private readonly IStageProvider _stageProvider;
        private readonly IElementService _elementService;
        private readonly IDieTypeProvider _dieTypeProvider;
        private readonly IStandartPatternService _standartPatternService;
        private readonly IStatisticService _statisticService;
        private readonly IStandartMeasurementPatternService _standartMeasurementPatternService;
        private readonly Cache.Redis.ICacheProvider _cacheProvider;
        public DirtyCellsController(IAppCache cache, Cache.Redis.ICacheProvider cacheProvider, IStandartMeasurementPatternService standartMeasurementPatternService, IElementService elementService, IDieValueService dieValueService, IStageProvider stageProvider, IStandartPatternService standartPatternService, IDieTypeProvider dieTypeProvider, IKurbatovParameterService kurbatovParameterService, IWaferProvider waferProvider, IStatisticService statisticService)
        {
            _cacheProvider = cacheProvider;
            _cache = cache;
            _kurbatovParameterService = kurbatovParameterService;
            _dieValueService = dieValueService;
            _stageProvider = stageProvider;
            _statisticService = statisticService;
            _waferProvider = waferProvider;
            _dieTypeProvider = dieTypeProvider;
            _standartPatternService = standartPatternService;
            _standartMeasurementPatternService = standartMeasurementPatternService;
            _elementService = elementService;
        }

        [HttpGet]
        [Route("GetDirtyCellsByMeasurementRecordingStandart/{measurementRecordingId:int}")]
        public async Task<IActionResult> GetByMeasurementRecording ([FromRoute] int measurementRecordingId)
        {
            string measurementRecordingIdAsKey = Convert.ToString(measurementRecordingId);
            var k = 1.5;
            var dc = await _cacheProvider.GetFromCache<List<StatisticsCore.DirtyCellsCore.GraphicDirtyCells>>($"DC{measurementRecordingIdAsKey}");
            var wafer = await _waferProvider.GetByMeasurementRecordingId(measurementRecordingId);
            var stageId = (await _stageProvider.GetByMeasurementRecordingId(measurementRecordingId)).StageId;
            var dieType = (await _dieTypeProvider.GetByWaferId(wafer.WaferId)).FirstOrDefault();
            var standartPattern = await _standartPatternService.GetByDieTypeId(dieType.DieTypeId);
            var element = await _elementService.GetByIdmr(measurementRecordingId);
            var smp = await _standartMeasurementPatternService.GetByStageAndElementAndPattern(stageId, element.FirstOrDefault().ElementId, standartPattern.FirstOrDefault().Id);
            var kpList = await _kurbatovParameterService.GetBySmp(smp.Id);
            var diesList = await _dieValueService.GetSelectedDiesByMeasurementRecordingId(measurementRecordingId);
            Func<Task<Dictionary<string, List<DieValue>>>> cachedService = async () => await _dieValueService.GetDieValuesByMeasurementRecording(measurementRecordingId);
            var dieValuesDictionary = await _cache.GetOrAddAsync($"V_{measurementRecordingIdAsKey}", cachedService);
            Func<Task<Dictionary<string, List<VueExample.StatisticsCore.SingleParameterStatistic>>>> cachedStatisticService = async () => await _statisticService.GetSingleParameterStatisticByDieValues(new ConcurrentDictionary<string, List<DieValue>>(dieValuesDictionary), stageId, 1.0, k);
            var statDictionary = await _cache.GetOrAddAsync($"S_{measurementRecordingIdAsKey}_KF_{k*10}", cachedStatisticService);
            var dirtyCells = _statisticService.GetGraphicDirtyCells(statDictionary, kpList);
            await _cacheProvider.SetCache<List<StatisticsCore.DirtyCellsCore.GraphicDirtyCells>>($"DC{measurementRecordingIdAsKey}", dirtyCells, new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(1)));
            return Ok(dirtyCells);
        }
    }
}