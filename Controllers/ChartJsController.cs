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
using VueExample.Providers.Srv6.Interfaces;

namespace VueExample.Controllers
{
    [Route("api/[controller]")]
    public class ChartJsController : Controller
    {        
        private readonly IDieValueService _dieValueService;
        private readonly IChartJSProvider _chartJSProvider;
        public ChartJsController(IChartJSProvider chartJSProvider, IDieValueService dieValueService)
        {
            _chartJSProvider = chartJSProvider;
            _dieValueService = dieValueService;
        }

        [HttpGet]
        [ProducesResponseType (typeof(AbstractChart), StatusCodes.Status200OK)]
        [Route("GetLinearForMeasurement")]
        [ResponseCache(CacheProfileName = "Default60")]
        public async Task<IActionResult> GetLinearForMeasurement([FromQuery] string statisticSingleGraphicViewModelJSON)
        {
            var statisticSingleGraphicViewModel = JsonConvert.DeserializeObject<VueExample.ViewModels.StatisticSingleGraphicViewModel>(statisticSingleGraphicViewModelJSON);
            string measurementRecordingIdAsKey = Convert.ToString(statisticSingleGraphicViewModel.MeasurementId);
            var dieValueList = await _dieValueService.GetDieValuesByMeasurementRecordingAndKeyGraphicState(statisticSingleGraphicViewModel.MeasurementId, statisticSingleGraphicViewModel.KeyGraphicState);
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
            var dieValueList = await _dieValueService.GetDieValuesByMeasurementRecordingAndKeyGraphicState(statisticSingleGraphicViewModel.MeasurementId, keyGraphic);
            var amchart = await _chartJSProvider.GetHistogramFromDieValues(dieValueList, 
                                                                           statisticSingleGraphicViewModel.dieIdList, 
                                                                           double.Parse(statisticSingleGraphicViewModel.Divider, CultureInfo.InvariantCulture), 
                                                                           keyGraphic);
            return Ok(amchart);
        }

    }
}