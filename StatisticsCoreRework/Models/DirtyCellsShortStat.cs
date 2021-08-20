namespace VueExample.StatisticsCoreRework.Models
{
    public class DirtyCellsShortStat : DirtyCellsShort
    {
        public string K { get; set; } = "0.0";
        public DirtyCellsShortStat(string statName, string k) : base(statName)
        {
            Type = "STAT";
            StatName = statName;
            K = k;
        }
    }
}