using System.Collections.Generic;
using VueExample.Parsing.Models;

public class DieWithCode
{
    public long DieId { get; set; }
    public string DieCode { get; set; }
    public Dictionary<string, Dictionary<string, SingleLine>> stateDictionary { get; set; } = new Dictionary<string, Dictionary<string, SingleLine>>();
}