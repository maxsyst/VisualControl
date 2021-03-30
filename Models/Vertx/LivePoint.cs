using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VueExample.Models.Vertx
{
    [Table("Points")]
    public class LivePoint
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("value")]
        public string Value { get; set; }
        [Column("characteriticName")]
        public string CharacteristicName { get; set; }
        [Column("date")]
        public DateTime Date { get; set; }
        [Column("measurementName")]
        public string MeasurementName { get; set; }

    }
}