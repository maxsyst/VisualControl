using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VueExample.Models
{
    [Table("MeasuredDevice")]
    public class MeasuredDevice
    {
        [Column("id_measureddevice")]
        public int MeasuredDeviceId { get; set; }
        [Column("id_cp")]
        public int CodeProductId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
    }
}
