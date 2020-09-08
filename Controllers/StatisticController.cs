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

namespace VueExample.Controllers
{
    [Route ("api/[controller]")]
    public class StatisticController : Controller 
    {
        private readonly IAppCache cache;
        private readonly IServiceProvider _services;
        private readonly IDieValueService _dieValueService;
        private readonly IStageProvider _stageProvider;
        private readonly StatisticsCore.Services.StatisticService statisticService;
        public StatisticController (IAppCache cache, IServiceProvider services, IStageProvider stageProvider, IDieValueService dieValueService, ISRV6GraphicService graphicService) 
        {
            _dieValueService = dieValueService;
            _stageProvider = stageProvider;
            _services = services;
            statisticService = new StatisticsCore.Services.StatisticService(graphicService, _services);
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
            Func<Dictionary<string, List<VueExample.StatisticsCore.SingleParameterStatistic>>> cachedStatisticService = () => statisticService.GetSingleParameterStatisticByDieValues(dieValuesDictionary, stageId, 1.0, k);
            var statDictionary = cache.GetOrAdd($"S_{measurementRecordingIdAsKey}_KF_{k*10}", cachedStatisticService);
            var dirtyCells = statisticService.GetDirtyCellsBySPSDictionary(statDictionary, diesList.Count);
            return Ok(new {goodCellsPercentage = dirtyCells.StatPercentageFullWafer, dirtyCellsArray = dirtyCells.StatList, diesCount = diesList.Count});
        }

        [HttpGet]
        [Route("GetDirtyCellsByMeasurementRecording")]
        public async Task<IActionResult> GetDirtyCellsByMeasurementRecording ([FromQuery] int measurementRecordingId, [FromQuery] int diesCount, [FromQuery] double k)
        {
            string measurementRecordingIdAsKey = Convert.ToString (measurementRecordingId);
            var stageId = (await _stageProvider.GetByMeasurementRecordingId(measurementRecordingId)).StageId;
            var dieValuesDictionary = await cache.GetAsync<Dictionary<string, List<DieValue>>>($"V_{measurementRecordingIdAsKey}");
            Func<Dictionary<string, List<VueExample.StatisticsCore.SingleParameterStatistic>>> cachedStatisticService = () => statisticService.GetSingleParameterStatisticByDieValues(dieValuesDictionary, stageId, 1.0, k);
            var statDictionary = cache.GetOrAdd($"S_{measurementRecordingIdAsKey}_KF_{k*10}", cachedStatisticService);
            return Ok(statisticService.GetDirtyCellsBySPSDictionary(statDictionary, diesCount));
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