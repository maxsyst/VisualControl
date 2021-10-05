using System.Collections.Generic;
using VueExample.Entities;
using VueExample.ViewModels;

namespace VueExample.Models.SRV6.Uploader
{
    public class UploadingFileStatus
    {
        public string Guid { get; set; }
        public StageViewModel Stage { get; set; }
        public IList<FkMrGraphic> AlreadyData { get; set; }
        public string UploadStatus { get; set; }
    }
}