using System.Globalization;
using System;
using System.Collections.Generic;
using LazyCache;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VueExample.Models.SRV6;
using System.Threading.Tasks;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.Contexts;
using System.Linq;
using VueExample.Providers.Srv6;
using System.Collections.Concurrent;

namespace VueExample.Controllers
{
    [Route ("api/[controller]")]
    public class StatisticController : Controller 
    {
        private readonly IAppCache cache;
        private readonly IDieValueService _dieValueService;
        private readonly IStageProvider _stageProvider;
        private readonly IStatParameterService _statParameterService;
        private readonly StatisticsCore.Services.StatisticService statisticService;
        public StatisticController (IAppCache cache, IStageProvider stageProvider, IDieValueService dieValueService, ISRV6GraphicService graphicService, IStatParameterService statParameterService) 
        {
            _dieValueService = dieValueService;
            _stageProvider = stageProvider;
            _statParameterService = statParameterService;
            statisticService = new StatisticsCore.Services.StatisticService(graphicService, statParameterService);
            this.cache = cache;
        }

        [HttpGet]
        [Route("GetDirtyCellsByMeasurementRecordingOnly")]
        public async Task<IActionResult> GetDirtyCellsByMeasurementRecording ([FromQuery] int measurementRecordingId)
        {
            var k = 1.5;
            var diesList = await _dieValueService.GetSelectedDiesByMeasurementRecordingId(measurementRecordingId);
            string measurementRecordingIdAsKey = Convert.ToString(measurementRecordingId);
            var stageId = (await _stageProvider.GetByMeasurementRecordingId(measurementRecordingId)).StageId;
            Func<Task<Dictionary<string, List<DieValue>>>> cachedService = async () => await _dieValueService.GetDieValuesByMeasurementRecording(measurementRecordingId);
            var dieValuesDictionary = await cache.GetOrAddAsync($"V_{measurementRecordingIdAsKey}", cachedService);
            Func<Task<Dictionary<string, List<VueExample.StatisticsCore.SingleParameterStatistic>>>> cachedStatisticService = async () => await statisticService.GetSingleParameterStatisticByDieValues(new ConcurrentDictionary<string, List<DieValue>>(dieValuesDictionary), stageId, 1.0, k);
            var statDictionary = await cache.GetOrAddAsync($"S_{measurementRecordingIdAsKey}_KF_{k*10}", cachedStatisticService);
            var dirtyCells = statisticService.GetDirtyCellsBySPSDictionary(new ConcurrentDictionary<string, List<StatisticsCore.SingleParameterStatistic>>(statDictionary), diesList.Count);
            return Ok(new {goodCellsPercentage = dirtyCells.StatPercentageFullWafer, dirtyCellsArray = dirtyCells.StatList, diesCount = diesList.Count});
        }

        [HttpGet]
        [Route("GetDirtyCellsByMeasurementRecording")]
        public async Task<IActionResult> GetDirtyCellsByMeasurementRecording ([FromQuery] int measurementRecordingId, [FromQuery] int diesCount, [FromQuery] double k)
        {
            string measurementRecordingIdAsKey = Convert.ToString (measurementRecordingId);
            var stageId = (await _stageProvider.GetByMeasurementRecordingId(measurementRecordingId)).StageId;
            var dieValuesDictionary = await cache.GetAsync<Dictionary<string, List<DieValue>>>($"V_{measurementRecordingIdAsKey}");
            Func<Task<Dictionary<string, List<VueExample.StatisticsCore.SingleParameterStatistic>>>> cachedStatisticService = async () => await statisticService.GetSingleParameterStatisticByDieValues(new ConcurrentDictionary<string, List<DieValue>>(dieValuesDictionary), stageId, 1.0, k);
            var statDictionary = await cache.GetOrAddAsync($"S_{measurementRecordingIdAsKey}_KF_{k*10}", cachedStatisticService);
            return Ok(statisticService.GetDirtyCellsBySPSDictionary(new ConcurrentDictionary<string, List<StatisticsCore.SingleParameterStatistic>>(statDictionary), diesCount));
        }

