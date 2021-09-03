using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LazyCache;
using MathNet.Numerics.Statistics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.StatisticsCoreRework.Abstract;
using VueExample.ViewModels;

namespace VueExample.Controllers
{
    [Route("api/[controller]")]
    public class GradientController : Controller
    {
        private readonly IGradientService _gradientService;
        private readonly IStatisticService _statisticService;
        public GradientController(IStatisticService statisticService, IGradientService gradientService)
        {
            _statisticService = statisticService;
            _gradientService = gradientService;
        }

        [HttpGet]
        [ProducesResponseType (typeof(GradientViewModel), StatusCodes.Status200OK)]
        [Route("statparameter")]
        public async Task<IActionResult> GetGradientStatParameter([FromQuery] string gradientViewModelJSON)
        {
            var gradientViewModel = JsonConvert.DeserializeObject<GradientStatViewModel>(gradientViewModelJSON);
            var (measurementRecordingId, kgs, lowBorder, topBorder) = gradientViewModel;
            var singleParameterStatistic = ((await _statisticService.GetSingleParameterStatisticByMeasurementRecording(measurementRecordingId))[kgs])[gradientViewModel.StatParameter];
            var gradient = _gradientService.GetGradient(singleParameterStatistic, gradientViewModel.StepsQuantity, lowBorder, topBorder, gradientViewModel.Divider);
            return gradient.GradientSteps.Count > 0 ? Ok(gradient) : (IActionResult)BadRequest(gradient);
        }

    }
}