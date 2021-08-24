namespace VueExample.StatisticsCoreRework.Models
{
    public class DirtyCellsShortStat : DirtyCellsShort
    {
        public string K { get; set; } = "0.0";
        public DirtyCellsShortStat(string statName, string lowBorder, string topBorder, string k) : base(statName, lowBorder, topBorder)
        {
            Type = "STAT";
            StatName = statName;
            LowBorder = lowBorder;
            TopBorder = topBorder;
            K = k;
        }
    }
}