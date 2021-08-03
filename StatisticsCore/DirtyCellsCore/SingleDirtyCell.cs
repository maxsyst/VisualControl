namespace VueExample.StatisticsCore.DirtyCellsCore
{
    public class SingleDirtyCell
    {
        public int DieId { get; set; }
        public Cause Cause { get; set; }
        public string Difference { get; set; }
        public string TrueValue { get; set; }

        public SingleDirtyCell(int dieId)
        {
            DieId = dieId;
        }
    }

    public enum Cause
    {
        Lower = -1,
        Bigger = 1,
        isNaN = 0
    }
}