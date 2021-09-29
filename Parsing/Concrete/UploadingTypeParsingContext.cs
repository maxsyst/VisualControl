using System.Collections.Generic;
using VueExample.Parsing.Models;
using VueExample.Parsing.StrategyInterface;
using VueExample.Parsing.UploadingType;

namespace VueExample.Parsing.Concrete
{
    public class UploadingTypeParsingContext
    {
        private readonly IUploadingTypeParsingStrategy _uploadingTypeParsingStrategy;
        public UploadingTypeParsingContext(string uploadingType, string s2pType)
        {
            if(uploadingType == "ATT")
            {
                _uploadingTypeParsingStrategy = new AttParseStrategy(s2pType);
            }
            if(uploadingType == "PSW")
            {
                _uploadingTypeParsingStrategy = new PswParseStrategy();
            }
        }

        public Dictionary<string, SingleLine> Parse(string path) 
        {
            return _uploadingTypeParsingStrategy.Parse(path);    
        }
        public Dictionary<string, Dictionary<string, SingleLine>> DeltaCalculation(Dictionary<string, Dictionary<string, SingleLine>> stateDictionary) 
        {
            return _uploadingTypeParsingStrategy.DeltaCalculation(stateDictionary);    
        }
    }
}