using System.Globalization;
using System.Text;
using System.IO.IsolatedStorage;
using Microsoft.AspNetCore.Mvc;
using VueExample.Providers.Srv6.Interfaces;
using System;
using System.Threading.Tasks;

namespace VueExample.Controllers
{
    [Route("api/[controller]")]
    public class NormalizeController : Controller
    {
        private readonly INormalizeService _normalizeService;
        public NormalizeController(INormalizeService normalizeService)
        {
            _normalizeService = normalizeService;
        }

        [HttpPost]
        [Route("hist/{idmr:int}/mean/{mean}/stddev/{stddev}")]
        public async Task<IActionResult> NormalizeHistogram([FromRoute] int idmr, [FromRoute] string mean, [FromRoute] string stddev)
        {
            await _normalizeService.NormalizeHistogram(idmr, Convert.ToDouble(mean, CultureInfo.InvariantCulture), Convert.ToDouble(stddev, CultureInfo.InvariantCulture));
            return Ok();
        }

        [HttpPost]
        [Route("hist/{idmr:int}/graphicid/{graphicId:int}/waferId/{waferId}/mean/{mean}/stddev/{stddev}")]
        public async Task<IActionResult> CreateNormalizeHistogram([FromRoute] int idmr, [FromRoute] int graphicId, [FromRoute] string waferId, [FromRoute] string mean, [FromRoute] string stddev)
        {
            await _normalizeService.CreateNewNormalizeHistogram(idmr, graphicId, waferId, Convert.ToDouble(mean, CultureInfo.InvariantCulture), Convert.ToDouble(stddev, CultureInfo.InvariantCulture));
            return Ok();
        }

    }
}