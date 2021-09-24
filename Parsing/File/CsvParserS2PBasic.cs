using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VueExample.Parsing.Models;
using VueExample.Parsing.StrategyInterface;


namespace VueExample.Parsing.File
{
    public class CsvParserS2PBasic : ICsvParsingS2PStrategy
    {
        public SingleLine Parse(string path, string ordinateName)
        {
            var singleLine = new SingleLine();
            var parseList = System.IO.File.ReadAllLines(path).Skip(3).ToList();
            foreach (var line in parseList)
            {
                var splitarray = line.Split(' ').ToList();
                singleLine.AbscissList.Add(Math.Round((double.Parse(splitarray[0], CultureInfo.InvariantCulture) / 1E9), 2).ToString(CultureInfo.InvariantCulture));
                singleLine.ValueList.Add(Math.Round((double.Parse(splitarray[2], CultureInfo.InvariantCulture)), 8).ToString(CultureInfo.InvariantCulture));

            }
            return singleLine;
        }
    }
}