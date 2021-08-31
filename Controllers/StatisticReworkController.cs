using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.StatisticsCoreRework.Abstract;

namespace VueExample.Controllers
{
    [Route ("api/statrwrk")]
    public class StatisticReworkController : Controller
    {
        private readonly IStatisticService _statisticService;
        private readonly IDieValueService _dieValueService;
        private readonly IDirtyCellsService _dirtyCellsService;
        public StatisticReworkController(IStatisticService statisticService, IDieValueService dieValueService, IDirtyCellsService dirtyCellsService)
        {
            _dieValueService = dieValueService;
            _dirtyCellsService = dirtyCellsService;
            _statisticService = statisticService;
        }

        [HttpGet]
        [Route("stat/{measurementRecordingId:int}")]
        public async Task<IActionResult> GetStatByMeasurementRecording ([FromRoute] int measurementRecordingId)
        {
            var s = await _statisticService.GetSingleParameterStatisticByMeasurementRecording(measurementRecordingId);
            return Ok(s);
        }

        [HttpGet]
        [Route("dirtyCellsSnapshot/{measurementRecordingId:int}")]
        public async Task<IActionResult> GetDirtyCellsSnapshot ([FromRoute] int measurementRecordingId)
        {
            var s = await _dirtyCellsService.GetDirtyCellsInitialSnapShotByMeasurementRecordingId(measurementRecordingId);
            return Ok(s);
        }

        [HttpGet]
        [Route("statisticSingleGraphic")]
        public async Task<IActionResult> GetStatisticSingleGraphic([FromQuery] string statisticSingleGraphicViewModelJSON)
        {
            var statisticSingleGraphicViewModel = JsonConvert.DeserializeObject<VueExample.ViewModels.StatisticSingleGraphicViewModel>(statisticSingleGraphicViewModelJSON);
            var (measurementRecordingId, keyGraphicState, _) = statisticSingleGraphicViewModel;
            string keyGraphic = statisticSingleGraphicViewModel.KeyGraphicState;
            var dict = await _statisticService.GetCalculatedStatisticByMeasurementRecordingGraphicStateAndDies(measurementRecordingId, keyGraphicState, double.Parse(statisticSingleGraphicViewModel.Divider, CultureInfo.InvariantCulture), statisticSingleGraphicViewModel.dieIdList.Select(x => (long)x).ToList());
            return Ok(dict.Values.ToList());
        }
    }
}