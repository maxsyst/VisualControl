using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VueExample.Models;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.ViewModels;

namespace VueExample.Controllers
{
    [Route("api/[controller]")]
    public class ProcessController : Controller
    {
        private readonly IProcessProvider _processProvider;
        private readonly IMapper _mapper;
        public ProcessController(IMapper mapper, IProcessProvider processProvider)
        {
            _processProvider = processProvider;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ProcessViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("waferid/{waferId}")]
        public async Task<IActionResult> GetByWaferId([FromRoute] string waferId)
        {
            var process = _mapper.Map<Process, ProcessViewModel>(await _processProvider.GetByWaferId(waferId));
            return Ok(process);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<ProcessViewModel>), StatusCodes.Status200OK)]
        [Route("all")]
        public async Task<IActionResult> GetAll() => Ok(_mapper.Map<List<Process>, List<ProcessViewModel>>(await _processProvider.GetAll()));

        [HttpGet]
        [ProducesResponseType(typeof(ProcessViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ResponseCache(CacheProfileName = "Default60")]
        [Route("dietype/{dieTypeId:int}")]
        public async Task<IActionResult> GetByDieTypeId(int dieTypeId)
            => Ok(_mapper.Map<Process, ProcessViewModel>(await _processProvider.GetProcessByDieTypeId(dieTypeId)));      
    }
}