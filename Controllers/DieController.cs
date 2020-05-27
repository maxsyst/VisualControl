using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VueExample.Providers;

namespace VueExample.Controllers
{
    [Route("api/[controller]/[action]")]
    public class DieController : Controller
    {
        readonly IDieProvider _dieProvider;

        public DieController(IDieProvider dieProvider)
        {
            _dieProvider = dieProvider;
        }

        [HttpGet]
        public async Task<IActionResult> GetByWaferId([FromQuery(Name = "waferid")] string waferId)
        {
            var graphic = await _dieProvider.GetDiesByWaferId(waferId);
            return Ok(graphic);
        }
    }
}
