using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using LazyCache;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VueExample.ChartModels.ChartJs;
using VueExample.Models.SRV6;
using VueExample.Providers;

namespace VueExample.Controllers
{
    [Route("api/[controller]")]
    public class ChartJsController : Controller
    {        
        private readonly IAppCache cache;
        private readonly IChartJSProvider _chartJSProvider;
        public ChartJsController(IChartJSProvider chartJSProvider, IAppCache cache)
        {
            this._chartJSProvider = chartJSProvider;
            this.cache = cache;
        }

        [HttpGet]
        [ProducesResponseType (typeof(AbstractChart), StatusCodes.Status200OK)]
        [Route("GetLinearForMeasurement")]
        [ResponseCache(CacheProfileName = "Default60")]
        public async Task<IActionResult> GetLinearForMeasurement([FromQuery] string statisticSingleGraphicViewModelJSON)
        {
            var statisticSingleGraphicViewModel = JsonConvert.DeserializeObject<VueExample.ViewModels.StatisticSingleGraphicViewModel>(statisticSingleGraphicViewModelJSON);
            string measurementRecordingIdAsKey = Convert.ToString(statisticSingleGraphicViewModel.MeasurementId);
            var dieValueList = (await cache.GetAsync<Dictionary<string, List<DieValue>>>($"V_{measurementRecordingIdAsKey}"))[statisticSingleGraphicViewModel.KeyGraphicState];
            var chart = await _chartJSProvider.GetLinearFromDieValues(dieValueList.ToDictionary(x => x.DieId, x => x), 
                                                                      statisticSingleGraphicViewModel.dieIdList, 
                                                                      double.Parse(statisticSingleGraphicViewModel.Divider, CultureInfo.InvariantCulture), 
                                                                      statisticSingleGraphicViewModel.KeyGraphicState);
            return Ok(chart);
        }

        [HttpGet]
        [ProducesResponseType (typeof(AbstractChart), StatusCodes.Status200OK)]
        [Route("GetHistogramForMeasurement")]
        [ResponseCache(CacheProfileName = "Default60")]
        public async Task<IActionResult> GetHistogramForMeasurement([FromQuery] string statisticSingleGraphicViewModelJSON)
        {
            var statisticSingleGraphicViewModel = JsonConvert.DeserializeObject<VueExample.ViewModels.StatisticSingleGraphicViewModel>(statisticSingleGraphicViewModelJSON);
            string measurementRecordingIdAsKey = Convert.ToString(statisticSingleGraphicViewModel.MeasurementId);
            string keyGraphic = statisticSingleGraphicViewModel.KeyGraphicState;
            var dieValueList = (await cache.GetAsync<Dictionary<string, List<DieValue>>>($"V_{measurementRecordingIdAsKey}"))[keyGraphic];
            var amchart = await _chartJSProvider.GetHistogramFromDieValues(dieValueList, 
                                                                           statisticSingleGraphicViewModel.dieIdList, 
                                                                           double.Parse(statisticSingleGraphicViewModel.Divider, CultureInfo.InvariantCulture), 
                                                                           keyGraphic);
            return Ok(amchart);
        }

    }
}