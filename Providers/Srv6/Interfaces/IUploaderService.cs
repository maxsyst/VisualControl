using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models.SRV6;
using VueExample.Models.SRV6.Uploader;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface IUploaderService
    {
        Task<string> Uploading(UploadingFile uploadingFile, int type);
        Task<string> UploadingGraphic4(UploadingFileGraphic4 uploadingFile);
        Task<IList<UploadingFileStatus>> CheckStatus(IList<UploadingFile> uploadingFiles);
        Task<MeasurementRecordingStatus> CheckStatusGraphic4(string waferId, string measurementRecordingName);
    }
}