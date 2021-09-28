using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace VueExample.Models.SRV6.Uploader
{
    public class UploadingFileGraphic4
    {
        [BindingBehavior(BindingBehavior.Required)]
        public string MeasurementRecordingName { get; set; }
        [BindingBehavior(BindingBehavior.Required)]
        public string WaferId { get; set; }
        [BindingBehavior(BindingBehavior.Required)]
        public string UploadingType { get; set; }
        [BindingBehavior(BindingBehavior.Required)]
        public string S2PParserMode{ get; set; }
        public UploadingFileData UploadingFileData { get; set; }
    }
}