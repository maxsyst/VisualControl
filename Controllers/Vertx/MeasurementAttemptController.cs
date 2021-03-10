using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Vertx.InputModels;
using Vertx.Models;
using Vertx.Mongo.Abstract;
using Vertx.ResponseModels;
using VueExample.Exceptions;

namespace Vertx.Controllers
{
    [Route("api/vertx/[controller]")]
    public class MeasurementAttemptController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IMeasurementAttemptService _measurementAttemptService;
        public MeasurementAttemptController(IMapper mapper, IMeasurementAttemptService measurementAttemptService)
        {
            _mapper = mapper;
            _measurementAttemptService = measurementAttemptService;
        }
        
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] MeasurementAttemptInputModel measurementAttemptInputModel)
        {
            var measurementAttempt = _mapper.Map<MeasurementAttempt>(measurementAttemptInputModel);
            MeasurementAttemptResponseModel measurementAttemptResponseModel;
            try
            {
                measurementAttemptResponseModel =
                    _mapper.Map<MeasurementAttemptResponseModel>(await _measurementAttemptService.CreateMeasurementAttempt(measurementAttempt));
            }
            catch (DuplicateException e)
            {
                return Conflict();
            }

            return CreatedAtAction("Create", measurementAttemptResponseModel);
        }
        
        [HttpGet]
        [Route("name/{measurementName}/mdvId/{mdvId}")]
        public async Task<IActionResult> GetByNameAndMdvId([FromRoute] string measurementName, [FromRoute] string mdvId)
        {
            var measurementAttempt = await _measurementAttemptService.GetByNameAndMdvId(measurementName, mdvId);
            return measurementAttempt is null
                ? (IActionResult) NotFound()
                : Ok(_mapper.Map<MeasurementAttemptResponseModel>(measurementAttempt));
        }
        
        [HttpGet]
        [Route("mdvId/{mdvId}")]
        public async Task<IActionResult> GetByMdvId([FromRoute] string mdvId)
        {
            var measurementAttemptList = await _measurementAttemptService.GetByMdvId(mdvId);
            return measurementAttemptList.Count == 0
                ? (IActionResult) NotFound()
                : Ok(measurementAttemptList);
        }
        
    }
}