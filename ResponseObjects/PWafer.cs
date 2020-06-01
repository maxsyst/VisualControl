using System;

namespace VueExample.ResponseObjects
{
    public class PWafer
    {
        public string WaferId { get; set; }
        public string CodeProductName { get; set; }
        public string ProcessName { get; set; }
        public DateTime MeasurementDate { get; set; }
        public DateTime StartTime { get; set; }
    }
}