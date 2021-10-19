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
    public class CsvParserS2PBasic : ICsvParsingS2PStrategy
    {
        public SingleLine Parse(string path, string ordinateName, string S2PType)
        {
            var singleLine = new SingleLine();
            var parseList = System.IO.File.ReadAllLines(path).Skip(3).ToList();
            foreach (var line in parseList)
            {
                var nospaceline = Regex.Replace(line, " {2,}", @"\t");
                var splitarray = nospaceline.Split(@"\t").ToList();
                singleLine.AbscissList.Add(Math.Round((double.Parse(splitarray[0], CultureInfo.InvariantCulture) / 1E9), 2).ToGoodFormat());
                singleLine.ValueList.Add(Math.Round((double.Parse(splitarray[2], CultureInfo.InvariantCulture)), 8).ToGoodFormat());
            }
            return singleLine;
        }
    }
}