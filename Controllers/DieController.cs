using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VueExample.Color;
using VueExample.Providers;
using VueExample.ViewModels;

namespace VueExample.Controllers
{
    [Route("api/[controller]/[action]")]
    public class DieController : Controller
    {
        private readonly IDieProvider _dieProvider;
        private readonly IColorService _colorService;

        public DieController(IDieProvider dieProvider, IColorService colorService)
        {
            _dieProvider = dieProvider;
            _colorService = colorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetByWaferId([FromQuery(Name = "waferid")] string waferId)
        {
            var diesList = await _dieProvider.GetDiesByWaferId(waferId);
            return Ok(diesList);
        }

        [HttpGet]
        [ResponseCache(CacheProfileName = "Default60")]
        public async Task<IActionResult> GetColorsByWaferId([FromQuery(Name = "waferid")] string waferId)
        {
            var dieColorList = new List<DieColorViewModel>();
            foreach (var die in await _dieProvider.GetDiesByWaferId(waferId))
            {
                dieColorList.Add(new DieColorViewModel{DieId = die.DieId, HexColor = _colorService.GetHexColorByDieId(die.DieId)});
            }            
            return Ok(dieColorList);
        }
    }
}
