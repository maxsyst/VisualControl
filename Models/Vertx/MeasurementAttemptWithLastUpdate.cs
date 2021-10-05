using VueExample.ViewModels.Vertx.ResponseModels;

namespace VueExample.Models.Vertx
{
    public class LastMeasurementMdv
    {
        public MeasurementAttemptResponseModel MeasurementAttempt { get; set; }
        public Measurement Measurement { get; set; }
        public Mdv Mdv { get; set; }

        public LastMeasurementMdv(MeasurementAttemptResponseModel measurementAttempt, Measurement measurement)
        {
            Measurement = measurement;
            MeasurementAttempt = measurementAttempt;
        }
    }
}
