using Microsoft.AspNetCore.Mvc;
using VueExample.Providers;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.Providers.Abstract;

namespace VueExample.Controllers
{
    [Route("api/[controller]/[action]")]
    public class DefectFilterController : Controller
    {
        private IStageProvider _stageProvider;
        private IDieProvider  _dieProvider;
        private IDefectTypeProvider _defectTypeProvider;
        private IDangerLevelProvider _dangerLevelProvider;
        
        public DefectFilterController(IDieProvider dieProvider, IStageProvider stageProvider, IDefectTypeProvider defectTypeProvider, IDangerLevelProvider dangerLevelProvider)
        {
            _dieProvider = dieProvider;
            _defectTypeProvider = defectTypeProvider;
            _dangerLevelProvider = dangerLevelProvider;
            _stageProvider = stageProvider;
        }

        // [HttpGet]
        // public async Task<IActionResult> GetDefectFilter([FromQuery] string defects)
        // {
        //     var defectList = JsonConvert.DeserializeObject<List<Defect>>(defects);
        //     if (defectList.Count == 0)
        //     {
        //         return Ok(new StandardResponseObject { ResponseType = "warning", ErrorCode = "DFC001" });
        //     }
        //     try
        //     {
        //         var defectFilter = new DefectFilterViewModel
        //         {
        //             AvbDangerLevelList = _dangerLevelProvider.GetAll().FindAll(d =>
        //                 defectList.Select(x => x.DangerLevelId).Distinct().Contains(d.DangerLevelId)),
        //             AvbDefectTypesList = _defectTypeProvider.GetAll().FindAll(d =>
        //                 defectList.Select(x => x.DefectTypeId).Distinct().Contains(d.DefectTypeId)),
        //             AvbStagesList = (await _stageProvider.GetAll()).FindAll(d =>
        //                 defectList.Select(x => x.StageId).Distinct().Contains(d.StageId)),
        //             AvbDiesList = (await _dieProvider.GetAll()).FindAll(d =>
        //                 defectList.Select(x => x.DieId).Distinct().Contains(d.DieId))
        //         };

        //         return Ok(new StandardResponseObject<DefectFilterViewModel> { ResponseType = "success", Body = defectFilter, ErrorCode = "OK"});
        //     }
        //     catch (Exception e)
        //     {
        //         Console.WriteLine(e);
        //         return Ok(new StandardResponseObject {ResponseType = "error", ErrorCode = "UE001"});

        //     }
          

        // }
    }
}
