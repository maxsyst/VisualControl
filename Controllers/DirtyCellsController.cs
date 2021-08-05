using System.Linq;
using Microsoft.AspNetCore.Mvc;
using LazyCache;
using VueExample.Providers.Srv6.Interfaces;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using VueExample.Models.SRV6;
using VueExample.StatisticsCore.Services;
using VueExample.Providers.Abstract;

namespace VueExample.Controllers
{
    [Route("api/[controller]")]
    public class DirtyCellsController : Controller
    {
        private readonly IAppCache _cache;
        private readonly IDieValueService _dieValueService;
        private readonly IKurbatovParameterProvider _kurbatovParameterProvider;
        private readonly IWaferProvider _waferProvider;
        private readonly IStageProvider _stageProvider;
        private readonly IElementService _elementService;
        private readonly IDieTypeProvider _dieTypeProvider;
        private readonly IStandartPatternService _standartPatternService;
        private readonly StatisticService _statisticService;
        private readonly IStandartMeasurementPatternService _standartMeasurementPatternService;
        public DirtyCellsController(IAppCache cache, IStandartMeasurementPatternService standartMeasurementPatternService, IElementService elementService, IDieValueService dieValueService, IStageProvider stageProvider, IStandartPatternService standartPatternService, IDieTypeProvider dieTypeProvider, IKurbatovParameterProvider kurbatovParameterProvider, IWaferProvider waferProvider, StatisticService statisticService)
        {
            _cache = cache;
            _kurbatovParameterProvider = kurbatovParameterProvider;
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
        [Route("GetDirtyCellsByMeasurementRecordingStandart")]
        public async Task<IActionResult> GetByMeasurementRecording ([FromQuery] int measurementRecordingId)
        {
            var k = 1.5;
            var wafer = await _waferProvider.GetByMeasurementRecordingId(measurementRecordingId);
            var stageId = (await _stageProvider.GetByMeasurementRecordingId(measurementRecordingId)).StageId;
            var dieType = (await _dieTypeProvider.GetByWaferId(wafer.WaferId)).FirstOrDefault();
            var standartPattern = await _standartPatternService.GetByDieTypeId(dieType.DieTypeId);
            var element = await _elementService.GetByIdmr(measurementRecordingId);
            var smp = await _standartMeasurementPatternService.GetByStageAndElementAndPattern(stageId, element.FirstOrDefault().ElementId, standartPattern.FirstOrDefault().Id);
            var kpList = await _kurbatovParameterProvider.GetBySmp(smp.Id);
            var diesList = await _dieValueService.GetSelectedDiesByMeasurementRecordingId(measurementRecordingId);
            string measurementRecordingIdAsKey = Convert.ToString(measurementRecordingId);
            Func<Task<Dictionary<string, List<DieValue>>>> cachedService = async () => await _dieValueService.GetDieValuesByMeasurementRecording(measurementRecordingId);
            var dieValuesDictionary = await _cache.GetOrAddAsync($"V_{measurementRecordingIdAsKey}", cachedService);
            Func<Task<Dictionary<string, List<VueExample.StatisticsCore.SingleParameterStatistic>>>> cachedStatisticService = async () => await _statisticService.GetSingleParameterStatisticByDieValues(new ConcurrentDictionary<string, List<DieValue>>(dieValuesDictionary), stageId, 1.0, k);
            var statDictionary = await _cache.GetOrAddAsync($"S_{measurementRecordingIdAsKey}_KF_{k*10}", cachedStatisticService);
            return Ok();
        }
    }
}