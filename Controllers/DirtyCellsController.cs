using Microsoft.AspNetCore.Mvc;
using LazyCache;
using VueExample.Providers.Srv6.Interfaces;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using VueExample.Models.SRV6;
using VueExample.StatisticsCore.Services;

namespace VueExample.Controllers
{
    [Route("api/[controller]")]
    public class DirtyCellsController : Controller
    {
        private readonly IAppCache _cache;
        private readonly IDieValueService _dieValueService;
        private readonly IStageProvider _stageProvider;
        private readonly StatisticService _statisticService;
        public DirtyCellsController(IAppCache cache, IDieValueService dieValueService, IStageProvider stageProvider, StatisticService statisticService)
        {
            _cache = cache;
            _dieValueService = dieValueService;
            _stageProvider = stageProvider;
            _statisticService = statisticService;
        }

        [HttpGet]
        [Route("GetDirtyCellsByMeasurementRecordingStandart")]
        public async Task<IActionResult> GetByMeasurementRecording ([FromQuery] int measurementRecordingId)
        {
            var k = 1.5;
            var diesList = await _dieValueService.GetSelectedDiesByMeasurementRecordingId(measurementRecordingId);
            string measurementRecordingIdAsKey = Convert.ToString(measurementRecordingId);
            var stageId = (await _stageProvider.GetByMeasurementRecordingId(measurementRecordingId)).StageId;
            Func<Task<Dictionary<string, List<DieValue>>>> cachedService = async () => await _dieValueService.GetDieValuesByMeasurementRecording(measurementRecordingId);
            var dieValuesDictionary = await _cache.GetOrAddAsync($"V_{measurementRecordingIdAsKey}", cachedService);
            Func<Task<Dictionary<string, List<VueExample.StatisticsCore.SingleParameterStatistic>>>> cachedStatisticService = async () => await _statisticService.GetSingleParameterStatisticByDieValues(new ConcurrentDictionary<string, List<DieValue>>(dieValuesDictionary), stageId, 1.0, k);
            var statDictionary = await _cache.GetOrAddAsync($"S_{measurementRecordingIdAsKey}_KF_{k*10}", cachedStatisticService);
            return Ok();
        }
    }
}