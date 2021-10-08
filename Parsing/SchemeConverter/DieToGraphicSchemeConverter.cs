using System.Collections.Generic;
using System.Linq;
using VueExample.Models.SRV6.Uploader.Models;
using VueExample.Parsing.Models;

namespace VueExample.Parsing.SchemeConverter
{
    public class DieToGraphicSchemeConverter
    {
        public Graphic4ParseResult ConvertToScheme(List<Dictionary<string, DieWithCode>> dieWithCodeDictionaryList, Graphic4Type graphic)
        {
            var graphic4ParseResult = new Graphic4ParseResult();
            graphic4ParseResult.Graphic = graphic;
            graphic4ParseResult.DieWithCodesList.AddRange(from dieWithCodeDictionary in dieWithCodeDictionaryList
                                                          select dieWithCodeDictionary[graphic.GraphicMode]);
            if(graphic4ParseResult.DieWithCodesList.Count > 0)
            {
                graphic4ParseResult.States = graphic4ParseResult.DieWithCodesList.FirstOrDefault().SingleLineWithStateList.Select(x => x.State).ToList();
            }
            return graphic4ParseResult;
        }
        public Dictionary<string, DieWithCode> ConvertDieWithCode(DieWithCode dieWithCode, Dictionary<string, Dictionary<string, SingleLine>> stateDictionary) 
        {
            var lastKey = stateDictionary.Keys.Last();
            var graphicNames = stateDictionary[lastKey].Keys.ToList();
            var graphicsDictionary = new Dictionary<string, DieWithCode>();
            foreach (var graphicName in graphicNames)
            {
                var dieWithCodeGraphic = new DieWithCode();
                dieWithCodeGraphic.DieCode = dieWithCode.DieCode;
                dieWithCodeGraphic.DieId = dieWithCode.DieId;
                foreach (var state in stateDictionary.Keys)
                {
                    SingleLine singleLine;
                    var isGraphicExists = stateDictionary[state].TryGetValue(graphicName, out singleLine);
                    if(isGraphicExists)
                    {
                        var singleLineWithState = new SingleLineWithState(singleLine, state);
                        dieWithCodeGraphic.SingleLineWithStateList.Add(singleLineWithState);
                    }
                }
                graphicsDictionary.Add(graphicName, dieWithCodeGraphic);                
            }
            return graphicsDictionary;
        }
    }
}