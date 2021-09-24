using VueExample.Parsing.Concrete;
using VueExample.Parsing.StrategyInterface;

namespace VueExample.Parsing.UploadingType
{
    public class AttParseStrategy : IUploadingTypeParsingStrategy
    {
        public void Parse(string path)
        {
            var parseContext = new CsvParserContext("S21");
            
        }
    }
}