using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models.SRV6.Uploader;
using VueExample.ViewModels;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface IFolderService
    {
        List<string> GetAllCodeProductInUploaderDirectory(string directoryPath);
        List<string> GetAllWaferInFolder(string directoryPath);
        bool IsWaferExistsInFolder(string directoryPath, string waferId);
        List<string> GetAllWaferInCodeProductFolder(string directoryPath, string codeProductFolderName);
        List<string> GetAllMeasurementRecordingFolder(string directoryPath, string codeProductFolderName, string waferFolderName);
        Dictionary<string, UploadingFileData> GetDataFromLNRFile(string path);
        Dictionary<string, UploadingFileData> GetDataFromHSTGFile(string path);
        Task GetGraphic4(UploadingFileGraphic4 uploadingFile);
        Task<List<SimpleOperationUploaderViewModel>> GetSimpleOperations(string directoryPath, string codeProductName, string waferName, int dieTypeId, List<string> measurementRecordings);
    }
}