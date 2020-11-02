using System.ComponentModel.DataAnnotations.Schema;
using VueExample.Models.SRV6;

namespace VueExample.Models
{
    [Table("GrowWafer")]
    public class Wafer
    {
        [Column("Wafer_id")]
        public string WaferId { get; set; }
        [Column("id_cp")]
        public int? CodeProductId { get; set; }
        [Column("id_parcel")]
        public int? ParcelId { get; set; }
        public Parcel Parcel { get; set; }
        public CodeProduct CodeProduct { get; set; }
    }
}
