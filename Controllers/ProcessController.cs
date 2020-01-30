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
        [ProducesResponseType(typeof(IList<ProcessViewModel>), StatusCodes.Status200OK)]
        [Route("all")]
        public async Task<IActionResult> GetAll() => Ok(_mapper.Map<List<Process>, List<ProcessViewModel>>(await _processProvider.GetAll()));
               
    }
}