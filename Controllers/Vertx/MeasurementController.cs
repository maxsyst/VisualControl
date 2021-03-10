using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Vertx.InputModels;
using Vertx.Models;
using Vertx.Mongo.Abstract;
using Vertx.ResponseModels;
using VueExample.Exceptions;

namespace Vertx.Controllers
{
    [Route("api/vertx/[controller]")]
    public class MeasurementController : Controller
    {
        private readonly IMeasurementService _measurementService;
        private readonly IMapper _mapper;

        public MeasurementController(IMapper mapper, IMeasurementService measurementService)
        {
            _mapper = mapper;
            _measurementService = measurementService;
        }
        
        [HttpPost("create")]
        public async Task<IActionResult> CreateMeasurement([FromBody] MeasurementInputModel measurementInputModel)
        {
            var measurement = _mapper.Map<Measurement>(measurementInputModel);
            MeasurementResponseModel measurementResponseModel;
            try
            {
                measurementResponseModel =
                    _mapper.Map<MeasurementResponseModel>(await _measurementService.Create(measurement, new ObjectId(measurementInputModel.MeasurementAttemptId)));
            }
            catch (DuplicateException e)
            {
                return Conflict();
            }

            return CreatedAtAction("CreateMeasurement", measurementResponseModel);
        }

        [HttpGet]
        [Route("id/{measurementId}")]
        public async Task<IActionResult> GetById([FromRoute] string measurementId)
        {
            var measurement = await _measurementService.GetById(new ObjectId(measurementId));
            return measurement is null
                ? (IActionResult) NotFound()
                : Ok(_mapper.Map<MeasurementResponseModel>(measurement));
        }
        
        [HttpGet]
        [Route("measurementAttemptId/{measurementAttemptId}")]
        public async Task<IActionResult> GetByMeasurementAttemptId([FromRoute] string measurementAttemptId)
        {
            var measurementResponseList = _mapper.Map<List<MeasurementResponseModel>>((await _measurementService.GetByMeasurementAttemptId(new ObjectId(measurementAttemptId))).OrderBy(x => x.CreationDate).ToList());
            var currentDuration = 0;
            foreach (var measurement in measurementResponseList)
            {
                measurement.DurationPreSeconds = currentDuration;
                currentDuration += measurement.DurationSeconds;
            }
            return measurementResponseList.Count == 0
                ? (IActionResult) NotFound()
                : Ok(measurementResponseList);
        }
        
        [HttpGet]
        [Route("id/{measurementId}/characteristics")]
        public async Task<IActionResult> GetAllCharacteristics([FromRoute] string measurementId)
        {
            var measurementList = await _measurementService.GetAllCharacteristics(new ObjectId(measurementId));
            return measurementList.Count == 0
                ? (IActionResult) NotFound()
                : Ok(measurementList);
        }

        [HttpGet]
        [Route("name/{measurementName}")]
        public async Task<IActionResult> GetByName([FromRoute] string measurementName)
        {
            var measurement = await _measurementService.GetByName(measurementName);
            return measurement is null
                ? (IActionResult) NotFound()
                : Ok(_mapper.Map<MeasurementResponseModel>(measurement));
        }

    }
}