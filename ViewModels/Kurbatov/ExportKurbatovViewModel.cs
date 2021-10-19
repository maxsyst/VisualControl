using System.Collections.Generic;
using VueExample.Models.SRV6.Export;

namespace VueExample.ViewModels.Kurbatov
{
    public class ExportKurbatovViewModel
    {
        public string Name { get; set; }
        public List<AtomicDieValue> dieValueList = new List<AtomicDieValue>();
    }
}