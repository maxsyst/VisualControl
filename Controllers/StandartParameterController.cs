using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VueExample.Models.SRV6;
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
            => Ok(_mapper.Map<StandartParameterModel, StandartParameterViewModel>(await _standartParameterService.GetById(standartParameterId)));

        [HttpPut]
        [ProducesResponseType(typeof(StandartParameterViewModel), StatusCodes.Status201Created)]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] StandartParameterViewModel standartParameterViewModel)
            => CreatedAtAction("Create", _mapper.Map<StandartParameterModel, StandartParameterViewModel>(await _standartParameterService.Create(_mapper.Map<StandartParameterViewModel, StandartParameterModel>(standartParameterViewModel))));
     
        [HttpPatch]
        [ProducesResponseType(typeof(StandartParameterViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] StandartParameterViewModel standartParameterViewModel)
            => Ok(_mapper.Map<StandartParameterModel, StandartParameterViewModel>(await _standartParameterService.Update(_mapper.Map<StandartParameterViewModel, StandartParameterModel>(standartParameterViewModel))));

        [HttpDelete]
        [ProducesResponseType(typeof(StandartParameterViewModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("delete/{standartParameterId:int}")]
        public async Task<IActionResult> Delete([FromRoute] int standartParameterId)
        {
            await _standartParameterService.Delete(standartParameterId);
            return NoContent();
        }
    }
}