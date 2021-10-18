using VueExample.Models.SRV6;

namespace VueExample.ViewModels
{
    public class GraphicShortLinkViewModel
    {
        public int GraphicId { get; set; }
        public string KeyGraphicState{ get; set; }
        public string Mode { get; set; }
        public bool IsLog { get; set; }

        public GraphicShortLinkViewModel()
        {
        }

        public GraphicShortLinkViewModel(Graphic graphic, string mode = "initial", bool isLog = false)
        {
            GraphicId = graphic.Id;
            if(graphic.Name.Contains("Count"))
            {
                KeyGraphicState = graphic.Id + "_HSTG";
            }
            else
            {
                KeyGraphicState = graphic.Id + "_LNR";
            }
            Mode = mode;
            IsLog = isLog;
        }
    }
}