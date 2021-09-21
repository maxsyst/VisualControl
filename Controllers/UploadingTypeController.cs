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
    }
}