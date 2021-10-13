using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using VueExample.Extensions;
using VueExample.Parsing.Models;
using VueExample.Parsing.StrategyInterface;
namespace VueExample.Parsing.File
{
    public class CsvParserS2PPhase : ICsvParsingS2PStrategy
    {
        public SingleLine Parse(string path, string ordinateName, string S2PType)
        {
            var singleLine = new SingleLine();
            var index = 3;
            if (ordinateName.Contains("S11"))
            {
                index = 1;
            }
            if (ordinateName.Contains("S21"))
            {
                index = 3;
            }
            if (ordinateName.Contains("S12"))
            {
                index = 5;
            }
            if (ordinateName.Contains("S22"))
            {
                index = 7;
            }

            var parseList = System.IO.File.ReadAllLines(path).Skip(9).ToList();
            foreach (var line in parseList)
            {
                var nospaceline = Regex.Replace(line, " {1,}", @"\t");
                var splitarray = nospaceline.Split(@"\t").ToList();
                singleLine.AbscissList.Add(Math.Round((double.Parse(splitarray[0], CultureInfo.InvariantCulture) / 1E9), 2).ToGoodFormat());
                singleLine.ValueList.Add((Math.Atan2(Math.Round((double.Parse(splitarray[index + 1], CultureInfo.InvariantCulture)*180/Math.PI), 8),Math.Round((double.Parse(splitarray[index], CultureInfo.InvariantCulture) * 180 / Math.PI), 8))*180/Math.PI).ToGoodFormat());
            }
            return singleLine;
        }
    }
}