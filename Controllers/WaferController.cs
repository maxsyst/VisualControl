using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VueExample.Models;
using VueExample.Providers;
using VueExample.Providers.Srv6.Interfaces;

namespace VueExample.Controllers
{
    [Route("api/[controller]")]
    public class WaferController : Controller
    {
        private readonly IWaferProvider _waferProvider;
        private readonly IDefectProvider _defectProvider;
        
        public WaferController(IWaferProvider waferProvider, IDefectProvider defectProvider)
        {
            _waferProvider = waferProvider;
            _defectProvider = defectProvider;
        }

        [HttpGet]
        [ProducesResponseType (typeof(List<Wafer>), StatusCodes.Status200OK)]
        [Route("all")]
        public async Task<IActionResult> GetAll()
        {
           return Ok(await _waferProvider.GetWafers());
        }

        [HttpGet]
        [Route("pwafer")]
        public async Task<IActionResult> GetPWafer()
        {
           return Ok(await _waferProvider.GetPWafer());
        }

        [HttpGet]
        [Route("measurementrecordingid/{measurementRecordingId:int}")]
        public async Task<IActionResult> GetByMeasurementRecording([FromRoute] int measurementRecordingId)
        {
           return Ok(await _waferProvider.GetByMeasurementRecordingId(measurementRecordingId));
        }
    }
}
