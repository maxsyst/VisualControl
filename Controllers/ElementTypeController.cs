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
    public class ElementTypeController : Controller
    {
        private readonly IElementTypeService _elementTypeService;
        private readonly IMapper _mapper;
        public ElementTypeController(IElementTypeService elementTypeService, IMapper mapper)
        {
            _elementTypeService = elementTypeService;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(TypeElementViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("id/{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
            => Ok(_mapper.Map<ElementType, TypeElementViewModel>(await _elementTypeService.GetById(id)));

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TypeElementViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status404NotFound)]
        [Route("all")]
        public async Task<IActionResult> GetAll()
            => Ok(_mapper.Map<IEnumerable<ElementType>, IEnumerable<TypeElementViewModel>>(await _elementTypeService.GetAll()));

        [HttpPut]
        [ProducesResponseType(typeof(TypeElementViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType (StatusCodes.Status403Forbidden)]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] TypeElementViewModel typeElementViewModel)
            => CreatedAtAction("create", _mapper.Map<ElementType, TypeElementViewModel>(await _elementTypeService.Create(typeElementViewModel.Name)));

        [HttpPatch]
        [ProducesResponseType(typeof(TypeElementViewModel), StatusCodes.Status200OK)]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] TypeElementViewModel typeElementViewModel)
            => Ok(_mapper.Map<ElementType, TypeElementViewModel>(await _elementTypeService.Update(_mapper.Map<TypeElementViewModel, ElementType>(typeElementViewModel))));
    }
}