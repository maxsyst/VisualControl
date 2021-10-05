using VueExample.Parsing.Models;

namespace VueExample.Parsing.StrategyInterface
{
    public interface ICsvParsingS2PStrategy
    {
        SingleLine Parse(string path, string ordinateName, string S2PType);
    }
}