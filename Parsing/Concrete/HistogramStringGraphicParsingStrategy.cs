using System.Collections.Generic;
using VueExample.Entities;
using VueExample.Models.SRV6;
using VueExample.Parsing.Strategies;

namespace VueExample.Parsing.Concrete
{
    public class HistogramStringGraphicParsingStrategy : IStringGraphicSRV6ParsingStrategy
    {
        public DieValue ParseStringGraphic(DieGraphics dieGraphic)
        {
            var dieValue = new DieValue();
            dieValue.YList.Add(dieGraphic.ValueString.Split('X')[1]);
            dieValue.State = "HSTG";
            dieValue.GraphicId = dieGraphic.GraphicId;
            dieValue.DieId = dieGraphic.DieId;
            dieValue.MeasurementRecordingId = dieGraphic.MeasurementRecordingId;
            return  dieValue;
        }
    }
}
