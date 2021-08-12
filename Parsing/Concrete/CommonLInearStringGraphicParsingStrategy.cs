using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VueExample.Entities;
using VueExample.Extensions;
using VueExample.Models.SRV6;
using VueExample.Parsing.Strategies;

namespace VueExample.Parsing.Concrete
{
    public class CommonLinearStringGraphicParsingStrategy : IStringGraphicSRV6ParsingStrategy
    {
        public DieValue ParseStringGraphic(DieGraphics dieGraphic)
        {
            var dieValue = new DieValue();
            var stringGraphicSplit = dieGraphic.ValueString.Split('X').ToList();
            dieValue.XList.AddRange(stringGraphicSplit[0].Split('*').ToList());
            dieValue.YList.AddRange(stringGraphicSplit[1].Split('*').ToList());
            dieValue.XList.RemoveFirstIfStringEmpty();
            dieValue.YList.RemoveFirstIfStringEmpty();
            dieValue.State = "LNR";
            dieValue.GraphicId = dieGraphic.GraphicId;
            dieValue.DieId = dieGraphic.DieId;
            dieValue.MeasurementRecordingId = dieGraphic.MeasurementRecordingId;  
            return dieValue;  
        }
    }
}
