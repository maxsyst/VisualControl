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
    public class ParcelController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IParcelProvider _parcelProvider;
        public ParcelController(IMapper mapper, IParcelProvider parcelProvider)
        {
            _mapper = mapper;
            _parcelProvider = parcelProvider;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ParcelViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("id/{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id) 
        {
            var parcel = await _parcelProvider.GetById(id);
            return parcel == null ? (IActionResult)NotFound() : Ok(parcel);          
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ParcelWithWafersViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("processId/{processId:int}")]
        public async Task<IActionResult> GetByProcessId([FromRoute] int processId) 
        {
            var parcelList = await _parcelProvider.GetByProcessId(processId);
            return parcelList.Count == 0 ? (IActionResult)NotFound() : Ok(parcelList);          
        }
        
        [HttpGet]
        [ProducesResponseType(typeof(ParcelViewModel), StatusCodes.Status200OK)]
        [Route("waferid/{waferId}")]
        public async Task<IActionResult> GetByWaferId([FromRoute] string waferId) 
        {
            return Ok(await _parcelProvider.GetByWaferId(waferId));
        }
    }
}