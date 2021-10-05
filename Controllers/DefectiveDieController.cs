using Microsoft.AspNetCore.Mvc;
using VueExample.Providers;
using VueExample.Providers.Abstract;

namespace VueExample.Controllers
{
    [Route("api/[controller]/[action]")]
    public class DefectiveDieController : Controller
    {
        private readonly IDefectProvider _defectProvider;
        private readonly DefectiveDieProvider _defectiveDieProvider = new DefectiveDieProvider();
        private readonly IDefectTypeProvider _defectTypeProvider;
        private readonly IDangerLevelProvider _dangerLevelProvider;

        public DefectiveDieController(IDefectProvider defectProvider, IDefectTypeProvider defectTypeProvider, IDangerLevelProvider dangerLevelProvider)
        {
            _defectProvider = defectProvider;
            _defectTypeProvider = defectTypeProvider;
            _dangerLevelProvider = dangerLevelProvider;
        }

        // [HttpGet]
        // public IActionResult GetByDangerLevel(string waferId, int dangerLevelId)
        // {
        //     var defectsList = _defectProvider.GetByWaferId(waferId);
        //     var dangerLevel = _dangerLevelProvider.GetById(dangerLevelId);
        //     var defectiveDies = _defectiveDieProvider.GetByDangerLevel(defectsList, dangerLevel);
        //     return Ok(defectiveDies);
        // }

        // [HttpGet]
        // public IActionResult GetByDefectType(string waferId, int defectTypeId, int dangerLevelId)
        // {
        //     var defectsList = _defectProvider.GetByWaferId(waferId);
        //     var defectType = _defectTypeProvider.GetById(defectTypeId);
        //     var dangerLevel = _dangerLevelProvider.GetById(dangerLevelId);
        //     var defectiveDies = _defectiveDieProvider.GetBadByDefectType(defectsList, defectType, dangerLevel);
        //     return Ok(defectiveDies);
        // }



    }
}
