using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VueExample.Helpers;
using VueExample.Models.SRV6;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.Services.Abstract;
using VueExample.StatisticsCore.Abstract;
using VueExample.ViewModels;

namespace VueExample.Controllers
{
    [Route("api/[controller]")]
    
    public class MeasurementRecordingController : Controller
    {
        private readonly AppSettings _appSettings;
        private readonly IMeasurementRecordingService _measurementRecordingService;
        private readonly IStageProvider _stageProvider;
        private readonly IElementService _elementService;
        private readonly IExportProvider _exportProvider;
        private readonly IGraphic4Service _graphic4Service;
        private readonly IMapper _mapper;   
       
        public MeasurementRecordingController(IOptions<AppSettings> appSettings, IGraphic4Service graphic4Service, IElementService elementService, IMapper mapper, IExportProvider exportProvider, IStageProvider stageProvider, IMeasurementRecordingService measurementRecordingService)
        {
            _appSettings = appSettings.Value;
            _graphic4Service = graphic4Service;
            _elementService = elementService;
            _stageProvider = stageProvider;
            _measurementRecordingService = measurementRecordingService;
            _mapper = mapper;
            _exportProvider = exportProvider;
        }

        [HttpGet]
        [ProducesResponseType(typeof(MeasurementRecording), StatusCodes.Status200OK)]
        [Route("")]
        public async Task<IActionResult> GetMeasurementRecordingsByWaferId([FromQuery] string waferId)
        {
            return Ok(await _measurementRecordingService.GetByWaferId(waferId));
        }

        [HttpPost]
        [ProducesResponseType(typeof(MeasurementRecording), StatusCodes.Status201Created)]
        [Route("getorcreate")]
        public async Task<IActionResult> GetOrCreate([FromBody] MeasurementRecordingWithBigMeasurementViewModel measurementRecordingWithBigMeasurementViewModel)
        {
            var measurementRecording = await _measurementRecordingService.GetOrCreate(measurementRecordingWithBigMeasurementViewModel.Name, 
                                                                                2, 
                                                                                measurementRecordingWithBigMeasurementViewModel.BmrId, 
                                                                                measurementRecordingWithBigMeasurementViewModel.StageId);
            return Ok(measurementRecording);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Route("delete/{measurementRecordingId:int}")]        
        public async Task<IActionResult> Delete([FromRoute] int measurementRecordingId, [FromQuery(Name = "superuser")] string superuser)
        {
            if(_appSettings.SuperUser == superuser) {
                await _measurementRecordingService.Delete(measurementRecordingId);
                return NoContent();
            }
            return Forbid();
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Route("delete/graphic4/{measurementRecordingId:int}")]        
        public async Task<IActionResult> Delete4([FromRoute] int measurementRecordingId)
        {
            return await _graphic4Service.DeleteGraphic4(measurementRecordingId) ? (IActionResult)NoContent() : (IActionResult)NotFound();
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Route("delete/list")]        
        public async Task<IActionResult> DeleteList([FromBody] List<int> measurementIdList)
        {
            await _measurementRecordingService.DeleteSet(measurementIdList);
            return NoContent();
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Route("deletespecific/{measurementRecordingId:int}/{graphicId:int}")]        
        public async Task<IActionResult> DeleteSpecificMeasurement([FromRoute] int measurementRecordingId, [FromRoute] int graphicId)
        {
            await _measurementRecordingService.DeleteSpecificMeasurement(measurementRecordingId, graphicId);
            return NoContent();
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Route("deletespecificmultiply/{measurementRecordingId:int}/{graphicIdString}")]        
        public async Task<IActionResult> DeleteSpecificMeasurement([FromRoute] int measurementRecordingId, [FromRoute] string graphicIdString)
        {
            var graphicIdArray = graphicIdString.Split('$').Select(x => Convert.ToInt32(x)).ToList();
            await _measurementRecordingService.DeleteSpecificMultiplyMeasurement(measurementRecordingId, graphicIdArray);
            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(typeof(MeasurementRecording), StatusCodes.Status200OK)]
        [Route("update-stage")]
        public async Task<IActionResult> UpdateStage([FromBody] StageMeasurementRecordingChunkViewModel stageMeasurementRecordingChunkViewModel)
        {
            var measurementRecording = await _measurementRecordingService.UpdateStage(stageMeasurementRecordingChunkViewModel.MeasurementRecordingId, 
                                                                                     stageMeasurementRecordingChunkViewModel.StageId);
            return Ok(measurementRecording);
        }

        
        [HttpPost]
        [ProducesResponseType(typeof(BigMeasurementRecording), StatusCodes.Status200OK)]
        [Route("bmr/getorcreate")]
        public async Task<IActionResult> GetOrAddBigMeasurement([FromBody] BigMeasurementRecordingViewModel bigMeasurementRecordingViewModel)
        {
            var bigMeasurementRecording = await _measurementRecordingService.GetOrAddBigMeasurement(bigMeasurementRecordingViewModel.Name, bigMeasurementRecordingViewModel.WaferId);
            return Ok(bigMeasurementRecording);
        }  

