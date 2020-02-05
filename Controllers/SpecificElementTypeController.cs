using System.Collections.Generic;
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
    public class SpecificElementTypeController : Controller
    {
        private readonly ISpecificElementTypeService _specificElementTypeService;
        private readonly IMapper _mapper;
        public SpecificElementTypeController(ISpecificElementTypeService specificElementTypeService, IMapper mapper)
        {
            _specificElementTypeService = specificElementTypeService;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(SpecificElementTypeViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("id/{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id) 
            => Ok(_mapper.Map<SpecificElementType, SpecificElementTypeViewModel>(await _specificElementTypeService.GetById(id)));

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SpecificElementTypeViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("elementid/{elementTypeId:int}")]
        public async Task<IActionResult> GetByElementId([FromRoute] int elementTypeId)
            => Ok(_mapper.Map<IEnumerable<SpecificElementType>, IEnumerable<SpecificElementTypeViewModel>>(await _specificElementTypeService.GetByElementTypeId(elementTypeId)));
        
        [HttpPut]
        [ProducesResponseType(typeof(SpecificElementTypeViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] SpecificElementTypeViewModel specificElementTypeViewModel)
           => CreatedAtAction("Create", _mapper.Map<SpecificElementType, SpecificElementTypeViewModel>(await _specificElementTypeService.Create(specificElementTypeViewModel)));

        [HttpPatch]
        [ProducesResponseType(typeof(SpecificElementTypeViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] 
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] SpecificElementTypeViewModel specificElementTypeViewModel)
            => Ok(_mapper.Map<SpecificElementType, SpecificElementTypeViewModel>(await _specificElementTypeService.Update(specificElementTypeViewModel)));

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("id/{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await _specificElementTypeService.Delete(id);
            return NoContent();
        }
    }
}