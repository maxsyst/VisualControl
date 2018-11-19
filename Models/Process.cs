using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VueExample.Models
{
    [Table("ProcessList")]
    public class Process
    {
        [Column("id_processlist")]
        public int ProcessId { get; set; }
        [Column("Process_Name")]
        public string ProcessName { get; set; }
    }
}
