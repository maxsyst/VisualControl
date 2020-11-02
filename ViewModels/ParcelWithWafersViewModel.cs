using System.Collections.Generic;

namespace VueExample.ViewModels
{
    public class ParcelWithWafersViewModel
    {
        public int ParcelId { get; set; }
        public string ParcelName { get; set; }
        public List<WaferViewModel> ChildrenWafers { get; set; }
    }
}