using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace VueExample.ViewModels
{
    public class GraphicS2PViewModel
    {
        [BindingBehavior(BindingBehavior.Required)]
        public int CodeProductId { get; set; }
        [BindingBehavior(BindingBehavior.Required)]
        public string GraphicS2PType { get; set; }
    }
}