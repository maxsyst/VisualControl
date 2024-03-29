using System.Collections.Generic;

namespace VueExample.StatisticsCoreRework.Models
{
    public class MeasurementRecordingDirtyCellsSnapshot
    {
        public int MeasurementRecordingId { get; set; }
        public List<long> BadDies { get; set; } = new List<long>();
        public List<long> SelectedDies { get; set; } = new List<long>();
        public Dictionary<string, List<DirtyCellsProfile>> DirtyCellsProfiles { get; set; } = new Dictionary<string, List<DirtyCellsProfile>>();
        public string GoodDiesPercentage { get; set; } = "0.0";
        public Dictionary<string, SingleGraphicDirtyCells> SingleGraphicDirtyCellsDictionary { get; set; } = new Dictionary<string, SingleGraphicDirtyCells>();
    }
}