using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LazyCache;
using MathNet.Numerics.Statistics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.StatisticsCore.Abstract;
using VueExample.ViewModels;

namespace VueExample.Controllers
{
    [Route("api/[controller]")]
    public class GradientController : Controller
    {
        private readonly IGradientService _gradientService;
        private readonly IStatisticCacheService _statisticService;
        public GradientController(IStatisticCacheService statisticService, IGradientService gradientService)
        {
            _statisticService = statisticService;
            _gradientService = gradientService;
        }

        [ProducesResponseType (typeof(Histogram), StatusCodes.Status200OK)]
        [Route("histogram")]
        [HttpGet]
        public async Task<IActionResult> GetHistogram([FromQuery] string histogramViewModelJSON)
        {
            var histogramViewModel = JsonConvert.DeserializeObject<GradientStatViewModel>(histogramViewModelJSON);
            var (measurementRecordingId, kgs, k) = histogramViewModel;
            var singleParameterStatisticList = await _statisticService.GetSingleParameterStatisticByMeasurementRecordingIdAndKeyGraphicState(measurementRecordingId, k, kgs);
            var histogram = _gradientService.GetHistogram(singleParameterStatisticList, histogramViewModel.StepsQuantity, histogramViewModel.StatParameter, histogramViewModel.SelectedDiesId);
            return histogram.DataCount > 0 ? Ok(histogram) : (IActionResult)BadRequest(histogram);
        }


        [HttpGet]
        [ProducesResponseType (typeof(GradientViewModel), StatusCodes.Status200OK)]
        [Route("statparameter")]
        public async Task<IActionResult> GetGradientStatParameter([FromQuery] string gradientViewModelJSON)
        {
            var gradientViewModel = JsonConvert.DeserializeObject<GradientStatViewModel>(gradientViewModelJSON);
            var (measurementRecordingId, kgs, k) = gradientViewModel;
            var singleParameterStatisticList = await _statisticService.GetSingleParameterStatisticByMeasurementRecordingIdAndKeyGraphicState(measurementRecordingId, k, kgs);
            var gradient = _gradientService.GetGradient(singleParameterStatisticList, gradientViewModel.StepsQuantity, gradientViewModel.Divider, gradientViewModel.StatParameter, gradientViewModel.SelectedDiesId);
            return gradient.GradientSteps.Count > 0 ? Ok(gradient) : (IActionResult)BadRequest(gradient);
        }

    }
}