        [HttpGet]
        [Route("GetStatisticSingleGraphic")]
        [ResponseCache(CacheProfileName = "Default60")]
        public async Task<IActionResult> GetStatisticSingleGraphic([FromQuery] string statisticSingleGraphicViewModelJSON)
        {
            var statisticSingleGraphicViewModel = JsonConvert.DeserializeObject<VueExample.ViewModels.StatisticSingleGraphicViewModel>(statisticSingleGraphicViewModelJSON);
            string measurementRecordingIdAsKey = Convert.ToString(statisticSingleGraphicViewModel.MeasurementId);
            string keyGraphic = statisticSingleGraphicViewModel.KeyGraphicState;
            var dieValueList = (await cache.GetAsync<Dictionary<string, List<DieValue>>>($"V_{measurementRecordingIdAsKey}"))[keyGraphic];
            var singleParameterStatisticList = 
                (await cache.GetAsync<Dictionary<string, List<VueExample.StatisticsCore.SingleParameterStatistic>>>($"S_{measurementRecordingIdAsKey}_KF_{statisticSingleGraphicViewModel.K*10}"))[keyGraphic];
            var statisticDataList = await statisticService
                                          .GetStatisticsDataByGraphicState(statisticSingleGraphicViewModel.dieIdList, 
                                                                           statisticSingleGraphicViewModel.KeyGraphicState, 
                                                                           dieValueList, 
                                                                           double.Parse(statisticSingleGraphicViewModel.Divider, CultureInfo.InvariantCulture), 
                                                                           singleParameterStatisticList);
            return Ok(statisticDataList);
        }

        [HttpGet]
        [Route("GetStatisticSingleGraphicFullWafer")]
        [ResponseCache(CacheProfileName = "Default60")]
        public async Task<IActionResult> GetStatisticSingleGraphicFullWafer ([FromQuery] int measurementRecordingId, [FromQuery] string keyGraphicState, [FromQuery] double k) 
        {
            string measurementRecordingIdAsKey = Convert.ToString(measurementRecordingId);
            var dieValueList = (await cache.GetAsync<Dictionary<string, List<DieValue>>>($"V_{measurementRecordingIdAsKey}"))[keyGraphicState];
            var dieList = dieValueList.Select(x => x.DieId).ToList();
            var singleParameterStatisticList = 
                (await cache.GetAsync<Dictionary<string, List<VueExample.StatisticsCore.SingleParameterStatistic>>>($"S_{measurementRecordingIdAsKey}_KF_{k*10}"))[keyGraphicState];
            var statisticDataList = await statisticService
                                          .GetStatisticsDataByGraphicState(dieList, 
                                                                           keyGraphicState, 
                                                                           dieValueList, 
                                                                           1.0, 
                                                                           singleParameterStatisticList);
            return Ok(statisticDataList);
        }
        
        [HttpGet]
        [Route("GetDirtyCellsSingleGraphic")]
        public async Task<IActionResult> GetDirtyCellsSingleGraphic([FromQuery] string statisticSingleGraphicViewModelJSON)
        {
            var statisticSingleGraphicViewModel = JsonConvert.DeserializeObject<VueExample.ViewModels.StatisticSingleGraphicViewModel>(statisticSingleGraphicViewModelJSON);
            string measurementRecordingIdAsKey = Convert.ToString(statisticSingleGraphicViewModel.MeasurementId);
            string keyGraphic = statisticSingleGraphicViewModel.KeyGraphicState;
            var statDictionary = (await cache.GetAsync<Dictionary<string, List<VueExample.StatisticsCore.SingleParameterStatistic>>>($"S_{measurementRecordingIdAsKey}_KF_{statisticSingleGraphicViewModel.K*10}"))[keyGraphic];
            return Ok(statDictionary);
        }
    }
}