using System.Collections.Generic;

namespace VueExample.ChartModels.AmChart
{
    public abstract class AmChart<T> where T : class 
    {
        public abstract List<T> Data { get; set; }
        public string Title { get; set; }
    }
}
