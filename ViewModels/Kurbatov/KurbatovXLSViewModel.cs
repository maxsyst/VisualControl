using System.Collections.Generic;
using VueExample.Models.SRV6.Export;

namespace VueExample.ViewModels.Kurbatov
{
    public class KurbatovXLSViewModel
    {
        public string OperationNumber { get; set; }
        public string ElementName { get; set; }
        public string StageName {get; set; }
        public bool IsAddedToCommonWorksheet { get; set; } = true;
        public bool IsNeedToCopyS2P { get; set; } = false;
        public List<KurbatovParameter> parameters = new List<KurbatovParameter>();
    }
}