        [HttpPost]
        [ProducesResponseType(typeof(MeasurementRecording), StatusCodes.Status200OK)]
        [Route("edit/name")]
        public async Task<IActionResult> UpdateName([FromBody] MeasurementRecordingViewModel measurementRecordingViewModel)
        {
            var measurementRecording = await _measurementRecordingService.UpdateName(measurementRecordingViewModel.Id, measurementRecordingViewModel.Name);
            return Ok(measurementRecording);  
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<StageFullViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Route("wafer/{waferid}/dietype/{dieTypeId:int}")]
        public async Task<IActionResult> GetMeasurementRecordingWithStagesByWaferId([FromRoute] string waferId, [FromRoute] int dieTypeId)
        {
            var measurementRecordingList = dieTypeId == 0 ? (await _measurementRecordingService.GetByWaferId(waferId)).GroupBy(x => x.Id).Select(x => x.FirstOrDefault()).ToList() 
                                                          : (await _measurementRecordingService.GetByWaferIdAndDieType(waferId, dieTypeId)).Distinct().ToList();
            if(measurementRecordingList.Count == 0)
            {
                return NoContent();
            }
            var stagesFullViewModelList = new List<StageFullViewModel>();
            var stagesList = measurementRecordingList.Select(x => x.StageId ?? 0).Distinct().ToList();
            stagesList.Remove(0);
            stagesList.Insert(0,0);
            foreach (var stage in stagesList)
            {
                stagesFullViewModelList.Add(new StageFullViewModel{
                    Id = stage,
                    Name = stage == 0 ? "Этап не выбран" : (await _stageProvider.GetById(stage)).StageName,
                    MeasurementRecordingList = new List<MeasurementRecordingWithStageAndElementViewModel>()
                });
            }

            foreach (var measurementRecording in measurementRecordingList)
            {
                var elementList = await _elementService.GetByIdmr(measurementRecording.Id);
                var thisStage = stagesFullViewModelList.FirstOrDefault(x => x.Id == (measurementRecording.StageId ?? 0));
                thisStage.MeasurementRecordingList.Add(new MeasurementRecordingWithStageAndElementViewModel {
                                                        Id = measurementRecording.Id, 
                                                        Name = measurementRecording.Name, 
                                                        Element = elementList.Select(x => _mapper.Map<ElementViewModel>(x)).ToList().FirstOrDefault()});
            }

            return Ok(stagesFullViewModelList);
        }
      

        [HttpGet]
        [ProducesResponseType(typeof(MeasurementRecording), StatusCodes.Status200OK)]
        [Route("{id:int}")]
        public IActionResult GetMeasurementRecordingById([FromRoute] int id)
        {
            return Ok(_measurementRecordingService.GetById(id));
        }

        

        [HttpGet]
        [ProducesResponseType(typeof(List<MeasurementRecordingViewModel>), StatusCodes.Status200OK)]
        [Route("getbyelement")]
        public async Task<IActionResult> GetByWaferIdAndStageNameAndElementName([FromQuery] string waferId, [FromQuery] string stageName, [FromQuery] string elementName)
        {
            var element = await _elementService.GetByNameAndWafer(elementName, waferId);
            var mrList = new List<MeasurementRecordingViewModel>();
            if(element is null)
                return (IActionResult)NotFound();
            var measurementRecordingsList = await _measurementRecordingService.GetByWaferIdAndStageNameAndElementId(waferId, stageName, element.ElementId);
            foreach (var measurementRecording in measurementRecordingsList)
            {
                mrList.Add(new MeasurementRecordingViewModel {Id = measurementRecording.Id, 
                                                              Name = measurementRecording.Name, 
                                                              WaferId = waferId,
                                                              avStatisticParameters = await _exportProvider.GetStatisticsNameByMeasurementId(measurementRecording.Id, 1.5)});
            }
            return mrList.Count == 0 ? (IActionResult)NotFound() : Ok(mrList);
        }        
    }
}