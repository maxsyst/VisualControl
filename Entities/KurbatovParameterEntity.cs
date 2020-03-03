using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VueExample.Entities
{
    [Table("KurbatovParameter")]
    public class KurbatovParameterEntity
    {
        [Key]
        [Column("id_kp")]
        public int Id { get; set; }
        [Column("id_sp")]
        public int StandartParameterId { get; set; }
        [Column("id_b")]
        public int? BordersId { get; set; }
        [Column("id_smp")]
        public int SmpId { get; set; }

        public StandartMeasurementPatternEntity StandartMeasurementPatternEntity { get; set; }
        public StandartParameterEntity StandartParameterEntity { get; set; }
        public KurbatovParameterBordersEntity KurbatovParameterBordersEntity { get; set; }


    }
}