using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VueExample.Entities;
using VueExample.Providers.Abstract;
using VueExample.ViewModels;

namespace VueExample.Controllers
{
    [Route("api/[controller]")]
    public class StandartWaferController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IStandartWaferProvider _standartWaferProvider;
        public StandartWaferController(IMapper mapper, IStandartWaferProvider standartWaferProvider)
        {
            _mapper = mapper;
            _standartWaferProvider = standartWaferProvider;
        }

        [HttpPost]
        [ProducesResponseType(typeof(List<StandartWaferViewModel>), StatusCodes.Status200OK)]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] List<StandartWaferViewModel> standartWafers)
        {
            await _standartWaferProvider.Delete((int)standartWafers.FirstOrDefault().CodeProductId);
            var createdStandartWafers = await _standartWaferProvider.Create(_mapper.Map<List<StandartWaferViewModel>, List<CodeProductStandartWafer>>(standartWafers));
            return Ok(_mapper.Map<List<CodeProductStandartWafer>, List<StandartWaferViewModel>>(createdStandartWafers));
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<StandartWaferViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("codeProductId/{codeProductId:int}")]
        public async Task<IActionResult> GetByCodeProductId([FromRoute] int codeProductId)
        {
           return Ok(await _standartWaferProvider.GetByCodeProduct(codeProductId));
        }
    }
}