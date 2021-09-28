using System.Collections.Generic;
using VueExample.Parsing.Models;

namespace VueExample.Parsing.StrategyInterface
{
    public interface IUploadingTypeParsingStrategy
    {
        Dictionary<string, SingleLine> Parse(string path);
        Dictionary<string, Dictionary<string, SingleLine>> DeltaCalculation(Dictionary<string, Dictionary<string, SingleLine>> stateDictionary);
    }
}