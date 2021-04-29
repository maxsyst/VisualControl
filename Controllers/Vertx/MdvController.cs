using System;
using System.Linq;
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

        [HttpGet]
        [Route("wafers/all")]
        public async Task<IActionResult> GetAllWafers() 
        {
            var mdvList = await _mdvService.GetAll();
            var wafersList = mdvList.Select(x => x.WaferId).Distinct().ToList();
            return wafersList.Count == 0 ? (IActionResult)NotFound() : Ok(wafersList);
        }
        
        [HttpGet]
        [Route("waferId/{waferId}")]
        public async Task<IActionResult> GetByWaferId([FromRoute] string waferId) 
        {
            var mdvList = await _mdvService.GetByWafer(waferId);
            return mdvList.Count == 0 ? (IActionResult)NotFound() : Ok(mdvList);
        }

        [HttpGet]
        [Route("waferId/{waferId}/code/{code}")]
        public async Task<IActionResult> GetByWaferIdAndCode([FromRoute] string waferId, [FromRoute] string code) 
        {
            var mdv = await _mdvService.GetByWaferAndCode(waferId, code);
            return mdv == null ? (IActionResult)NotFound() : Ok(mdv);
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
