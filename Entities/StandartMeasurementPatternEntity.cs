using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VueExample.Models;
using VueExample.Models.SRV6;

namespace VueExample.Entities
{
    [Table("StandartMeasurementPattern")]
    public class StandartMeasurementPatternEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_smp")]
        public int Id { get; set; }
        [Column("id_et")]
        public int ElementId { get; set; }
        [Column("id_stage")]
        public int StageId { get; set; }
        [Column("id_divider")]
        public int DividerId { get; set; }
        [Column("id_pattern")]
        public int PatternId { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        [Column("mslName")]
        public string MslName { get; set; }
        
        public virtual StandartPatternEntity StandartPattern { get; set; }
        public virtual Divider Divider { get; set; }
        public virtual Stage Stage { get; set; }
        public virtual Element Element { get; set; }
        public virtual ICollection<KurbatovParameterEntity> KurbatovParameters { get; set; }
    }
}