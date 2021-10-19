using System.Collections.Generic;

namespace VueExample.ViewModels.Kurbatov
{
    public class KurbatovXLSBodyModel
    {
        public string WaferId { get; set; }
        public string MSLNumber { get; set; }
        public string Date { get; set; }
        public List<KurbatovXLSViewModel> kurbatovXLSViewModelList = new List<KurbatovXLSViewModel>();
    }
}