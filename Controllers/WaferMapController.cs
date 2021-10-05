using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VueExample.Providers;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.ViewModels;

namespace VueExample.Controllers
{
    [Route("api/[controller]/[action]")]
    public class WaferMapController : Controller
    {
        private readonly IDieProvider _dieProvider;
        private readonly IWaferMapService _waferMapService;

        public WaferMapController(IDieProvider dieProvider, IWaferMapService waferMapService)
        {
            _dieProvider = dieProvider;
           _waferMapService = waferMapService;
        }

        [HttpGet]
        [ResponseCache(CacheProfileName = "Default60")]
        public async Task<IActionResult> GetFormedWaferMap([FromQuery] string waferMapFieldViewModelJSON)
        {
            var waferMapFieldViewModel = JsonConvert.DeserializeObject<WaferMapFieldViewModel>(waferMapFieldViewModelJSON);
            var formedMap = await _waferMapService.GetFormedMap(waferMapFieldViewModel);
            return Ok(formedMap);
        }
    }
}
