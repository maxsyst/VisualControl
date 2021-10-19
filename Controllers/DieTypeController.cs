using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using VueExample.Models.SRV6;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.ViewModels;
using VueExample.ViewModels.DieType;

namespace VueExample.Controllers
{
    [Route("api/[controller]")]
    public class DieTypeController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IDieTypeProvider _dieTypeProvider;
        public DieTypeController(IDieTypeProvider dieTypeProvider, IMapper mapper)
        {
            _mapper = mapper;
            _dieTypeProvider = dieTypeProvider;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<DieTypeViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ResponseCache(CacheProfileName = "Default60")]
        [Route("all")]
        public async Task<IActionResult> GetAll()
        {
            var dieTypeList = await _dieTypeProvider.GetAll();
            return dieTypeList.Count > 0 ? Ok(_mapper.Map<List<DieType>, List<DieTypeViewModel>>(dieTypeList)) : (IActionResult)NotFound();
        }

        [HttpGet]
        [ProducesResponseType(typeof(DieTypeViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("name/{name}")]
        public async Task<IActionResult> GetByName([FromRoute] string name)
        {
            var dieType = await _dieTypeProvider.GetByName(name);
            return dieType.DieTypeId == 0 ? (IActionResult)NotFound() : Ok(_mapper.Map<DieType, DieTypeViewModel>(dieType));
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("")]
        public async Task<IActionResult> Create([FromBody] JObject dieTypeUpdatingJObject)
        {
            var dieTypeUpdatingViewModel = dieTypeUpdatingJObject.ToObject<DieTypeUpdatingViewModel>();
            var dieType = await _dieTypeProvider.Create(dieTypeUpdatingViewModel);
            return Created("", dieType.Name);
        }

        [HttpPost]
        [ProducesResponseType(typeof(DieTypeViewModel), StatusCodes.Status200OK)]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] DieTypeViewModel dieTypeViewModel)
        {
            var dieType = await _dieTypeProvider.Update(dieTypeViewModel);
            return Ok(_mapper.Map<DieType, DieTypeViewModel>(dieType));
        }

        [HttpPost]
        [Route("codeproduct/update-fk")]
        public async Task<IActionResult> UpdateCodeProductsMap([FromBody] JObject DieTypeCodeProductJObject)
        {
            var dieTypeCodeProduct = DieTypeCodeProductJObject.ToObject<DieTypeCodeProduct>();
            var tuple = await _dieTypeProvider.UpdateCodeProductsMap(dieTypeCodeProduct.DieTypeId, dieTypeCodeProduct.CodeProductId);
            return tuple.Item2 == "ERROR" ? (IActionResult)NotFound() : (tuple.Item2 == "DELETED" ? Ok(tuple.Item1.Name) : (IActionResult)Created("", tuple.Item1.Name));
        }

        [HttpGet]
        [ProducesResponseType(typeof(DieTypeUpdatingViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("cp-el/{id:int}")]
        public async Task<IActionResult> GetCodeProductsAndElements([FromRoute] int id)
        {
            var dieTypeUpdatingViewModel = await _dieTypeProvider.GetCodeProductsAndElements(id);
            return dieTypeUpdatingViewModel is null ? (IActionResult)NotFound() : Ok(dieTypeUpdatingViewModel);
        }

        [HttpGet]
        [ProducesResponseType(typeof(DieTypeViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("wafer/{waferId}")]
        public async Task<IActionResult> GetByWaferId([FromRoute] string waferId)
        {
            var dieTypes = await _dieTypeProvider.GetByWaferId(waferId);
            return !dieTypes.Any() ? (IActionResult)NotFound() : Ok(dieTypes);
        }
    }
}