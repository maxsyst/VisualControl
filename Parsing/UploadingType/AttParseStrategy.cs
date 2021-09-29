using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using VueExample.Parsing.Concrete;
using VueExample.Parsing.Models;
using VueExample.Parsing.StrategyInterface;
using System;

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
            var A0State = stateDictionary["A0"];
            var S21A0State = A0State["S21"];
            var phaseA0State = A0State["Phase"];
            foreach (var state in stateDictionary.Keys.Where(x => x != "A0"))
            {
                var deltaS21OrdinateList = new List<string>();
                var deltaPhaseOrdinateList = new List<string>();
                deltaS21OrdinateList.AddRange(S21A0State.ValueList);
                deltaPhaseOrdinateList.AddRange(phaseA0State.ValueList);
                for (var i = 0; i < S21A0State.ValueList.Count; i++)
                {
                    deltaS21OrdinateList[i] = (Convert.ToDouble(deltaS21OrdinateList[i], CultureInfo.InvariantCulture) - Convert.ToDouble(S21A0State.ValueList[i], CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture);
                    deltaPhaseOrdinateList[i] = (Convert.ToDouble(deltaPhaseOrdinateList[i], CultureInfo.InvariantCulture) - Convert.ToDouble(phaseA0State.ValueList[i], CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture);
                    if (Math.Abs(Convert.ToDouble(deltaPhaseOrdinateList[i], CultureInfo.InvariantCulture)) > 240)
                    {
                        deltaPhaseOrdinateList[i] = (Math.Abs(Convert.ToDouble(deltaPhaseOrdinateList[i], CultureInfo.InvariantCulture) / Math.Abs(Convert.ToDouble(deltaPhaseOrdinateList[i], CultureInfo.InvariantCulture)) * (Math.Abs(Convert.ToDouble(deltaPhaseOrdinateList[i], CultureInfo.InvariantCulture)) - 360))).ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        deltaPhaseOrdinateList[i] = (Math.Abs(Convert.ToDouble(deltaPhaseOrdinateList[i], CultureInfo.InvariantCulture))).ToString(CultureInfo.InvariantCulture);
                    }
                }
                stateDictionary[state].Add("S21Delta", new SingleLine{AbscissList = S21A0State.AbscissList.ToList(), ValueList = deltaS21OrdinateList});
                stateDictionary[state].Add("PhaseDelta", new SingleLine{AbscissList = S21A0State.AbscissList.ToList(), ValueList = deltaS21OrdinateList});
            }
            return stateDictionary;
        }

        public Dictionary<string, SingleLine> Parse(string path)
        {
            var s21SingleLine = _csvParsingSParameters.Parse(path, "S21", _s2pType);
            var phaseSingleLine = _csvParsingPhase.Parse(path, "S21", _s2pType);
            return new Dictionary<string, SingleLine>{{"S21", s21SingleLine}, {"Phase", phaseSingleLine}};
        }
        
    }
}