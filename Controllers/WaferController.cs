using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VueExample.Models;
using VueExample.Providers;
using VueExample.Providers.Srv6.Interfaces;

namespace VueExample.Controllers
{
    [Route("api/[controller]/[action]")]
    public class WaferController : Controller
    {
        private readonly IWaferProvider _waferProvider;
        private readonly DefectProvider _defectProvider = new DefectProvider();
        
        public WaferController(IWaferProvider waferProvider)
        {
            _waferProvider = waferProvider;
        }

        [HttpGet]
        [ProducesResponseType (typeof(List<Wafer>), StatusCodes.Status200OK)]
        [Route("all")]
        public async Task<IActionResult> GetAll()
        {
           return Ok(await _waferProvider.GetWafers());
        }

        [HttpGet]
        public IActionResult GetAllWithDefects()
        {
            return Ok(_defectProvider.GetAll().Select(x=>x.WaferId).Distinct());
        }
    }
}
