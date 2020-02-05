using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VueExample.Models.SRV6
{
    [Table("SpecificElementType")]
    public class SpecificElementType
    {
        [Key]        
        [Column("id_set")]
        public int Id { get; set; }         
        [Column("id_et")]
        public int ElementType { get; set; }         
        [Column("Name")]
        public string Name { get; set; }
        [Column("Specification")]
        public string Specification { get; set; }
    }
}