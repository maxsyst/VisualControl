using System.Collections.Generic;
using VueExample.Models.SRV6.Uploader.Models;

namespace VueExample.ViewModels
{
    public class UploadingTypeViewModel
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public IList<Graphic4Type> Graphics { get; set; }
    }
}