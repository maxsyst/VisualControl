namespace VueExample.Parsing.Models
{
    public class SingleLineWithState : SingleLine
    {
        public string State { get; set; }
        public SingleLineWithState(SingleLine singleLine, string state) : base(singleLine)
        {
            State = state;
        }
    }
}