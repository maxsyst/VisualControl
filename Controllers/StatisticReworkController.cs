using System.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VueExample.Models.SRV6;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.StatisticsCoreRework.Abstract;

namespace VueExample.Controllers
{
    [Route ("api/statrwrk")]
    public class StatisticReworkController : Controller
    {
        private readonly IStatisticService _statisticService;
        private readonly IDieValueService _dieValueService;
        public StatisticReworkController(IStatisticService statisticService, IDieValueService dieValueService)
        {
            _dieValueService = dieValueService;
            _statisticService = statisticService;
        }

        [HttpGet]
        [Route("GetStatByMeasurementRecording")]
        public async Task<IActionResult> GetStatByMeasurementRecording ([FromQuery] int measurementRecordingId)
        {
            var s = await _statisticService.GetSingleParameterStatisticByMeasurementRecording(measurementRecordingId);
            return Ok(s);
        }

        [HttpGet]
        [Route("GetStatisticSingleGraphic")]
        public async Task<IActionResult> GetStatisticSingleGraphic([FromQuery] string statisticSingleGraphicViewModelJSON)
        {
            var statisticSingleGraphicViewModel = JsonConvert.DeserializeObject<VueExample.ViewModels.StatisticSingleGraphicViewModel>(statisticSingleGraphicViewModelJSON);
            var (measurementRecordingId, keyGraphicState, _) = statisticSingleGraphicViewModel;
            string keyGraphic = statisticSingleGraphicViewModel.KeyGraphicState;
            var dict = await _statisticService.GetCalculatedStatisticByMeasurementRecordingGraphicStateAndDies(measurementRecordingId, keyGraphicState,  double.Parse(statisticSingleGraphicViewModel.Divider, CultureInfo.InvariantCulture), statisticSingleGraphicViewModel.dieIdList.Select(x => (long)x).ToList());
            return Ok(dict);
        }
    }
}