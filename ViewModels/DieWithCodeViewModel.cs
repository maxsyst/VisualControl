using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using VueExample.Parsing.Models;

namespace VueExample.ViewModels
{
    public class DieWithCodeViewModel
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("dieId")]
        public long DieId { get; set; }
        [BsonElement("dieCode")]
        public string DieCode { get; set; }
        [BsonElement("abscissList")]
        public List<string> AbscissList { get; set; } = new List<string>();
        [BsonElement("valueListWithState")]
        public List<ValueListWithState> ValueListWithState { get; set; } = new List<ValueListWithState>();

        public DieWithCodeViewModel()
        {
        }
        public DieWithCodeViewModel(DieWithCode dieWithCode)
        {
            DieId = dieWithCode.DieId;
            DieCode = dieWithCode.DieCode;
            AbscissList = dieWithCode.AbscissList.ToList();
            ValueListWithState = dieWithCode.ValueListWithState.ToList();
        }
    }
}