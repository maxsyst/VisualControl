using System.Collections.Generic;
using System.Linq;
using VueExample.Parsing.Models;

public class DieWithCode
{
    public long DieId { get; set; }
    public string DieCode { get; set; }
    public List<string> AbscissList { get; set; } = new List<string>();
    public List<ValueListWithState> ValueListWithState { get; set; } = new List<ValueListWithState>();
    public DieWithCode()
    {
    }

    public DieWithCode(long id, string code)
    {
        this.DieId = id;
        this.DieCode = code;
    }
    public DieWithCode SetValues(SingleLineWithState singleLineWithState) 
    {
        if(AbscissList.Count == 0)
        {
            AbscissList = singleLineWithState.AbscissList.ToList();
        }
        ValueListWithState.Add(new ValueListWithState{State = singleLineWithState.State, ValueList = singleLineWithState.ValueList.ToList()});
        return this;
    }
}