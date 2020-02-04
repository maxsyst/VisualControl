using Microsoft.AspNetCore.Mvc;
using VueExample.Providers;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.Services;

namespace VueExample.Controllers
{
    [Route("api/[controller]/[action]")]
    public class MassiveUploaderController : Controller
    {
        private readonly IWaferProvider _waferProvider;
        private readonly IDieProvider _dieProvider;
        public MassiveUploaderController(IWaferProvider waferProvider, IDieProvider dieProvider)
        {
            _dieProvider = dieProvider;
            _waferProvider = waferProvider;
        }
        [HttpGet]
        public IActionResult GetFolderDefects([FromQuery] string folderPath)
        {
            var massiveUploaderService = new MassiveUploaderService(_waferProvider, _dieProvider);
            return Ok(massiveUploaderService.FindDefectsInFolder(folderPath));
        }
    }
}
