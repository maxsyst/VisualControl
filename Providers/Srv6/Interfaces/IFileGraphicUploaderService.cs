using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models.SRV6.Uploader;
using VueExample.ViewModels;
using VueExample.ViewModels.FileNameUploader;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface IFileGraphicUploaderService
    {
        Task<IList<FileName>> GetAllFileNamesByProcessId(int processId);
        Task<IList<GraphicNameUploaderViewModel>> GetGraphicsByFileName(int fileNameId);
        Task<FileName> CreateFileName(FileNameUploaderViewModel fileNameViewModel);
        Task<GraphicName> AddGraphicToFileName(int fileNameId, GraphicNameUploaderViewModel graphicNameUploaderViewModel);
        Task CopyFileNamesToAnotherProcess(int sourceProcessId, int destProcessId);
        Task DeleteFileName(int fileNameId, int processId);
        Task DeleteGraphicFromFileName(int fileNameId, GraphicNameUploaderViewModel graphicNameUploaderViewModel);
    }
}