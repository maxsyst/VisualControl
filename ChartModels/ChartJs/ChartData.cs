using System.Collections.Generic;
namespace VueExample.ChartModels.ChartJs
{
    public class ChartData
    {
        public List<string> Labels { get; set; }
        public Dictionary<long, Dataset> Datasets { get; set; }

        public ChartData(List<string> labels, Dictionary<long, Dataset> datasets)
        {
            Labels = labels;
            Datasets = datasets;
        }
    }
}