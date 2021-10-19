using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using VueExample.Exceptions;
using VueExample.Models.Vertx;
using VueExample.Services.Vertx.Abstract;
using VueExample.ViewModels.Vertx.InputModels;
using VueExample.ViewModels.Vertx.ResponseModels;

namespace VueExample.Controllers.Vertx
{
    [Route("api/vertx/[controller]")]
    public class MeasurementController : Controller
    {
        private readonly IMdvService _mdvService;
        private readonly IMeasurementAttemptService _measurementAttemptService;
        private readonly IMeasurementService _measurementService;
        private readonly IMapper _mapper;

        public MeasurementController(IMapper mapper, IMeasurementService measurementService, IMdvService mdvService, IMeasurementAttemptService measurementAttemptService)
        {
            _mapper = mapper;
            _mdvService = mdvService;
            _measurementAttemptService = measurementAttemptService;
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
                return Conflict(e);
            }
            return CreatedAtAction("CreateMeasurement", measurementResponseModel);
        }

        [HttpPost("create/withmdv")]
        public async Task<IActionResult> CreateMeasurementWithMdv([FromBody] MeasurementWithMdvInputModel measurementWithMdvInputModel)
        {
            var mdv = await _mdvService.GetByWaferAndCode(measurementWithMdvInputModel.WaferId, measurementWithMdvInputModel.Code);
            if (mdv == null)
            {
                return (IActionResult)NotFound();
            }
            var measurementAttempt = await _measurementAttemptService.GetMasterByMdvId(mdv.Id.ToString());
            var measurementInputModel = measurementWithMdvInputModel.MeasurementInputModel;
            measurementInputModel.MeasurementAttemptId = measurementAttempt.Id.ToString();
            if(measurementInputModel.CreationDate.Ticks == 0)
            {
                measurementInputModel.CreationDate = DateTime.Now;
            }
            var measurement = _mapper.Map<Measurement>(measurementInputModel);
            MeasurementResponseModel measurementResponseModel;
            try
            {
                measurementResponseModel =
                    _mapper.Map<MeasurementResponseModel>(await _measurementService.Create(measurement, new ObjectId(measurementInputModel.MeasurementAttemptId)));
            }
            catch (DuplicateException e)
            {
                return Conflict(e);
            }
            return CreatedAtAction("CreateMeasurement", measurementResponseModel);
        }

        [HttpGet]
        [Route("generate/name/waferId/{waferId}/code/{code}")]
        public async Task<IActionResult> GenerateNameByMdv([FromRoute] string waferId, [FromRoute] string code)
        {
            var mdv = await _mdvService.GetByWaferAndCode(waferId, code);
            if(mdv == null) {
                return (IActionResult)NotFound();
            }
            var measurementAttempts = await _measurementAttemptService.GetByMdvId(mdv.Id.ToString());
            var measurementCounter = 0;
            foreach (var measurementAttempt in measurementAttempts)
            {
                measurementCounter = measurementCounter + measurementAttempt.MeasurementsId.Count();
            }
            var measurementName = $"{waferId}&&{code}&&Test{measurementCounter + 1}";
            return String.IsNullOrEmpty(measurementName)
                ? (IActionResult)NotFound()
                : Ok(measurementName);
        }

        [HttpGet]
        [Route("id/{measurementId}")]
        public async Task<IActionResult> GetById([FromRoute] string measurementId)
        {
            var measurement = await _measurementService.GetById(new ObjectId(measurementId));
            return measurement is null
                ? (IActionResult)NotFound()
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
                ? (IActionResult)NotFound()
                : Ok(measurementResponseList);
        }

        [HttpGet]
        [Route("id/{measurementId}/characteristics")]
        public async Task<IActionResult> GetAllCharacteristics([FromRoute] string measurementId)
        {
            var measurementList = await _measurementService.GetAllCharacteristics(new ObjectId(measurementId));
            return measurementList.Count == 0
                ? (IActionResult)NotFound()
                : Ok(measurementList);
        }

        [HttpGet]
        [Route("name/{measurementName}")]
        public async Task<IActionResult> GetByName([FromRoute] string measurementName)
        {
            var measurement = await _measurementService.GetByName(measurementName);
            return measurement is null
                ? (IActionResult)NotFound()
                : Ok(_mapper.Map<MeasurementResponseModel>(measurement));
        }
    }
}