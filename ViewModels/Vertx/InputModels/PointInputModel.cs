using System;
using System.Collections.Generic;
using VueExample.Models.Vertx;

namespace VueExample.ViewModels.Vertx.InputModels
{
    public class PointInputModel
    {
        public string MeasurementName { get; set; }
        public Characteristic Characteristic { get; set; }
        public string Value { get; set; }
        public bool IsNewSet { get; set; }
        public DateTime? CreationDate { get; set; }
    }

    public class PointBatchInputModel
    {
        public string MeasurementName { get; set; }
        public List<CharacteristicWithValueInputModel> CharacteristicWithValues { get; set; } = new List<CharacteristicWithValueInputModel>();
        public bool IsNewSet { get; set; }
        public DateTime? CreationDate { get; set; }

    }
}
