namespace VueExample.StatisticsCoreRework.Models
{
    public class DirtyCellsShortFxd : DirtyCellsShort
    {
        public DirtyCellsShortFxd(string statName, string lowBorder, string topBorder) : base(statName, lowBorder, topBorder) 
        {
            Type = "FXD";
            LowBorder = lowBorder;
            TopBorder = topBorder;
        }
    }
}