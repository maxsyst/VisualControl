using VueExample.Entities;
using VueExample.Models.SRV6;

namespace VueExample.Parsing.Strategies
{
    public interface IStringGraphicSRV6ParsingStrategy
    {
        DieValue ParseStringGraphic(DieGraphics dieGraphic);
    }
}
