using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LazyCache;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.ViewModels;

namespace VueExample.Controllers
{
    [Route("api/[controller]")]
    public class GradientController : Controller
    {
        private readonly IGradientService _gradientService;
        private readonly IAppCache _cache; 
        public GradientController(IAppCache cache, IGradientService gradientService)
        {
            _gradientService = gradientService;
            _cache = cache;
        }

        [HttpGet]
        [ProducesResponseType (typeof(GradientViewModel), StatusCodes.Status200OK)]
        [Route("GetGradient")]
        public async Task<IActionResult> GetGradient([FromQuery] string gradientViewModelJSON)
        {
            var gradientViewModel = JsonConvert.DeserializeObject<GradientStatViewModel>(gradientViewModelJSON);
            string measurementRecordingIdAsKey = Convert.ToString(gradientViewModel.MeasurementRecordingId);
            var singleParameterStatisticList = 
                (await _cache.GetAsync<Dictionary<string, List<VueExample.StatisticsCore.SingleParameterStatistic>>>($"S_{measurementRecordingIdAsKey}_KF_{gradientViewModel.K*10}"))[gradientViewModel.KeyGraphicState];
            return Ok();
        }

    }
}