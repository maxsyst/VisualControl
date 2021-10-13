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
    public class CsvParserS2PSParameters : ICsvParsingS2PStrategy
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
                singleLine.ValueList.Add(CheckType(splitarray, index, S2PType));
            }
            return singleLine;
        }
        private string CheckType(List<string> splitArray, int index, string S2PType)
        {
            if(S2PType == "RI")
            {
                return FromRItoDB(CalculateValue(splitArray, index), CalculateValue(splitArray, index + 1)).ToGoodFormat();
            }
            return CalculateValue(splitArray, index).ToGoodFormat();
        }
        private double CalculateValue(List<string> splitArray, int index)
        {
            return Math.Round((double.Parse(splitArray[index], CultureInfo.InvariantCulture)), 8);
        }
        private double FromRItoDB(double x, double y)
        {
            var result = Math.Round(20 * Math.Log10(Math.Sqrt(Math.Pow(x, 2.0) + Math.Pow(y, 2.0))), 2);
            return Double.IsInfinity(result) || Double.IsNaN(result) ? 0 : result;
        }
    }
}