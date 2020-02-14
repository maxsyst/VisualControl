using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.ViewModels;

namespace VueExample.Controllers
{
    [Route("auth/[controller]")]
    public class StandartPatternController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IStandartPatternService _standartPatternService;
        public StandartPatternController(IMapper mapper, IStandartPatternService standartPatternService)
        {
            _standartPatternService = standartPatternService;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<StandartPatternViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("dietype/{dieTypeId:int}")]
        public async Task<IActionResult> GetByDieTypeId([FromRoute] int dieTypeId)
            => Ok(await _standartPatternService.GetByDieTypeId(dieTypeId));
        
    }
}