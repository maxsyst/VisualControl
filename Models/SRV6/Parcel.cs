using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace VueExample.Models.SRV6
{
    [Table("Parcel")]
    public class Parcel
    {
        [Column("id_parcel")]
        public int Id { get; set; }
        [Column("ParcelName")]
        public string Name { get; set; }
        [NotMapped]
        public ICollection<Wafer> Wafers { get; set; }  
    }
}