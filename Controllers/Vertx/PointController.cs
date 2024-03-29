﻿using System.Globalization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using VueExample.Models.Vertx;
using VueExample.Services.Vertx.Abstract;
using VueExample.ViewModels.Vertx.InputModels;
using VueExample.ViewModels.Vertx.ResponseModels;
using System.Linq;

namespace Vertx.Controllers
{
    [Route("api/vertx/[controller]")]
    public class PointController : Controller
    {
        private readonly IPointService _pointService;
        private readonly IMapper _mapper;
        public PointController(IMapper mapper, IPointService pointService)
        {
            _pointService = pointService;
            _mapper = mapper;
        }

        [HttpGet("measurementId/{measurementId}/characteristicName/{characteristicName}/sifted/{siftedK:int}/withoutbadpoints/{withoutBadPoints}")]
        public async Task<IActionResult> GetByMeasurement([FromRoute] string measurementId, [FromRoute] string characteristicName, [FromRoute] int siftedK, [FromRoute] bool withoutBadPoints)
        {
            var pointsDictionary = await _pointService.GetByMeasurement(new ObjectId(measurementId), characteristicName, siftedK, withoutBadPoints);
            return pointsDictionary.Count == 0 ? (IActionResult) NotFound() : Ok(pointsDictionary);
        }

        [HttpGet("measurementId/{measurementId}/characteristicName/{characteristicName}/seconds/{seconds:int}")]
        public async Task<IActionResult> GetByMeasurementFirstSeconds([FromRoute] string measurementId, [FromRoute] string characteristicName, [FromRoute] int seconds)
        {
            var pointsDictionary = await _pointService.GetByMeasurementFirstSeconds(new ObjectId(measurementId), characteristicName, seconds);
            return pointsDictionary.Count == 0 ? (IActionResult) NotFound() : Ok(pointsDictionary);
        }

        [HttpGet("measurementAttemptId/{measurementAttemptId}/characteristicName/{characteristicName}/sifted/{siftedK:int}/withoutbadpoints/{withoutBadPoints}/date")]
        public async Task<IActionResult> GetByMeasurementAttemptTrueDateSifted([FromRoute] string measurementAttemptId, [FromRoute] string characteristicName, [FromRoute] int siftedK, [FromRoute] bool withoutBadPoints)
        {
            var pointsDictionary = await _pointService.GetByMeasurementAttemptTrueDate(new ObjectId(measurementAttemptId), characteristicName, siftedK, withoutBadPoints);
            return pointsDictionary.Count == 0 ? (IActionResult) NotFound() : Ok(pointsDictionary);
        }

        [HttpGet("measurementAttemptId/{measurementAttemptId}/characteristicName/{characteristicName}/sifted/{siftedK:int}/withoutbadpoints/{withoutBadPoints}/duration")]
        public async Task<IActionResult> GetByMeasurementAttemptDurationSifted([FromRoute] string measurementAttemptId, [FromRoute] string characteristicName, [FromRoute] int siftedK, [FromRoute] bool withoutBadPoints)
        {
            var pointsDictionary = await _pointService.GetByMeasurementAttemptDuration(new ObjectId(measurementAttemptId), characteristicName, siftedK, withoutBadPoints);
            return pointsDictionary.Count == 0 ? (IActionResult) NotFound() : Ok(pointsDictionary);
        }

        [HttpGet("measurementAttemptId/{measurementAttemptId}/characteristicName/{characteristicName}/seconds/{seconds:int}")]
        public async Task<IActionResult> GetByMeasurementAttemptFirstSeconds([FromRoute] string measurementAttemptId, [FromRoute] string characteristicName, [FromRoute] int seconds)
        {
            var pointsDictionary = await _pointService.GetByMeasurementAttemptFirstSeconds(new ObjectId(measurementAttemptId), characteristicName, seconds);
            return pointsDictionary.Count == 0 ? (IActionResult) NotFound() : Ok(pointsDictionary);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePoint([FromBody] PointInputModel pointInputModel)
        {
            var creationDate = pointInputModel.CreationDate == null ? DateTime.Now : Convert.ToDateTime(pointInputModel.CreationDate);
            if (double.TryParse(pointInputModel.Value, out var parsedNumber))
            {
                var point = await _pointService.Create(parsedNumber, pointInputModel.Characteristic,
                pointInputModel.MeasurementName, pointInputModel.IsNewSet, creationDate);
                return CreatedAtAction("CreatePoint", _mapper.Map<PointResponseModel>(point));
            }
            return BadRequest();
        }

        [HttpPost("create/batch")]
        public async Task<IActionResult> CreatePointBatch([FromBody] PointBatchInputModel pointBatchInputModel)
        {
            var pointsList = new List<PointResponseModel>();
            foreach (var characteristicWithValue in pointBatchInputModel.CharacteristicWithValues)
            {
                var creationDate = pointBatchInputModel.CreationDate == null ? DateTime.Now : Convert.ToDateTime(pointBatchInputModel.CreationDate);
                var corTest = characteristicWithValue.Value;
                corTest = string.Concat(corTest.Where(x => x != '.' && x != ',' && x != 'E' &&  x != 'e').ToList());
                if (corTest.Any(c => char.IsDigit(c)))
                {
                    pointsList.Add(_mapper.Map<PointResponseModel>(await _pointService.Create(
                    Convert.ToDouble(characteristicWithValue.Value, CultureInfo.InvariantCulture),
                    new Characteristic(characteristicWithValue.Name, characteristicWithValue.Unit),
                    pointBatchInputModel.MeasurementName, pointBatchInputModel.IsNewSet, creationDate)));
                }                
            }
            return CreatedAtAction("CreatePointBatch", pointsList);
        }
    }
}
