using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VueExample.Providers.Srv6.Interfaces;

namespace VueExample.Controllers
{
    [Route("api/[controller]")]
    public class UploadingTypeController : Controller
    {
        private readonly IUploadingTypeService _uploadingTypeService;
        public UploadingTypeController(IUploadingTypeService uploadingTypeService)
        {
            _uploadingTypeService = uploadingTypeService;
        }
        
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetStatisticSingleGraphic()
        {
           var uploadingTypeList = await _uploadingTypeService.GetAll();
           return uploadingTypeList.Count > 0 ? Ok(uploadingTypeList) : (IActionResult)NotFound();
        }
    }
}