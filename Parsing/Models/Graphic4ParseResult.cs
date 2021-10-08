using System.Collections.Generic;
using VueExample.Models.SRV6.Uploader.Models;

namespace VueExample.Parsing.Models
{
    public class Graphic4ParseResult
    {
        public Graphic4Type Graphic { get; set; }
        public List<string> States { get; set; } = new List<string>();
        public List<DieWithCode> DieWithCodesList { get; set; } = new List<DieWithCode>();
    }
}