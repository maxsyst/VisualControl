using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VueExample.Models.SRV6;

namespace VueExample.Entities
{
    [Table("KurbatovPattern")]
    public class StandartPatternEntity
    {
        [Key]
        [Column("id_kp")]
        public int Id { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        [Column("id_dt")]
        public int DieTypeId { get; set; }
        [NotMapped]
        public bool IsNullObject { get; set; } = false;

        public virtual DieType DieType { get; set; }
        public virtual ICollection<StandartMeasurementPatternEntity> StandartMeasurementPatterns { get; set; }
    }
}