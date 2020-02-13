using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VueExample.Entities
{
    [Table("StandartParameter")]
    public class StandartParameterEntity
    {
        [Key]
        [Column("id_sp")]
        public int Id { get; set; }
        [Column("ParameterName")]
        public string ParameterName { get; set; }
        [Column("RussianParameterName")]
        public string RussianParameterName { get; set; }
        [Column("ParameterNameStat")]
        public string ParameterNameStat { get; set; }
        [Column("SpecialRon")]
        public bool SpecialRon { get; set; }
        [Column("DividerNeed")]
        public bool DividerNeed { get; set; }
    }
}