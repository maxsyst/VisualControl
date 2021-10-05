namespace VueExample.Models.Vertx
{
    public class MeasurementResult
    {
        public FinishResult FinishResult { get; set; } = FinishResult.Undefined;
        public string Description { get; set; }

        public MeasurementResult SetSuccess(string description)
        {
            FinishResult = FinishResult.Success;
            Description = description;
            return this;
        }

        public MeasurementResult SetFail(string description)
        {
            FinishResult = FinishResult.Fail;
            Description = description;
            return this;
        }
    }

    public enum FinishResult
    {
        Success = 1,
        Fail = 0,
        Undefined = -1
    }
}