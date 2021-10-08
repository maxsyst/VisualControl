using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VueExample.Models;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.ViewModels;

namespace VueExample.Controllers
{
    [Route("api/[controller]")]
    public class FolderController : Controller
    {
        private readonly IFolderService _folderService;
        private readonly IMapper _mapper;
        private readonly ICodeProductProvider _codeProductProvider;
        private readonly IWaferProvider _waferProvider;
        public FolderController(IFolderService folderService, IMapper mapper, ICodeProductProvider codeProductProvider, IWaferProvider waferProvider)
        {
            _folderService = folderService;
            _waferProvider = waferProvider;
            _codeProductProvider = codeProductProvider;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<CodeProductFolderViewModel>), StatusCodes.Status200OK)]
        [Route("folders-cp")]
        public async Task<IActionResult> GetCodeProductFoldersStrict()
        {
            var resultList = new List<CodeProductFolderViewModel>();
            var directoryPath = ExtraConfiguration.UploadingPath; 
            var codeProducts = await _codeProductProvider.GetAll();
            var directoriesList = _folderService.GetAllCodeProductInUploaderDirectory(directoryPath);
            foreach (var directoryName in directoriesList)
            {
                var codeProductFolderViewModel = new CodeProductFolderViewModel();
                var codeProduct = codeProducts.FirstOrDefault(x => x.CodeProductName == directoryName);
                codeProductFolderViewModel.FolderName = directoryName;
                if(codeProduct != null)
                {
                    codeProductFolderViewModel.CodeProduct = _mapper.Map<CodeProduct, CodeProductViewModel>(codeProduct);
                    codeProductFolderViewModel.Warning = false;
                }
                resultList.Add(codeProductFolderViewModel);
            }
            return Ok(resultList);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<SimpleOperationUploaderViewModel>), StatusCodes.Status200OK)]
        [Route("simpleoperation/{codeProductName}/{waferName}/{dietypeid:int}")]
        public async Task<IActionResult> GetSimpleOperations([FromRoute] string codeProductName, [FromRoute] string waferName, [FromRoute] int dietypeid, [FromQuery] string measurementRecordingsJSON)
        {
            var simpleOperations = await _folderService.GetSimpleOperations(ExtraConfiguration.UploadingPath, 
                                                                            codeProductName, 
                                                                            waferName, 
                                                                            dietypeid,
                                                                            JsonConvert.DeserializeObject<List<string>>(measurementRecordingsJSON));
            return Ok(simpleOperations);
        }

        [HttpGet]
        [Route("filedata/lnr")]
        public IActionResult GetFileLNRData([FromQuery] string path)
        {
           return Ok(_folderService.GetDataFromLNRFile(path));
        }

        [HttpGet]
        [Route("iswaferexist/graphic4/{waferId}")]
        public IActionResult IsExistWaferInFolder([FromRoute] string waferId)
        {
           var directoryPath = ExtraConfiguration.UploadingGraphic4Path;
           var isExist = _folderService.IsWaferExistsInFolder(directoryPath, waferId);
           return Ok(isExist);
        }

        [HttpGet]
        [Route("filedata/hstg")]
        public IActionResult GetFileHSTGData([FromQuery] string path)
        {
           return Ok(_folderService.GetDataFromHSTGFile(path));
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<WaferFolderViewModel>), StatusCodes.Status200OK)]
        [Route("folders-wafer/{codeProductFolderName}")]
        public async Task<IActionResult> GetWaferFolders([FromRoute] string codeProductFolderName)
        {
            var resultList = new List<WaferFolderViewModel>();
            var directoryPath = ExtraConfiguration.UploadingPath;
            var directoriesList = _folderService.GetAllWaferInCodeProductFolder(directoryPath, codeProductFolderName);
            var wafers = await _waferProvider.GetWafers();
            foreach (var directoryName in directoriesList)
            {
                var waferFolderViewModel = new WaferFolderViewModel();
                var wafer = wafers.FirstOrDefault(x => x.WaferId == directoryName);
                waferFolderViewModel.FolderName = directoryName;
                if(wafer != null)
                {
                    waferFolderViewModel.Wafer = _mapper.Map<Wafer, WaferViewModel>(wafer);
                    waferFolderViewModel.Disabled = false;
                }
                resultList.Add(waferFolderViewModel);
            }
            return Ok(resultList);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        [Route("folders-idmr/{codeProductFolderName}/{wafer}")]
        public IActionResult GetMeasurementRecordings([FromRoute] string codeProductFolderName, string wafer)
        {
            var directoryPath = ExtraConfiguration.UploadingPath;
            var directoriesList = _folderService.GetAllMeasurementRecordingFolder(directoryPath, codeProductFolderName, wafer);
            return Ok(directoriesList);
        }
    }
}