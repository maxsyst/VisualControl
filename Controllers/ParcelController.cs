using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VueExample.Providers.Srv6.Interfaces;

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
        [Route("id/{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id) 
        {
            var parcel =  await _parcelProvider.GetById(id);
            return parcel == null ? NotFound() : Ok(parcel);
        }
        
        [HttpGet]
        [Route("waferid/{waferId}")]
        public async Task<IActionResult> GetByWaferId([FromRoute] string waferId) 
        {
            return Ok(await _parcelProvider.GetByWaferId(waferId));
        }
    }
}