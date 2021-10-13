using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using VueExample.Models.SRV6.Uploader.Models;
using VueExample.Parsing.Models;

namespace VueExample.ViewModels
{
    public class Graphic4ParseResultViewModel
    {
        public Graphic4Type Graphic { get; set; }
        public List<string> States { get; set; } = new List<string>();
        public List<ObjectId> DieWithCodesList { get; set; } = new List<ObjectId>();
        public Graphic4ParseResultViewModel()
        {
        }
        public Graphic4ParseResultViewModel(Graphic4ParseResult graphic4ParseResult, List<ObjectId> dieWithCodesList)
        {
            Graphic = graphic4ParseResult.Graphic;
            States = graphic4ParseResult.States.ToList();
            DieWithCodesList = dieWithCodesList.ToList(); 
        }
    }
}