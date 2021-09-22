using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models.SRV6.Uploader;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface IUploaderService
    {
        Task<string> Uploading(UploadingFile uploadingFile, int type);
        Task<IList<UploadingFileStatus>> CheckStatus(IList<UploadingFile> uploadingFiles);
        Task<string> CheckStatusGraphic4(string waferId, string measurementRecordingName);
    }
}