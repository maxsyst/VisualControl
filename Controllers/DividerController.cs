using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VueExample.Models.SRV6;
using VueExample.Providers.Srv6.Interfaces;

namespace VueExample.Controllers
{
    [Route("api/[controller]")]
    public class DividerController : Controller
    {
        private readonly IDividerService _dividerService;
        public DividerController(IDividerService dividerService)
        {
            _dividerService = dividerService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(Divider), StatusCodes.Status200OK)]
        [Route("all")]
        public async Task<IActionResult> GetAll() => Ok(await _dividerService.GetAll());
    }
}