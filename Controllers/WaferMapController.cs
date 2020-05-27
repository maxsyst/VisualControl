using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VueExample.Providers;
using VueExample.Services;
using VueExample.ViewModels;

namespace VueExample.Controllers
{
    [Route("api/[controller]/[action]")]
    public class WaferMapController : Controller
    {
        private readonly IDieProvider _dieProvider;
        private readonly IWaferMapProvider _waferMapProvider;

        public WaferMapController(IDieProvider dieProvider, IWaferMapProvider waferMapProvider)
        {
            _dieProvider = dieProvider;
            _waferMapProvider = waferMapProvider;
        }

        [HttpPost]
        [ResponseCache(CacheProfileName = "Default60")]
        public async Task<IActionResult> GetFormedWaferMap([FromBody] WaferMapFieldViewModel waferMapFieldViewModel)
        {
            var diesList = await _dieProvider.GetDiesByWaferId(waferMapFieldViewModel.WaferId);
            if (diesList.Count == 0)
            {
                return BadRequest();
            }
            var orientation = (await _waferMapProvider.GetByWaferId(waferMapFieldViewModel.WaferId)).Orientation;
            var waferMapFormed = new WaferMapFormationService(waferMapFieldViewModel.FieldHeight,
                waferMapFieldViewModel.FieldWidth, waferMapFieldViewModel.StreetSize, diesList).GetFormedWaferMap();
            return Ok(new {waferMapFormed, orientation});
        }
    }
}
