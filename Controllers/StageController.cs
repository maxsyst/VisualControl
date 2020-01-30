using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VueExample.Models;
using VueExample.Providers;
using VueExample.Providers.Srv6.Interfaces;

namespace VueExample.Controllers
{
    [Route("api/[controller]")]
    public class StageController: Controller
    {
        private readonly ProcessProvider _processProvider;
        private readonly IStageProvider _stageProvider;
        public StageController(IStageProvider stageProvider)
        {
            _stageProvider = stageProvider;
        }

        [HttpGet]
        [ProducesResponseType (typeof(List<Stage>), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status404NotFound)]
        [Route("codeproduct/{codeproductid:int}")]
        public async Task<IActionResult> GetStagesByCodeProductId([FromRoute] int codeProductId) 
               
            => Ok(await _stageProvider.GetStagesByProcessId((await _processProvider.GetProcessByCodeProductId(codeProductId)).ProcessId));
       
        [HttpGet]
        [ProducesResponseType (typeof(List<Stage>), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status404NotFound)]
        [Route("wafer/{waferId}")]
        public async Task<IActionResult> GetStagesByWaferId([FromRoute] string waferId) 
        
            => Ok(await _stageProvider.GetStagesByWaferId(waferId));

        [HttpGet]
        [ProducesResponseType (typeof(Stage), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status404NotFound)]
        [Route("id/{stageId:int}")]
        public async Task<IActionResult> GetById([FromRoute] int stageId)
        
            => Ok(await _stageProvider.GetById(stageId));

        [HttpPut]
        [ProducesResponseType (typeof(Stage), StatusCodes.Status201Created)]
        [Route("create/name/{name}/codeProductId/{codeProductId:int}")]
        public async Task<IActionResult> Create([FromRoute] string name, [FromRoute] int codeProductId)
        {
           var process = await _processProvider.GetProcessByCodeProductId(codeProductId);
           var newStage = await _stageProvider.Create(name, process.ProcessId);          
           return CreatedAtAction("", newStage);
        }

    }
}
