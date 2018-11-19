using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VueExample.Providers;
using VueExample.Extensions;

namespace VueExample.Controllers
{
    [Route("api/[controller]")]
    public class MeasurementController : Controller
    {
        private readonly IMeasurementProvider measurementProvider;

        public MeasurementController(IMeasurementProvider measurementProvider)
        {
            this.measurementProvider = measurementProvider;
        }

        [HttpGet("[action]")]
        public IActionResult FullInfo()
        {
            var measurementInfo = measurementProvider.GetAllMeasurementInfo();
            return Ok(measurementInfo);
        }

        [HttpGet("[action]")]
        public IActionResult GetExtraInfo([FromQuery(Name = "measurementid")] int measurementId)
        {
            return Ok(measurementProvider.GetPointsByMeasurementId(measurementId));
        }

        [HttpGet("[action]")]
        public IActionResult GetPoints([FromQuery(Name = "measurementid")] int measurementId, [FromQuery(Name = "deviceid")] int deviceId, [FromQuery(Name = "graphicid")] int graphicId, [FromQuery(Name = "port")] int port)
        {
            var pointsDictionary = new Dictionary<string, List<object>>();
            var pointsList = new List<object>(measurementProvider.GetPoints(measurementId, deviceId, graphicId, port)
                .Select(x => new
                {
                    Value = x.Value, Time = x.Time.Trim(TimeSpan.TicksPerMinute), DeviceId = x.DeviceId,
                    PortNumber = x.PortNumber
                }));
            pointsDictionary.Add($"M{measurementId}D{deviceId}PN{port}", pointsList);
            if (pointsList.Count == 0)
            {
                return NoContent();
            }
            return Ok(pointsDictionary);
        }
    }
}
