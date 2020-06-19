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
        private readonly Srv6Context _srv6Context;
        private readonly IDieValueService _dieValueService;
        private readonly IStageProvider _stageProvider;
        private readonly StatisticsCore.Services.StatisticService statisticService;
        public StatisticController (IAppCache cache, IStageProvider stageProvider, Srv6Context srv6Context, IDieValueService dieValueService, ISRV6GraphicService graphicService) 
        {
            _dieValueService = dieValueService;
            _stageProvider = stageProvider;
            _srv6Context = srv6Context;
            statisticService = new StatisticsCore.Services.StatisticService(graphicService, _srv6Context);
            this.cache = cache;
        }

        [HttpGet]
        [Route("GetDirtyCellsByMeasurementRecording")]
        public async Task<IActionResult> GetDirtyCellsByMeasurementRecording ([FromQuery] int measurementRecordingId, [FromQuery] int diesCount, [FromQuery] double k)
        {
            string measurementRecordingIdAsKey = Convert.ToString (measurementRecordingId);
            var stageId = (await _stageProvider.GetByMeasurementRecordingId(measurementRecordingId)).StageId;
            Func<Task<Dictionary<string, List<DieValue>>>> cachedDieValueService = () => _dieValueService.GetDieValuesByMeasurementRecording(measurementRecordingId);
            var dieValuesDictionary = await cache.GetOrAddAsync($"V_{measurementRecordingIdAsKey}", cachedDieValueService);
            Func<Task<Dictionary<string, List<VueExample.StatisticsCore.SingleParameterStatistic>>>> cachedStatisticService = () => statisticService.GetSingleParameterStatisticByDieValues(dieValuesDictionary, stageId, 1.0, k);
            var statDictionary = await cache.GetOrAddAsync($"S_{measurementRecordingIdAsKey}_KF_{k*10}", cachedStatisticService);
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
            var statistics = new StatisticsCore.Statistics();
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
            var statistics = new StatisticsCore.Statistics();
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
            var statistics = new StatisticsCore.Statistics();
            var statDictionary = (await cache.GetAsync<Dictionary<string, List<VueExample.StatisticsCore.SingleParameterStatistic>>>($"S_{measurementRecordingIdAsKey}_KF_{statisticSingleGraphicViewModel.K*10}"))[keyGraphic];
            return Ok(statDictionary);
        }
    }
}