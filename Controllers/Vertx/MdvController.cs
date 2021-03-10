using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Vertx.InputModels;
using Vertx.Models;
using Vertx.Mongo.Abstract;
using Vertx.ResponseModels;

namespace Vertx.Controllers
{
    [Route("api/vertx/[controller]")]
    public class MdvController : Controller
    {
        private readonly IMdvService _mdvService;
        private readonly IMapper _mapper;

        public MdvController(IMapper mapper, IMdvService mdvService)
        {
            _mapper = mapper;
            _mdvService = mdvService;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateMdv([FromBody] MdvInputModel mdvInputModel)
        {
            var mdv = await _mdvService.CreateMdv(new Mdv
            {
                WaferId = mdvInputModel.WaferId, Code = mdvInputModel.Code, Description = mdvInputModel.Description
            });
            var response = _mapper.Map<MdvResponseModel>(mdv);
            return CreatedAtAction("CreateMdv", response);
        }


    }
}
