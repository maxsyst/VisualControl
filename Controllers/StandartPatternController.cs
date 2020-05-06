using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using VueExample.Models.SRV6;
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
        [ProducesResponseType(typeof(StandartPattern), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Route("creating")]
        public async Task<IActionResult> Create([FromBody] JObject standartMeasurementPatternFullJObject)
        {
            var smpViewModel = standartMeasurementPatternFullJObject.ToObject<StandartMeasurementPatternFullViewModel>();
            var standartPattern = await _standartPatternService.CreateFull(smpViewModel);
            return CreatedAtAction("Creating", standartPattern);
        }

        [HttpPost]
        [ProducesResponseType(typeof(StandartPattern), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Route("updating")]
        public async Task<IActionResult> Update([FromBody] JObject standartMeasurementPatternFullJObject)
        {
            var smpViewModel = standartMeasurementPatternFullJObject.ToObject<StandartMeasurementPatternFullViewModel>();
            var standartPattern = await _standartPatternService.Update(smpViewModel);
            return CreatedAtAction("Updating", standartPattern);
        }

        [HttpGet]
        [ProducesResponseType(typeof(StandartMeasurementPatternFullViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("smp/{patternId:int}")]
        public async Task<IActionResult> GetSmpByPatternId([FromRoute] int patternId)
        {
            var standartPatternFull = await _standartPatternService.GetFull(patternId);
            return Ok(standartPatternFull);
        }

        [HttpGet]
        [ProducesResponseType(typeof(StandartPatternViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("name/{name}")]
        public async Task<IActionResult> GetByName([FromRoute] string name)
        {
            return Ok(_mapper.Map<StandartPattern, StandartPatternViewModel>(await _standartPatternService.GetByName(name)));
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