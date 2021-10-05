using System;
using VueExample.Models.Vertx;

public class LivePointResponseModel
{
        public Int64 Id { get; set; }
        public string Value { get; set; }
        public string CharacteristicUnit { get; set; }
        public string CharacteristicName { get; set; }
        public DateTime Date { get; set; }
        public string MeasurementName { get; set; }

        public LivePointResponseModel()
        {
            
        }

        public LivePointResponseModel(LivePoint livePoint, string unit)
        {
            this.Id = livePoint.Id;
            this.Value = livePoint.Value;
            this.CharacteristicName = livePoint.CharacteristicName;
            this.CharacteristicUnit = unit;
            this.Date = livePoint.Date;
            this.MeasurementName = livePoint.MeasurementName;
        }
      
}