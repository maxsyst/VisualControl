using System;
using System.Globalization;
using System.Linq;
using VueExample.Parsing.Models;
using VueExample.Parsing.StrategyInterface;

namespace VueExample.Parsing.File
{
    public class CsvParserS2PSParameters : ICsvParsingS2PStrategy
    {
        public SingleLine Parse(string path, string ordinateName)
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
                var splitarray = line.Split('\t').ToList();
                singleLine.AbscissList.Add(Math.Round((double.Parse(splitarray[0], CultureInfo.InvariantCulture) / 1E9), 2).ToString(CultureInfo.InvariantCulture));
                singleLine.ValueList.Add(FromRItoDB(Math.Round((double.Parse(splitarray[index], CultureInfo.InvariantCulture)), 8), Math.Round((double.Parse(splitarray[index + 1], CultureInfo.InvariantCulture)), 8)).ToString(CultureInfo.InvariantCulture));
            }
            return singleLine;
        }
        private double FromRItoDB(double x, double y)
        {
            var result = Math.Round(20 * Math.Log10(Math.Sqrt(Math.Pow(x, 2.0) + Math.Pow(y, 2.0))), 2);
            return Double.IsInfinity(result) || Double.IsNaN(result) ? 0 : result;
        }
    }
}