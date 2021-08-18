using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
    }
}