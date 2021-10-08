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
    public class CodeProductController : Controller
    {
        private readonly ICodeProductProvider _codeProductProvider;
        private readonly IMapper _mapper;
        public CodeProductController(IMapper mapper, ICodeProductProvider codeProductProvider)
        {
            _mapper = mapper;
            _codeProductProvider = codeProductProvider;
        }

        [HttpGet]
        [ProducesResponseType(typeof(CodeProductViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("getbywaferid")]
        public async Task<IActionResult> GetByWaferId([FromQuery] string waferId)
        {
            var codeProduct = await _codeProductProvider.GetByWaferId(waferId);
            return codeProduct is null ? (IActionResult)NotFound() : Ok(_mapper.Map<CodeProduct, CodeProductViewModel>(codeProduct));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<CodeProductViewModel>), StatusCodes.Status200OK)]
        [Route("processid/{processId:int}")]
        public async Task<IActionResult> GetByProcessId([FromRoute] int processId)
        {
            return Ok(_mapper.Map<IList<CodeProduct>, IList<CodeProductViewModel>>(await _codeProductProvider.GetByProcessId(processId)));
        }
        
        [HttpGet]
        public async Task<IActionResult> GetCodeProductsByDieType()
        {
            //TODO: Implement Realistic Implementation
            await Task.Yield();
            return Ok();
        }
    }
}
