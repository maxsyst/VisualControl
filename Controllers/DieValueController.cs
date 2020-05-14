using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VueExample.Services;
using VueExample.Models.SRV6;
using LazyCache;
using VueExample.Providers.Srv6.Interfaces;

namespace VueExample.Controllers
{
    [Route("api/[controller]/[action]")]
    public class DieValueController : Controller
    {

        private readonly IAppCache cache;
        private readonly IDieValueService _dieValueService;

        public DieValueController(IDieValueService dieValueService, IAppCache cache)
        {
            _dieValueService = dieValueService;
            this.cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetByMeasurementRecordingId(int measurementRecordingId)
        {
            string measurementRecordingIdAsKey = "V_" + Convert.ToString(measurementRecordingId);
            Func<Task<Dictionary<string, List<DieValue>>>> cachedService = () => _dieValueService.GetDieValuesByMeasurementRecording(measurementRecordingId);
            var dieValuesDictionary = await cache.GetOrAddAsync(measurementRecordingIdAsKey, cachedService);
            return Ok(JsonConvert.SerializeObject(dieValuesDictionary));
        }

        [HttpGet]
        public async Task<IActionResult> GetSelectedDiesByMeasurementRecordingId(int measurementRecordingId)
        {
            var diesList = await _dieValueService.GetSelectedDiesByMeasurementRecordingId(measurementRecordingId);
            return Ok(JsonConvert.SerializeObject(diesList));
        }
    }
}
