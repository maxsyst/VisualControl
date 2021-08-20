using System.Collections.Generic;

namespace VueExample.StatisticsCoreRework.Models
{
    public class MeasurementRecordingDirtyCellsSnapshot
    {
        public int MeasurementRecordingId { get; set; }
        public List<long> BadDies { get; set; } = new List<long>();
        public Dictionary<string, SingleGraphicDirtyCells> SingleGraphicDirtyCellsDictionary = new Dictionary<string, SingleGraphicDirtyCells>();
    }
}