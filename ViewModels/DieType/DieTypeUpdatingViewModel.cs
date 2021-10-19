using System.Collections.Generic;

namespace VueExample.ViewModels.DieType
{
    public class DieTypeUpdatingViewModel
    {
        public string Name { get; set; }
        public List<int> CodeProductIdsList { get; set; }
        public List<ElementViewModel> ElementsList { get; set; }
    }
}