using VueExample.Parsing.File;
using VueExample.Parsing.Models;
using VueExample.Parsing.StrategyInterface;

namespace VueExample.Parsing.Concrete
{
    public class CsvParserContext
    {
        private readonly ICsvParsingS2PStrategy _csvParsingStrategy;
        public CsvParserContext(string graphicType)
        {
            if(graphicType == "S21")
            {
                _csvParsingStrategy = new CsvParserS2PSParameters();
            }
             if(graphicType == "Phase")
            {
                _csvParsingStrategy = new CsvParserS2PPhase();
            }
        }
        SingleLine Parse(string path, string ordinateName) 
        {
            return _csvParsingStrategy.Parse(path, ordinateName);
        }
    }
}