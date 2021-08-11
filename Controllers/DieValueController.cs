using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VueExample.Models.SRV6;
using LazyCache;
using VueExample.Providers.Srv6.Interfaces;

namespace VueExample.Controllers
{
    [Route("api/[controller]/[action]")]
    public class DieValueController : Controller
    {

        private readonly IDieValueService _dieValueService;

        public DieValueController(IDieValueService dieValueService)
        {
            _dieValueService = dieValueService;
        }

        [HttpGet]
        public async Task<IActionResult> GetByMeasurementRecordingId(int measurementRecordingId)
        {
            var dieValuesDictionary = await _dieValueService.GetDieValuesByMeasurementRecording(measurementRecordingId);
            return Ok(dieValuesDictionary);
        }

        [HttpGet]
        public async Task<IActionResult> GetSelectedDiesByMeasurementRecordingId(int measurementRecordingId)
        {
            var diesList = await _dieValueService.GetSelectedDiesByMeasurementRecordingId(measurementRecordingId);
            return Ok(JsonConvert.SerializeObject(diesList));
        }
    }
}
