using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VueExample.Models.SRV6;
using VueExample.Providers.Abstract;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.ResponseObjects;
using VueExample.ViewModels;

namespace VueExample.Controllers
{
    [Route("api/[controller]")]
    public class ShortLinkController : Controller
    {
        private readonly IShortLinkProvider _shortLinkProvider;
        private readonly IMeasurementRecordingService _measurementRecordingService;
        public ShortLinkController(IShortLinkProvider shortLinkProvider, IMeasurementRecordingService measurementRecordingService)
        {
            _measurementRecordingService = measurementRecordingService;
            _shortLinkProvider = shortLinkProvider;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ShortLink), StatusCodes.Status200OK)]
        [Route("guid/{generatedId}")]
        public async Task<IActionResult> GetByGeneratedId([FromRoute] string generatedId)
        {
            var shortLink = await _shortLinkProvider.GetByGeneratedId(generatedId);
            return shortLink.SelectedDies.Count == 0 ? (IActionResult)NotFound() : Ok(shortLink);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<MeasurementRecordingViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<Error>), StatusCodes.Status404NotFound)]
        [Route("{shortlink}/element-export")]
        public async Task<IActionResult> GetElementExportDetails([FromRoute] string shortLink)
        {
            var shortLinkInfo = await _shortLinkProvider.GetElementExportDetails(shortLink);
            return shortLinkInfo.HasErrors ? (IActionResult)NotFound(shortLinkInfo.GetErrors()) 
                                           : Ok(new List<MeasurementRecordingViewModel> 
                                                {new MeasurementRecordingViewModel 
                                                {Id = shortLinkInfo.TObject.MeasurementRecordingId, 
                                                Name = (await _measurementRecordingService.GetById(shortLinkInfo.TObject.MeasurementRecordingId)).Name, 
                                                WaferId = shortLinkInfo.TObject.WaferId,
                                                avStatisticParameters = shortLinkInfo.TObject.StatisticNameList}});
                      
        }

        [HttpPost]
        [Route("generate")]
        public async Task<IActionResult> GenerateShortLink([FromBody] ShortLinkGenerateViewModel shortLinkGenerateViewModel)
        {
            var shortLink = await _shortLinkProvider.CreateSRV3(shortLinkGenerateViewModel);
            return CreatedAtAction("Generate", shortLink);
        }
    }
}