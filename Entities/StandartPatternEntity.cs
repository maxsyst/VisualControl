using System.ComponentModel.DataAnnotations.Schema;

namespace VueExample.Entities
{
    [Table("KurbatovPattern")]
    public class StandartPatternEntity
    {
        [Column("id_kp")]
        public int Id { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        [Column("id_dt")]
        public int DieTypeId { get; set; }
    }
}