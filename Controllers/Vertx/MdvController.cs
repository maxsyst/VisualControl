using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VueExample.Exceptions;
using VueExample.Models.Vertx;
using VueExample.Services.Vertx.Abstract;
using VueExample.ViewModels.Vertx.InputModels;
using VueExample.ViewModels.Vertx.ResponseModels;

namespace VueExample.Controllers.Vertx
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
            if(String.IsNullOrEmpty(mdvInputModel.Code) || String.IsNullOrEmpty(mdvInputModel.WaferId)) 
            {
                return BadRequest();
            }
            MdvResponseModel mdvResponse;
            try
            {
                var mdv = await _mdvService.CreateMdv(new Mdv
                {
                    WaferId = mdvInputModel.WaferId,
                    Code = mdvInputModel.Code,
                    Description = mdvInputModel.Description
                });
                mdvResponse = _mapper.Map<MdvResponseModel>(mdv);        
            }
            catch(DuplicateException e) 
            {
                return (IActionResult)Conflict(e);
            }    
            return CreatedAtAction("CreateMdv", mdvResponse);
        }


    }
}
