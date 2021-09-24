using VueExample.Parsing.StrategyInterface;
using VueExample.Parsing.UploadingType;

namespace VueExample.Parsing.Concrete
{
    public class UploadingTypeParsingContext
    {
        private IUploadingTypeParsingStrategy _uploadingTypeParsingStrategy;
        public UploadingTypeParsingContext(string uploadingType)
        {
            if(uploadingType == "ATT")
            {
                _uploadingTypeParsingStrategy = new AttParseStrategy();
            }
            if(uploadingType == "PSW")
            {
                _uploadingTypeParsingStrategy = new PswParseStrategy();
            }
        }

        public void Parse(string path) 
        {
            _uploadingTypeParsingStrategy.Parse(path);    
        }
    }
}