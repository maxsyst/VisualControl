using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.StatisticsCoreRework.Abstract;
using System.Collections.Generic;
using VueExample.StatisticsCoreRework.Models;

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
        [Route("dirtyCellsSnapshot/{measurementRecordingId:int}/initial")]
        public async Task<IActionResult> GetDirtyCellsSnapshot ([FromRoute] int measurementRecordingId)
        {
            var s = await _dirtyCellsService.GetDirtyCellsInitialSnapShotByMeasurementRecordingId(measurementRecordingId);
            return Ok(s);
        }

        [HttpGet]
        [Route("dirtyCellsSnapshot/{measurementRecordingId:int}/{keyGraphicState}/withprofile")]
        public async Task<IActionResult> GetDirtyCellsSnapshotWithProfile ([FromRoute] int measurementRecordingId, [FromRoute] string keyGraphicState, [FromQuery] string dcProfilesJSON)
        {
            var dcProfiles = JsonConvert.DeserializeObject<List<DirtyCellsProfile>>(dcProfilesJSON);
            var singleGraphicDirtyCells = await _dirtyCellsService.GetDirtyCellsShortsByKeyGraphicState(measurementRecordingId, keyGraphicState, dcProfiles);
            var dcNewProfiles = singleGraphicDirtyCells.StatNameDirtyCellsDictionary.Select(s => new DirtyCellsProfile {StatName = s.Key, Type = s.Value.Type , LowBorder = s.Value.LowBorder, TopBorder = s.Value.TopBorder, K = dcProfiles.FirstOrDefault(d => d.StatName == s.Key).K}).ToList(); 
            return Ok(new { singleGraphicDirtyCells = singleGraphicDirtyCells, dcProfiles = dcNewProfiles});
        }

        [HttpGet]
        [Route("statisticSingleGraphic")]
        public async Task<IActionResult> GetStatisticSingleGraphic([FromQuery] string statisticSingleGraphicViewModelJSON)
        {
            var statisticSingleGraphicViewModel = JsonConvert.DeserializeObject<VueExample.ViewModels.StatisticSingleGraphicViewModel>(statisticSingleGraphicViewModelJSON);
            var (measurementRecordingId, keyGraphicState, _) = statisticSingleGraphicViewModel;
            var dict = await _statisticService.GetCalculatedStatisticByMeasurementRecordingGraphicStateAndDies(measurementRecordingId, keyGraphicState, double.Parse(statisticSingleGraphicViewModel.Divider, CultureInfo.InvariantCulture), statisticSingleGraphicViewModel.dieIdList.Select(x => (long)x).ToList());
            return Ok(dict.Values.ToList());
        }
    }
}