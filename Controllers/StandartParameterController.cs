using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.ViewModels;

namespace VueExample.Controllers
{   
    [Route("api/[controller]")]
    public class StandartParameterController : Controller
    {
        private readonly IStandartParameterService _standartParameterService;
        private readonly IMapper _mapper;
        public StandartParameterController(IStandartParameterService standartParameterService, IMapper mapper)
        {
            _standartParameterService = standartParameterService;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(StandartParameterViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("id/{standartParameterId:int}")]
        public async Task<IActionResult> GetById([FromRoute] int standartParameterId)
        {
            //TODO: Implement Realistic Implementation
            await Task.Yield();
            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StandartParameterViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("processid/{processId:int}")]
        public async Task<IActionResult> GetByProcess([FromRoute] int processId)
        {
            //TODO: Implement Realistic Implementation
            await Task.Yield();
            return Ok();
        }

        [HttpPut]
        [ProducesResponseType(typeof(StandartParameterViewModel), StatusCodes.Status201Created)]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] StandartParameterViewModel standartParameterViewModel)
        {
            //TODO: Implement Realistic Implementation
            await Task.Yield();
            return Ok();
        }

        [HttpPatch]
        [ProducesResponseType(typeof(StandartParameterViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] StandartParameterViewModel standartParameterViewModel)
        {
            await Task.Yield();
            return Ok();
        }

        [HttpDelete]
        [ProducesResponseType(typeof(StandartParameterViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("delete/{standartParameterId:int}")]
        public async Task<IActionResult> Delete([FromRoute] int standartParameterId)
        {
            //TODO: Implement Realistic Implementation
            await Task.Yield();
            return Ok();
        }
    }
}