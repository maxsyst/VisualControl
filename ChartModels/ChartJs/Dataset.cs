using System.Collections.Generic;

namespace VueExample.ChartModels.ChartJs
{
    public class Dataset
    {
        public List<double> Data { get; set; }
        public bool Fill { get; set; } = false;       
        public int BorderWidth { get; set; } = 1;
        public int PointHoverRadius { get; set; } = 0;
        public int PointRadius { get; set; } = 0;

        public Dataset()
        {
            this.Data = new List<double>();
        }

         
    }
}