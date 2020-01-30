using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VueExample.Models;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.ViewModels;
using AutoMapper;

namespace VueExample.Controllers
{
    [Route("api/[controller]")]
    public class StageController: Controller
    {
        private readonly IProcessProvider _processProvider;
        private readonly IStageProvider _stageProvider;
        private readonly IMapper _mapper;
        public StageController(IStageProvider stageProvider, IMapper mapper, IProcessProvider processProvider)
        {
            _stageProvider = stageProvider;
            _processProvider = processProvider;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType (typeof(List<Stage>), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status404NotFound)]
         [Route("process/{processId:int}")]
        public async Task<IActionResult> GetStagesByProcessId([FromRoute] int processId)

            => Ok(await _stageProvider.GetStagesByProcessId(processId));
        
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
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] StageViewModel stageViewModel)       
           
            => CreatedAtAction("create", await _stageProvider.Create(_mapper.Map<StageViewModel, Stage>(stageViewModel)));           

        [HttpPut]
        [ProducesResponseType (typeof(Stage), StatusCodes.Status201Created)]
        [Route("create/name/{name}/codeProductId/{codeProductId:int}")]
        public async Task<IActionResult> Create([FromRoute] string name, [FromRoute] int codeProductId)
        {
           var process = await _processProvider.GetProcessByCodeProductId(codeProductId);
           var newStage = await _stageProvider.Create(name, process.ProcessId);          
           return CreatedAtAction("create", newStage);
        }

        [HttpPost]
        [ProducesResponseType (typeof(Stage), StatusCodes.Status201Created)]
        [ProducesResponseType (StatusCodes.Status404NotFound)]     
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] StageViewModel stageViewModel) 
        
            => Created("update", await _stageProvider.Update(_mapper.Map<StageViewModel, Stage>(stageViewModel)));
       
        [HttpDelete]
        [ProducesResponseType (StatusCodes.Status204NoContent)]
        [ProducesResponseType (StatusCodes.Status403Forbidden)]
        [ProducesResponseType (StatusCodes.Status404NotFound)]               
        [Route("delete/{stageId:int}")]
        public async Task<IActionResult> Delete([FromRoute] int stageId)
        {
            await _stageProvider.Delete(stageId);
            return NoContent();
        }

    }
}
