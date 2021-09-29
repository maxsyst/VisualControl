using System.Collections.Generic;
using VueExample.Parsing.Models;

public class DieWithCode
{
    public long DieId { get; set; }
    public string DieCode { get; set; }
    public List<SingleLineWithState> SingleLineWithStateList { get; set; } = new List<SingleLineWithState>();
    public DieWithCode()
    {
        
    }

    public DieWithCode(long id, string code)
    {
        this.DieId = id;
        this.DieCode = code;
    }
}