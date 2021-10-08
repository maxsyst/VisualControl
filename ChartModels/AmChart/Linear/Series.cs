using System.Collections.Generic;

namespace VueExample.ChartModels.AmChart.Linear
{
    public class Series
    {
        public List<Point> pointList { get; set; } = new List<Point>();
        public string Color { get; set; }
    }
}