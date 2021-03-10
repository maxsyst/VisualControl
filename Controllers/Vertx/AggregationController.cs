using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VueExample.Services.Vertx.Abstract;

namespace VueExample.Controllers.Vertx
{
    [Route("api/vertx/[controller]")]
    public class AggregationController : Controller
    {
        private readonly IAggregationService _aggregationService;
        public AggregationController(IAggregationService aggregationService)
        {
            _aggregationService = aggregationService;
        }

        [HttpGet]
        [Route("lastUpdates/all")]
        public async Task<IActionResult> GetLastUpdates()
        {
            var list = await _aggregationService.GetNLastMeasurementAttemptsWithMdv(-1);
            return list.Count == 0 ? NotFound() : Ok(list);
        }

        [HttpGet]
        [Route("lastUpdates/{n:int}")]
        public async Task<IActionResult> GetLastNUpdates([FromRoute] int n)
        {
            var list = await _aggregationService.GetNLastMeasurementAttemptsWithMdv(n);
            return list.Count == 0 ? NotFound() : Ok(list);
        }
    }
}
