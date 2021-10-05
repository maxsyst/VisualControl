using System.Collections.Generic;
using VueExample.Parsing.Models;
using VueExample.Parsing.StrategyInterface;

namespace VueExample.Parsing.UploadingType
{
    public class PswParseStrategy : IUploadingTypeParsingStrategy
    {
        public PswParseStrategy()
        {
            
        }

        public Dictionary<string, Dictionary<string, SingleLine>> DeltaCalculation(Dictionary<string, Dictionary<string, SingleLine>> stateDictionary)
        {
            throw new System.NotImplementedException();
        }

        public Dictionary<string, SingleLine> Parse(string path)
        {
            throw new System.NotImplementedException();
        }
    }
}