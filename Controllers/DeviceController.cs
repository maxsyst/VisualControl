using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using VueExample.Models;
using VueExample.Providers.ChipVerification.Abstract;
using VueExample.ResponseObjects;
using VueExample.ViewModels;

namespace VueExample.Controllers
{
    [Route ("api/[controller]/")]
    public class DeviceController : Controller
    {
        private readonly IDeviceProvider _deviceProvider;
        private readonly IMapper _mapper;
        public DeviceController (IDeviceProvider deviceProvider, IMapper mapper)
        {
            _deviceProvider = deviceProvider;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<DeviceViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<Error>), StatusCodes.Status404NotFound)]
        [Route("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _deviceProvider.GetAll();
            return result.HasErrors ? (IActionResult)NotFound(result.GetErrors()) : (IActionResult)Ok(result.TObject);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<Device>), StatusCodes.Status200OK)]
        [Route("av/measurementid/{measurementId:int}")]
        public async Task<IActionResult> GetAvailableByMeasurementId([FromRoute] int measurementId)
        {
            var deviceList = await _deviceProvider.GetAvailableByMeasurementId(measurementId);
            return Ok(deviceList);
        }

        [HttpGet]
        [ProducesResponseType(typeof(DeviceViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<Error>), StatusCodes.Status404NotFound)]
        [Route("name/{name}")]
        public async Task<IActionResult> GetByName([FromRoute] string name)
        {
            var result = await _deviceProvider.GetByName(name);
            return result.HasErrors ? (IActionResult)NotFound(result.GetErrors()) : (IActionResult)Ok(_mapper.Map<DeviceViewModel>(result.TObject));
        }

        [HttpGet]
        [ProducesResponseType(typeof(DeviceViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<Error>), StatusCodes.Status404NotFound)]
        [Route("address/{address}")]
        public async Task<IActionResult> GetByAddress([FromRoute] string address)
        {
            var result = await _deviceProvider.GetByAddress(address);
            return result.HasErrors ? (IActionResult)NotFound(result.GetErrors()) : (IActionResult)Ok(_mapper.Map<DeviceViewModel>(result.TObject));
        }
        
    /// <remarks>
    /// Sample
    ///
    /// DeviceViewModel:
    /// {
    ///     name: string,
    ///     address: string,
    ///     model: string
    /// }
    /// </remarks>
        [HttpPut]
        [ProducesResponseType(typeof(DeviceViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseObjects.Error), StatusCodes.Status409Conflict)]
        [Route("")]
        public async Task<IActionResult> Create([FromBody] JObject deviceViewModel)
        {
            var result = await _deviceProvider.Create(deviceViewModel.ToObject<DeviceViewModel>());
            return result.HasErrors ? (IActionResult)Conflict(result.GetErrors()) : (IActionResult)CreatedAtAction("Create", _mapper.Map<DeviceViewModel>(result.TObject));
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseObjects.Error), StatusCodes.Status409Conflict)]
        [Route("{deviceId:int}")]
        public async Task<IActionResult> Delete([FromRoute] int deviceId)
        {
            var result = await _deviceProvider.Delete(deviceId);
            return result.HasErrors ? (IActionResult)Conflict(result.GetErrors()) : (IActionResult)NoContent();
        }

        [HttpPost]
        [ProducesResponseType(typeof(DeviceViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseObjects.Error), StatusCodes.Status409Conflict)]
        [Route("edit")]
        public async Task<IActionResult> Edit([FromBody] DeviceViewModel deviceViewModel)
        {
            var result = await _deviceProvider.Edit(deviceViewModel);
            return result.HasErrors ? (IActionResult)Conflict(result.GetErrors()) : (IActionResult)Ok(_mapper.Map<DefectViewModel>(result.TObject));
        }
    }
}