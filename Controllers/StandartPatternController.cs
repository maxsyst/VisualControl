using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.ViewModels;

namespace VueExample.Controllers
{
    [Route("api/[controller]")]
    public class StandartPatternController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IStandartPatternService _standartPatternService;
        public StandartPatternController(IMapper mapper, IStandartPatternService standartPatternService)
        {
            _standartPatternService = standartPatternService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] JObject standartMeasurementPatternFullJObject)
        {
            var standartMeasurementPatternFullViewModel = standartMeasurementPatternFullJObject.ToObject<StandartMeasurementPatternFullViewModel>();
            await _standartPatternService.CreateFull(standartMeasurementPatternFullViewModel);
            return CreatedAtAction("Create", null);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<StandartPatternViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("dietype/{dieTypeId:int}")]
        public async Task<IActionResult> GetByDieTypeId([FromRoute] int dieTypeId)
            => Ok(await _standartPatternService.GetByDieTypeId(dieTypeId));

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await _standartPatternService.Delete(id);
            return NoContent();
        }
        
    }   
}