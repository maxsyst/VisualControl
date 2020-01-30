using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VueExample.Models.SRV6
{
    [Table("Graphics")]
    public class Graphic
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("absciss")]
        public string Absciss { get; set; }
        [Column("ordinate")]
        public string Ordinate { get; set; }
        [Column("Comment")]
        public string Name { get; set; }
        [Column("abscissUnit")]
        public string AbscissUnit { get; set; }
        [Column("ordinateUnit")]
        public string OrdinateUnit { get; set; }
        [Column("StatisticsFunction")]
        public string StatisticsFunction { get; set; }
        [Column("GraphicType")]
        public Int16 Type { get; set; }

    }
}
