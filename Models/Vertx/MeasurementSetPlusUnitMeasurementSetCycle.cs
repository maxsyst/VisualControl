namespace VueExample.Models.Vertx
{
    public class MeasurementSetPlusUnitMeasurementSetCycle
    {
        public CycleStatus CycleStatus { get; set; } = CycleStatus.NotCreatedYet;

        public MeasurementSetPlusUnitMeasurementSetCycle()
        {

        }

        public MeasurementSetPlusUnitMeasurementSetCycle(CycleStatus cycleStatus)
        {
            CycleStatus = cycleStatus;
        }
    }

    public enum CycleStatus
    {
        NotCreatedYet = 0,
        Finished = -1,
        Live = 1
    }
}