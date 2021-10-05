using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using VueExample.Models.SRV6;

namespace VueExample.Models
{
    [Table("Stage")]
    public class Stage
    {
        [Key]
        [Column("id_stage")]
        public int StageId { get; set; }
        [Column("StageName")]
        public string StageName { get; set; }
        [Column("id_process")]
        public int ProcessId { get; set; }
        [Column("id_cp")]
        public int? CodeProductId { get; set; }
        [JsonIgnore]
        public IList<StatParameterForStage> StatParameterForStages { get; set; }
        [NotMapped]
        public bool IsNullObject { get; set; } = false;
    }
}
