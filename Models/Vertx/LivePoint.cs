using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VueExample.Models.Vertx
{
    [Table("LivePoints")]
    public class LivePoint
    {
        [Column("id")]
        public Int64 Id { get; set; }
        [Column("value")]
        public string Value { get; set; }
        [Column("characteristicName")]
        public string CharacteristicName { get; set; }
        [Column("date")]
        public DateTime Date { get; set; }
        [Column("measurementName")]
        public string MeasurementName { get; set; }

    }
}