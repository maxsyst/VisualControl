using Microsoft.AspNetCore.Mvc;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.Services;

namespace VueExample.Controllers
{
    [Route("api/[controller]/[action]")]
    public class MassiveUploaderController : Controller
    {
        private readonly IWaferProvider _waferProvider;
        public MassiveUploaderController(IWaferProvider waferProvider)
        {
            _waferProvider = waferProvider;
        }
        [HttpGet]
        public IActionResult GetFolderDefects([FromQuery] string folderPath)
        {
            var massiveUploaderService = new MassiveUploaderService(_waferProvider);
            return Ok(massiveUploaderService.FindDefectsInFolder(folderPath));
        }
    }
}
