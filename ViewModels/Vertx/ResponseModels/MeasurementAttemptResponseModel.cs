using System;
using System.Collections.Generic;
using VueExample.Models.Vertx;

namespace VueExample.ViewModels.Vertx.ResponseModels
{
    public class MeasurementAttemptResponseModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime CreationDate { get; set; }
        public string MdvId { get; set; }

        public string RootMeasurementId { get; set; }

        public IList<string> MeasurementsId { get; set; } = new List<string>();

        public MeasurementResult MeasurementResult { get; set; } = new MeasurementResult();
    }
}