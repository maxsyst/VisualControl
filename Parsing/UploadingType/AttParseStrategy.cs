using System.Collections.Generic;
using VueExample.Parsing.Concrete;
using VueExample.Parsing.Models;
using VueExample.Parsing.StrategyInterface;

namespace VueExample.Parsing.UploadingType
{
    public class AttParseStrategy : IUploadingTypeParsingStrategy
    {
        private readonly CsvParserContext _csvParsingSParameters;
        private readonly CsvParserContext _csvParsingPhase;
        private readonly string _s2pType;
        public AttParseStrategy(string S2PType)
        {
            _csvParsingSParameters = new CsvParserContext("SParameters");
            _csvParsingPhase = new CsvParserContext("Phase");
            _s2pType = S2PType;
        }

        public Dictionary<string, Dictionary<string, SingleLine>> DeltaCalculation(Dictionary<string, Dictionary<string, SingleLine>> stateDictionary)
        {
            throw new System.NotImplementedException();
        }

        public Dictionary<string, SingleLine> Parse(string path)
        {
            var s21SingleLine = _csvParsingSParameters.Parse(path, "S21", _s2pType);
            var phaseSingleLine = _csvParsingPhase.Parse(path, "S21", _s2pType);
            return new Dictionary<string, SingleLine>{{"S21", s21SingleLine}, {"Phase", phaseSingleLine}};
        }
    }
}