using System.Globalization;
using System;
using System.Collections.Generic;
using LazyCache;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VueExample.Models.SRV6;
using System.Threading.Tasks;
using VueExample.Providers.Srv6.Interfaces;
using System.Linq;
using System.Collections.Concurrent;
using VueExample.StatisticsCore.Abstract;

namespace VueExample.Controllers
{
    [Route ("api/[controller]")]
    public class StatisticController : Controller 
    {
        private readonly IAppCache cache;
        private readonly IDieValueService _dieValueService;
        private readonly IStageProvider _stageProvider;
        private readonly IStatParameterService _statParameterService;
        private readonly IStatisticService _statisticService;
        public StatisticController (IAppCache cache, IStatisticService statisticService, IStageProvider stageProvider, IDieValueService dieValueService, ISRV6GraphicService graphicService, IStatParameterService statParameterService) 
        {
            _dieValueService = dieValueService;
            _stageProvider = stageProvider;
            _statParameterService = statParameterService;
            _statisticService = statisticService;
            this.cache = cache;
        }

        [HttpGet]
        [Route("GetDirtyCellsByMeasurementRecordingArray")]
        public async Task<IActionResult> GetDirtyCellsByMeasurementRecordingArray([FromQuery] int[] measurementRecordingIdArray)
        {
            var k = 1.5;
            var resultList = new List<object>();
            for (int i = 0; i < measurementRecordingIdArray.Length; i++)
            {
                var measurementRecordingId =  measurementRecordingIdArray[i];
                var diesList = await _dieValueService.GetSelectedDiesByMeasurementRecordingId(measurementRecordingId);
                string measurementRecordingIdAsKey = Convert.ToString(measurementRecordingId);
                var stageId = (await _stageProvider.GetByMeasurementRecordingId(measurementRecordingId)).StageId;
                var dieValuesDictionary = await _dieValueService.GetDieValuesByMeasurementRecording(measurementRecordingId);
                Func<Task<Dictionary<string, List<VueExample.StatisticsCore.SingleParameterStatistic>>>> cachedStatisticService = async () => await _statisticService.GetSingleParameterStatisticByDieValues(new ConcurrentDictionary<string, List<DieValue>>(dieValuesDictionary), stageId, 1.0, k);
                var statDictionary = await cache.GetOrAddAsync($"S_{measurementRecordingIdAsKey}_KF_{k*10}", cachedStatisticService);
                var dirtyCells = _statisticService.GetDirtyCellsBySPSDictionary(new ConcurrentDictionary<string, List<StatisticsCore.SingleParameterStatistic>>(statDictionary), diesList.Count);
                resultList.Add(new {goodCellsPercentage = dirtyCells.StatPercentageFullWafer, dirtyCellsArray = dirtyCells.StatList, diesCount = diesList.Count, MeasurementId = measurementRecordingId});
            }
            return Ok(resultList);
        }

        [HttpGet]
        [Route("GetDirtyCellsByMeasurementRecordingOnly")]
        public async Task<IActionResult> GetDirtyCellsByMeasurementRecording ([FromQuery] int measurementRecordingId)
        {
            var k = 1.5;
            var diesList = await _dieValueService.GetSelectedDiesByMeasurementRecordingId(measurementRecordingId);
            string measurementRecordingIdAsKey = Convert.ToString(measurementRecordingId);
            var stageId = (await _stageProvider.GetByMeasurementRecordingId(measurementRecordingId)).StageId;
            var dieValuesDictionary = await _dieValueService.GetDieValuesByMeasurementRecording(measurementRecordingId);
            Func<Task<Dictionary<string, List<VueExample.StatisticsCore.SingleParameterStatistic>>>> cachedStatisticService = async () => await _statisticService.GetSingleParameterStatisticByDieValues(new ConcurrentDictionary<string, List<DieValue>>(dieValuesDictionary), stageId, 1.0, k);
            var statDictionary = await cache.GetOrAddAsync($"S_{measurementRecordingIdAsKey}_KF_{k*10}", cachedStatisticService);
            var dirtyCells = _statisticService.GetDirtyCellsBySPSDictionary(new ConcurrentDictionary<string, List<StatisticsCore.SingleParameterStatistic>>(statDictionary), diesList.Count);
            return Ok(new {goodCellsPercentage = dirtyCells.StatPercentageFullWafer, dirtyCellsArray = dirtyCells.StatList, diesCount = diesList.Count});
        }

        [HttpGet]
        [Route("GetDirtyCellsByMeasurementRecording")]
        public async Task<IActionResult> GetDirtyCellsByMeasurementRecording ([FromQuery] int measurementRecordingId, [FromQuery] int diesCount, [FromQuery] double k)
        {
            string measurementRecordingIdAsKey = Convert.ToString(measurementRecordingId);
            var stageId = (await _stageProvider.GetByMeasurementRecordingId(measurementRecordingId)).StageId;
            var dieValuesDictionary = await _dieValueService.GetDieValuesByMeasurementRecording(measurementRecordingId);
            Func<Task<Dictionary<string, List<VueExample.StatisticsCore.SingleParameterStatistic>>>> cachedStatisticService = async () => await _statisticService.GetSingleParameterStatisticByDieValues(new ConcurrentDictionary<string, List<DieValue>>(dieValuesDictionary), stageId, 1.0, k);
            var statDictionary = await cache.GetOrAddAsync($"S_{measurementRecordingIdAsKey}_KF_{k*10}", cachedStatisticService);
            var dirtyCells = _statisticService.GetDirtyCellsBySPSDictionary(new ConcurrentDictionary<string, List<StatisticsCore.SingleParameterStatistic>>(statDictionary), diesCount);
            return Ok(dirtyCells);
        }

        [HttpGet]
        [Route("GetStatisticSingleGraphic")]
        [ResponseCache(CacheProfileName = "Default60")]
        public async Task<IActionResult> GetStatisticSingleGraphic([FromQuery] string statisticSingleGraphicViewModelJSON)
        {
            var statisticSingleGraphicViewModel = JsonConvert.DeserializeObject<VueExample.ViewModels.StatisticSingleGraphicViewModel>(statisticSingleGraphicViewModelJSON);
            string measurementRecordingIdAsKey = Convert.ToString(statisticSingleGraphicViewModel.MeasurementId);
            string keyGraphic = statisticSingleGraphicViewModel.KeyGraphicState;
            var dieValueList = await _dieValueService.GetDieValuesByMeasurementRecordingAndKeyGraphicState(statisticSingleGraphicViewModel.MeasurementId, keyGraphic);
            var singleParameterStatisticList = 
                (await cache.GetAsync<Dictionary<string, List<VueExample.StatisticsCore.SingleParameterStatistic>>>($"S_{measurementRecordingIdAsKey}_KF_{statisticSingleGraphicViewModel.K*10}"))[keyGraphic];
            var statisticDataList = await _statisticService
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
            var dieValueList = await _dieValueService.GetDieValuesByMeasurementRecordingAndKeyGraphicState(measurementRecordingId, keyGraphicState);
            var dieList = dieValueList.Select(x => x.DieId).ToList();
            var singleParameterStatisticList = 
                (await cache.GetAsync<Dictionary<string, List<VueExample.StatisticsCore.SingleParameterStatistic>>>($"S_{measurementRecordingIdAsKey}_KF_{k*10}"))[keyGraphicState];
            var statisticDataList = await _statisticService
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