namespace VueExample.StatisticsCoreRework.Models
{
    public class DirtyCellsShortFxd : DirtyCellsShort
    {
        public string LowBorder { get; set; }
        public string TopBorder { get; set; }
        public DirtyCellsShortFxd(string statName, string lowBorder, string topBorder) : base(statName) 
        {
            Type = "FXD";
            LowBorder = lowBorder;
            TopBorder = topBorder;
        }
    }
}