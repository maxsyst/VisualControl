using Microsoft.AspNetCore.Mvc;
using VueExample.Providers;
using VueExample.Providers.Abstract;

namespace VueExample.Controllers
{
    [Route("api/[controller]/[action]")]
    public class DangerLevelController : Controller
    {
        private readonly IDangerLevelProvider _dangerLevelProvider;
        private readonly IDefectProvider _defectProvider;

        public DangerLevelController(IDefectProvider defectProvider, IDangerLevelProvider dangerLevelProvider)
        {
            _defectProvider = defectProvider;
            _dangerLevelProvider = dangerLevelProvider;
        }

        // [HttpGet]
        // public IActionResult GetAll()
        // {
        //     try
        //     {
        //         return Ok(_dangerLevelProvider.GetAll());
        //     }
        //     catch (Exception e)
        //     {
        //         Console.WriteLine(e);
        //         return StatusCode(500);
        //     }
        // }

        // [HttpGet]
        // public IActionResult GetByWaferId(string waferId)
        // {
        //     return Ok(JsonConvert.SerializeObject(_dangerLevelProvider.GetDangerLevelFromDefectList(_defectProvider.GetByWaferIdWithIncludes(waferId))));
        // }

        // [HttpGet]
        // [ProducesResponseType(200)]
        // public async Task<IActionResult> GetById(int dangerLevelId)
        // {
        //     var dangerLevel = await _dangerLevelProvider.GetByIdAsync(dangerLevelId);
        //     return CreatedAtAction(nameof(GetById), dangerLevel);
        // }
    }
}
