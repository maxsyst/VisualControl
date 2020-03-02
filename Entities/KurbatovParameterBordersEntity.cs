using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VueExample.Entities
{
    [Table("Borders")]
    public class KurbatovParameterBordersEntity
    {
        [Key]
        [Column("id_b")]
        public int Id { get; set; }
        [Column("LowerF")]
        public string Lower { get; set; }
        [Column("UpperF")]
        public string Upper { get; set; }
        public ICollection<KurbatovParameterEntity> KurbatovParameters { get; set; }
    }
}