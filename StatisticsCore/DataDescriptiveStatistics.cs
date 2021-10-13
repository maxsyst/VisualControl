using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MathNet.Numerics.Statistics;


namespace VueExample.StatisticsCore
{
    public class DataDescriptiveStatistics
    {
        public string Median { get; set; }
        public string Quartile1 { get; set; }
        public string Quartile3 { get; set; }
        public string Maximum { get; set; }
        public string Minimum { get; set; }
        public double Quartile1Double { get; set; }
        public double Quartile3Double { get; set; }
        public double MaximumDouble { get; set; }
        public double MinimumDouble { get; set; }
        public double IQRDouble { get; set; }
        public string WaferID { get; set; }

        public DataDescriptiveStatistics()
        {
        }

        public DataDescriptiveStatistics(List<double> list)
        {
            Median = Convert.ToString(list.Median(), CultureInfo.InvariantCulture);
            Quartile1 = Convert.ToString(list.LowerQuartile(), CultureInfo.InvariantCulture);
            Quartile3 = Convert.ToString(list.UpperQuartile(), CultureInfo.InvariantCulture);
            Quartile1Double = list.LowerQuartile();
            Quartile3Double = list.UpperQuartile();
            IQRDouble = IQR(list);
            Maximum = Convert.ToString(list.Maximum(), CultureInfo.InvariantCulture);
            Minimum = Convert.ToString(list.Minimum(), CultureInfo.InvariantCulture);
            MaximumDouble = list.Maximum();
            MinimumDouble = list.Minimum();
        }

        public Histogram GetHistogramFromList(List<double> list, int stepQuantity)
        {
            list.RemoveAll(Double.IsNaN);
            var histogram = new Histogram(list, stepQuantity);
            return histogram;
        }

        private double IQR(IEnumerable<double> list)
        {
            return list.InterquartileRange();
        }

        private double Mean(IEnumerable<double> list)
        {
            return list.Median();
        }

        public List<double> Filtered(List<double> list)
        {
            list.RemoveAll(Double.IsNaN);
            var lowlimit = (list.LowerQuartile() - (1.5 * IQR(list)));
            var upperlimit = (list.UpperQuartile() + (1.5 * IQR(list)));
            var filteredList = list.Where(d => d > lowlimit && d < upperlimit).ToList();
            return filteredList;
        }
    }
}