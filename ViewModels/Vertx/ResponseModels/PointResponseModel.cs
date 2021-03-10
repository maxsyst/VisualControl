using System;

namespace VueExample.ViewModels.Vertx.ResponseModels
{
    public class PointResponseModel
    {
        public string GeneratedId { get; set; }
        public double Value { get; set; }
        public DateTime TrueDate { get; set; }
        public TimeSpan FromStartDate { get; set; }
    }
}