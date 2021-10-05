using Microsoft.AspNetCore.Mvc.ModelBinding;
using VueExample.ViewModels;

namespace VueExample.Models.SRV6.Uploader
{
    public class UploadingFileGraphic4
    {
        [BindingBehavior(BindingBehavior.Required)]
        public string MeasurementRecordingName { get; set; }
        [BindingBehavior(BindingBehavior.Required)]
        public string WaferId { get; set; }
        [BindingBehavior(BindingBehavior.Required)]
        public UploadingTypeViewModel UploadingType { get; set; }
        [BindingBehavior(BindingBehavior.Required)]
        public string S2PParserMode{ get; set; }
        [BindingBehavior(BindingBehavior.Required)]
        public string UserName{ get; set; }
        [BindingBehavior(BindingBehavior.Required)]
        public int StageId{ get; set; }
        [BindingBehavior(BindingBehavior.Required)]
        public int ElementId{ get; set; }
    }
}