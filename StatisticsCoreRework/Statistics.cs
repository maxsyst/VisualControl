using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using MathNet.Numerics;
using VueExample.Models.SRV6;

namespace VueExample.StatisticsCoreRework
{

    public class Statistics
    {
        public string ExpectedValue { get; set; }
        public string Maximum { get; set; }
        public string Minimum { get; set; }
        public string StandartDeviation { get; set; }
        public string StatisticsName { get; set; }
        public string Unit { get; set; }
        public string Median { get; set; }
        public int ParameterID { get; set; }
        public DividerProfile NeedDivider { get; set; }
        public List<double> FullList { get; set; }



        public Statistics()
        {

        }

        public List<Statistics> GetStatistics(List<string> xList, List<List<string>> commonYList, Graphic graphics, double divider)
        {
            var thisType = GetType();
            if (String.IsNullOrEmpty(graphics.StatisticsFunction))
                return new List<Statistics>();
            var theMethod = thisType.GetMethod(graphics.StatisticsFunction, BindingFlags.Instance | BindingFlags.NonPublic);
            if (theMethod == null)
            {
                return new List<Statistics>();
            }
            var st = theMethod.Invoke(this, new object[] { xList, commonYList, divider }) as List<Statistics>;
            return st;
        }


        public List<Statistics> GetStatisticsHistogram(List<string> valueList, Graphic graphics)
        {
            var thisType = GetType();
            if (String.IsNullOrEmpty(graphics.StatisticsFunction))
                return new List<Statistics>();
            var theMethod = thisType.GetMethod(graphics.StatisticsFunction, BindingFlags.Instance | BindingFlags.NonPublic);
            if (theMethod == null)
            {
                return new List<Statistics>();
            }
            var st = theMethod.Invoke(this, new object[] { valueList, graphics }) as List<Statistics>;
            return st;
        }

        public List<Statistics> GetStatistics(List<double> xList, List<List<double>> commonYList, Graphic graphics, double divider, string type)
        {
            var thisType = GetType();
            if (String.IsNullOrEmpty(graphics.StatisticsFunction))
                return new List<Statistics>();
            var theMethod = thisType.GetMethod(graphics.StatisticsFunction, BindingFlags.Instance | BindingFlags.NonPublic);
            if (theMethod == null)
            {
                return new List<Statistics>();
            }
            var st = theMethod.Invoke(this, new object[] { xList, commonYList, divider, type }) as List<Statistics>;
            return st;
        }

        public string GetStatistics(List<double> xList, List<List<double>> commonYList, string statisticsfunction, string type, Dictionary<string,double> points )
        {
            var thisType = GetType();
            if (String.IsNullOrEmpty(statisticsfunction))
                return String.Empty;
            var theMethod = thisType.GetMethod(statisticsfunction, BindingFlags.Instance | BindingFlags.NonPublic);
            if (theMethod == null)
            {
                return String.Empty;
            }
            var st = theMethod.Invoke(this, new object[] { xList, commonYList, type, points }) as String;
            return st;
        }


        public string GetStatistics(List<double> xList, Dictionary<string, List<List<double>>> commonYList, string statisticsfunction, Dictionary<string,string> type, Dictionary<string, double> points)
        {
            var thisType = GetType();
            if (String.IsNullOrEmpty(statisticsfunction))
                return String.Empty;
            var theMethod = thisType.GetMethod(statisticsfunction, BindingFlags.Instance | BindingFlags.NonPublic);
            if (theMethod == null)
            {
                return String.Empty;
            }
            var st = theMethod.Invoke(this, new object[] { xList, commonYList, type, points }) as String;
            return st;
        }




        public static Statistics GetStatisticsAverageFull(List<Statistics> list)
        {

            var diffstat = new Statistics
            {
                Minimum = GetAverageFromFullStatistics(list, "Minimum"),
                ExpectedValue = GetAverageFromFullStatistics(list, "ExpectedValue"),
                Maximum = GetAverageFromFullStatistics(list, "Maximum"),
                StandartDeviation = GetAverageFromFullStatistics(list, "StandartDeviation"),
                Median = GetAverageFromFullStatistics(list, "Median")
            };
            return diffstat;
        }

        public static Statistics GetStatisticsSDFull(List<Statistics> list)
        {

            var diffstat = new Statistics
            {
                Minimum = GetSDFromFullStatistics(list, "Minimum"),
                ExpectedValue = GetSDFromFullStatistics(list, "ExpectedValue"),
                Maximum = GetSDFromFullStatistics(list, "Maximum"),
                StandartDeviation = GetSDFromFullStatistics(list, "StandartDeviation"),
                Median = GetSDFromFullStatistics(list, "Median")
            };
            return diffstat;
        }

        public List<Statistics> GetStatistics(List<string> list, Graphic graphics, StatisticParameter statisticParameter = null)
        {
            var thisType = GetType();
            var theMethod = thisType.GetMethod("GetSingleStatistics", BindingFlags.Instance | BindingFlags.NonPublic);
            if (theMethod == null)
            {
                return new List<Statistics>();
            }
            var st = theMethod.Invoke(this, new object[] { list, graphics, statisticParameter }) as List<Statistics>;
            return st;
        }

        public static Statistics GetDifferenceBetweenTwoStatistics(Statistics main, Statistics second)
        {
            var diffstat = new Statistics
            {
                Minimum = GetDifferenceBetweenTwoStatisticsParams(main.Minimum, second.Minimum),
                ExpectedValue = GetDifferenceBetweenTwoStatisticsParams(main.ExpectedValue, second.ExpectedValue),
                Maximum = GetDifferenceBetweenTwoStatisticsParams(main.Maximum, second.Maximum),
                StandartDeviation = GetDifferenceBetweenTwoStatisticsParams(main.StandartDeviation, second.StandartDeviation),
                Median = GetDifferenceBetweenTwoStatisticsParams(main.Median, second.Median)
            };
            return diffstat;
        }

        private static string GetDifferenceBetweenTwoStatisticsParams(string main, string second)
        {
            var returnstring = String.Empty;
            var difference = double.Parse(main) - double.Parse(second);
            if (difference < 0)
            {
                returnstring = "<br><span class=\"differencespan\" style=\"display:none\">" + "<font color=\"#238F23\">" + ToStringD(Math.Abs(difference)) + "↑" + "</font>" + "</span></br>";
            }
            else
            {
                returnstring = "<br><span class=\"differencespan\" style=\"display:none\">" + "<font color=\"#8F2323\">" + ToStringD(Math.Abs(difference)) + "↓" + "</font>" + "</span></br>";
            }
            return returnstring;

        }

        private static string GetAverageFromFullStatistics(IEnumerable<Statistics> list, string propertyname)
        {

            return CalculateExpectedValue(list.Select(statisticse => double.Parse(Convert.ToString(statisticse.GetType().GetProperty(propertyname).GetValue(statisticse, null)))).ToList());


        }

        private static string GetSDFromFullStatistics(IEnumerable<Statistics> list, string propertyname)
        {

            return CalculateStandartDeviation(list.Select(statisticse => double.Parse(Convert.ToString(statisticse.GetType().GetProperty(propertyname).GetValue(statisticse, null)))).ToList());


        }


        private static string CalculateExpectedValue(IEnumerable<double> list)
        {
            var enumerable = list as IList<double> ?? list.ToList();
            if (enumerable.Count == 0)
            {
                return String.Empty;
            }
            var average = enumerable.Average();
            if ((Math.Abs(average) >= 10000 || Math.Abs(average) < 1E-2) && Math.Abs(average - 0) > 1E-20)
            {
                return average.ToString("0.00E0");
            }
            return average.ToString("0.000");
        }

        private static string ToStringD(double d)
        {
            if ((Math.Abs(d) >= 10000 || Math.Abs(d) < 1E-2) && Math.Abs(d - 0) > 1E-20)
            {
                return d.ToString("0.00E0");
            }
            return d.ToString("0.000");
        }

        private static string CalculateMedian(IEnumerable<double> list)
        {
            var enumerable = list as IList<double> ?? list.ToList();
            var median = MathNet.Numerics.Statistics.Statistics.Median(enumerable);
            if ((Math.Abs(median) >= 10000 || Math.Abs(median) < 1E-2) && Math.Abs(median - 0) > 1E-20)
            {
                return median.ToString("0.00E0");
            }
            return median.ToString("0.000");
        }

        private static string CalculateStandartDeviation(List<double> list)
        {

            var enumerable = list as IList<double> ?? list.ToList();
            if (enumerable.Count == 0)
            {
                return String.Empty;
            }
            var avg = list.Average();
            //var standartdeviation = Math.Sqrt(list.Average(v => Math.Pow(v - avg, 2)));
            var standartdeviation = MathNet.Numerics.Statistics.Statistics.StandardDeviation(list);
            if (Double.IsNaN(standartdeviation))
            {
                standartdeviation = 0.0;
            }
            if ((Math.Abs(standartdeviation) >= 10000 || Math.Abs(standartdeviation) < 1E-2) && Math.Abs(standartdeviation - 0) > 1E-20)
            {
                return standartdeviation.ToString("0.00E0");
            }
            return standartdeviation.ToString("0.000");


        }

        private static string CalculateMinimum(IEnumerable<double> list)
        {


            var enumerable = list as IList<double> ?? list.ToList();
            if (enumerable.Count == 0)
            {
                return String.Empty;
            }
            var minimum = enumerable.Min();
            if ((Math.Abs(minimum) >= 10000 || Math.Abs(minimum) < 1E-2) && Math.Abs(minimum - 0) > 1E-20)
            {
                return minimum.ToString("0.00E0");
            }
            return minimum.ToString("0.000");
        }

        private static string CalculateMaximum(IEnumerable<double> list)
        {
            var enumerable = list as IList<double> ?? list.ToList();
            if (enumerable.Count == 0)
            {
                return String.Empty;
            }
            var maximum = enumerable.Max();
            if ((Math.Abs(maximum) >= 10000 || Math.Abs(maximum) < 1E-2) && Math.Abs(maximum - 0) > 1E-20)
            {
                return maximum.ToString("0.00E0");
            }
            return maximum.ToString("0.000");
        }


        private Statistics GetFullStatisticsFromList(List<double> list, string statisticsname, string unit, int parameterId = 0, DividerProfile dividerProfile = DividerProfile.WithoutDivider)
        {
            var statisitics = new Statistics
                {
                    StatisticsName = statisticsname,
                    Unit = unit,
                    NeedDivider = dividerProfile,
                    FullList = list,
                };
            return statisitics;
        }

        private static List<double> DeNanList(IEnumerable<double> list)
        {
            return list.Where(d => !double.IsNaN(d)).ToList();
        }

        private List<Statistics> GetIdssAndUgsoff(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var zeroIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.0 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.5 - item)).FirstOrDefault());
            var idssList = new List<double>();
            var id05List = new List<double>();
            var ugsoffwithinterpolationList = new List<double>();
            var ugsoffwithinterpolation100List = new List<double>();
            var ugsoffList = new List<double>();
            var ugsoffminList = new List<double>();
            var unit = "A";
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                var isugsoff = false;
                idssList.Add(yListdouble[zeroIndex]);
                id05List.Add(yListdouble[fiveIndex]);
                ugsoffminList.Add(xListdouble[yListdouble.IndexOf(yListdouble.Min())]);
                for (int i = yListdouble.Count - 1; i > -1; i--)
                {

                    if (yListdouble[i] < yListdouble[zeroIndex] / 1000)
                    {
                        if (i < yListdouble.Count - 1)
                        {
                            var xtwopointList = new List<double> {xListdouble[i], xListdouble[i + 1]};
                            var ytwopointList = new List<double> {yListdouble[i], yListdouble[i + 1]};
                            var interpolationmethod = Interpolate.Linear(ytwopointList, xtwopointList);
                            ugsoffwithinterpolationList.Add(interpolationmethod.Interpolate(yListdouble[zeroIndex] / 1000));

                            ugsoffList.Add((xListdouble[i] + xListdouble[i + 1]) / 2);
                            isugsoff = true;
                        }
                        else
                        {
                            ugsoffwithinterpolationList.Add(xListdouble[i]);
                            ugsoffList.Add(xListdouble[i]);
                            isugsoff = true;
                        }

                        i = -1;
                    }

                }
                var isugsoff100 = false;
                for (int i = yListdouble.Count - 1; i > -1; i--)
                {
                    if (yListdouble[i] < yListdouble[zeroIndex] / 100)
                    {
                        if (i < yListdouble.Count - 1)
                        {
                            var xtwopointList = new List<double> { xListdouble[i], xListdouble[i + 1] };
                            var ytwopointList = new List<double> { yListdouble[i], yListdouble[i + 1] };
                            var interpolationmethod100 = Interpolate.Linear(ytwopointList, xtwopointList);
                            ugsoffwithinterpolation100List.Add(interpolationmethod100.Interpolate(yListdouble[zeroIndex] / 100));
                            isugsoff100 = true;


                        }
                        else
                        {
                            ugsoffwithinterpolation100List.Add(xListdouble[i]);
                            isugsoff100 = true;
                        }

                        i = -1;
                    }

                }
                if (!isugsoff)
                {
                    ugsoffList.Add(double.NaN);
                    ugsoffwithinterpolationList.Add(double.NaN);
                }
                if(!isugsoff100) {
                    ugsoffwithinterpolation100List.Add(double.NaN);
                }


            }
            if (ugsoffList.Count(x => Double.IsNaN(x)) == ugsoffList.Count())
            {
                foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
                {
                    ugsoffList.Clear();
                    var isugsoff = false;
                    for (int i = yListdouble.Count - 1; i > -1; i--)
                    {

                        if (yListdouble[i] < yListdouble[zeroIndex] / 100)
                        {
                            if (i < yListdouble.Count - 1)
                            {
                                ugsoffList.Add((xListdouble[i] + xListdouble[i + 1]) / 2);
                                var xtwopointList = new List<double> { xListdouble[i], xListdouble[i + 1] };
                                var ytwopointList = new List<double> { yListdouble[i], yListdouble[i + 1] };
                                var interpolationmethod = Interpolate.Linear(ytwopointList, xtwopointList);
                                ugsoffwithinterpolationList.Add(interpolationmethod.Interpolate(yListdouble[zeroIndex] / 100));
                                isugsoff = true;
                            }
                            else
                            {
                                ugsoffwithinterpolationList.Add(xListdouble[i]);
                                ugsoffList.Add(xListdouble[i]);
                                isugsoff = true;
                            }

                            i = -1;
                        }
                    }
                    if (!isugsoff)
                    {
                        ugsoffList.Add(double.NaN);
                        ugsoffwithinterpolationList.Add(1E9);
                    }



                }

                var returnexList = new List<Statistics>
                {
                    GetFullStatisticsFromList(idssList, "I<sub>DSS(3V)</sub> (начальный ток стока)", unit, 3, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(ugsoffwithinterpolationList, "U<sub>GS-100(off)</sub> (напряжение отсечки при Idss/100)", "В", 4),
                    GetFullStatisticsFromList(id05List, "Id<sub>max</sub> (ток при Vgs=0.5В)", "А", 0, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(ugsoffminList, "U<sub>GS(min)</sub> (напряжение отсечки при Imin)", "В"),

                };
                return returnexList;

            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(idssList, "I<sub>DSS(3V)</sub> (начальный ток стока)", unit, 3, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(ugsoffwithinterpolationList, "U<sub>GS(off)</sub> (напряжение отсечки при Idss/1000)", "В", 4),
                    GetFullStatisticsFromList(id05List, "Id<sub>max</sub> (ток при Vgs=0.5В)", "А", 0, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(ugsoffwithinterpolation100List, "U<sub>GS-100(off)</sub> (напряжение отсечки при Idss/100)", "В"),
                    GetFullStatisticsFromList(ugsoffminList, "U<sub>GS(min)</sub> (напряжение при Imin)", "В"),

                };
            return returnList;
        }

        private List<Statistics> GetIdssAndUgsoff_REVERSED(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var zeroIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.0 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.5 - item)).FirstOrDefault());
            var idssList = new List<double>();
            var id05List = new List<double>();
            var ugsoffwithinterpolationList = new List<double>();
            var ugsoffwithinterpolation100List = new List<double>();
            var ugsoffList = new List<double>();
            var ugsoffminList = new List<double>();
            var unit = "A";
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                var isugsoff = false;
                idssList.Add(yListdouble[zeroIndex]);
                id05List.Add(yListdouble[fiveIndex]);
                ugsoffminList.Add(xListdouble[yListdouble.IndexOf(yListdouble.Min())]);
                for (int i = 0; i < yListdouble.Count; i++)
                {

                    if (yListdouble[i] < yListdouble[zeroIndex] / 1000)
                    {
                        if (i < yListdouble.Count - 1)
                        {
                            var xtwopointList = new List<double> { xListdouble[i], xListdouble[i + 1] };
                            var ytwopointList = new List<double> { yListdouble[i], yListdouble[i + 1] };
                            var interpolationmethod = Interpolate.Linear(ytwopointList, xtwopointList);
                            ugsoffwithinterpolationList.Add(interpolationmethod.Interpolate(yListdouble[zeroIndex] / 1000));

                            ugsoffList.Add((xListdouble[i] + xListdouble[i + 1]) / 2);
                            isugsoff = true;
                        }
                        else
                        {
                            ugsoffwithinterpolationList.Add(xListdouble[i]);
                            ugsoffList.Add(xListdouble[i]);
                            isugsoff = true;
                        }

                        i = yListdouble.Count - 1;
                    }

                }

                for (int i = 0; i < yListdouble.Count; i++)
                {

                    if (yListdouble[i] < yListdouble[zeroIndex] / 100)
                    {
                        if (i < yListdouble.Count - 1)
                        {
                            var xtwopointList = new List<double> { xListdouble[i], xListdouble[i + 1] };
                            var ytwopointList = new List<double> { yListdouble[i], yListdouble[i + 1] };
                            var interpolationmethod100 = Interpolate.Linear(ytwopointList, xtwopointList);
                            ugsoffwithinterpolation100List.Add(interpolationmethod100.Interpolate(yListdouble[zeroIndex] / 100));



                        }
                        else
                        {
                            ugsoffwithinterpolation100List.Add(xListdouble[i]);

                        }

                        i = yListdouble.Count - 1;
                    }

                }
                if (!isugsoff)
                {
                    ugsoffList.Add(double.NaN);
                    ugsoffwithinterpolationList.Add(double.NaN);
                }



            }
            if (ugsoffList.Count(x => Double.IsNaN(x)) == ugsoffList.Count())
            {
                foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
                {
                    ugsoffList.Clear();
                    var isugsoff = false;
                    for (int i = 0; i < yListdouble.Count; i++)
                    {

                        if (yListdouble[i] < yListdouble[zeroIndex] / 100)
                        {
                            if (i < yListdouble.Count - 1)
                            {
                                ugsoffList.Add((xListdouble[i] + xListdouble[i + 1]) / 2);
                                var xtwopointList = new List<double> { xListdouble[i], xListdouble[i + 1] };
                                var ytwopointList = new List<double> { yListdouble[i], yListdouble[i + 1] };
                                var interpolationmethod = Interpolate.Linear(ytwopointList, xtwopointList);
                                ugsoffwithinterpolationList.Add(interpolationmethod.Interpolate(yListdouble[zeroIndex] / 100));
                                isugsoff = true;
                            }
                            else
                            {
                                ugsoffwithinterpolationList.Add(xListdouble[i]);
                                ugsoffList.Add(xListdouble[i]);
                                isugsoff = true;
                            }

                            i = yListdouble.Count - 1;
                        }
                    }
                    if (!isugsoff)
                    {
                        ugsoffList.Add(double.NaN);
                        ugsoffwithinterpolationList.Add(1E9);
                    }



                }

                var returnexList = new List<Statistics>
                {
                    GetFullStatisticsFromList(idssList, "I<sub>DSS(3V)</sub> (начальный ток стока)", unit, 3, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(ugsoffwithinterpolationList.Select(x => x*(-1)).ToList(), "U<sub>GS(off)</sub> (напряжение отсечки при Idss/100) !Транзисторы не закрываются!", "В", 4),
                    GetFullStatisticsFromList(id05List, "Id<sub>max</sub> (ток при Vgs=0.5В)", "А", 0, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(ugsoffminList.Select(x => x*(-1)).ToList(), "U<sub>GS(min)</sub> (напряжение отсечки при Imin)", "В"),

                };
                return returnexList;

            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(idssList, "I<sub>DSS(3V)</sub> (начальный ток стока)", unit, 3, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(ugsoffwithinterpolationList.Select(x => x*(-1)).ToList(), "U<sub>GS(off)</sub> (напряжение отсечки при Idss/1000)", "В", 4),
                    GetFullStatisticsFromList(id05List, "Id<sub>max</sub> (ток при Vgs=0.5В)", "А", 0, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(ugsoffwithinterpolation100List.Select(x => x*(-1)).ToList(), "U<sub>GS-100(off)</sub> (напряжение отсечки при Idss/100)", "В"),
                    GetFullStatisticsFromList(ugsoffminList.Select(x => x*(-1)).ToList(), "U<sub>GS(min)</sub> (напряжение при Imin)", "В"),

                };
            return returnList;
        }

        private List<Statistics> GetIdssAndUgsoff_Progress(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var zeroIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.0 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.5 - item)).FirstOrDefault());
            var idssList = new List<double>();
            var id05List = new List<double>();
            var ugsoffwithinterpolationList = new List<double>();
            var ugsoffwithinterpolation100List = new List<double>();
            var ugsoffList = new List<double>();
            var ugsoffminList = new List<double>();
            var unit = "A";
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList()))
            {

                idssList.Add(yListdouble[zeroIndex] * 1.1 / divider);

            }

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                var isugsoff = false;

                id05List.Add(yListdouble[fiveIndex]);
                ugsoffminList.Add(xListdouble[yListdouble.IndexOf(yListdouble.Min())]);
                for (int i = yListdouble.Count - 1; i > -1; i--)
                {

                    if (yListdouble[i] < yListdouble[zeroIndex] / 1000)
                    {
                        if (i < yListdouble.Count - 1)
                        {
                            var xtwopointList = new List<double> { xListdouble[i], xListdouble[i + 1] };
                            var ytwopointList = new List<double> { yListdouble[i], yListdouble[i + 1] };
                            var interpolationmethod = Interpolate.Linear(ytwopointList, xtwopointList);
                            ugsoffwithinterpolationList.Add(interpolationmethod.Interpolate(yListdouble[zeroIndex] / 1000));

                            ugsoffList.Add((xListdouble[i] + xListdouble[i + 1]) / 2);
                            isugsoff = true;
                        }
                        else
                        {
                            ugsoffwithinterpolationList.Add(xListdouble[i]);
                            ugsoffList.Add(xListdouble[i]);
                            isugsoff = true;
                        }

                        i = -1;
                    }

                }

                for (int i = yListdouble.Count - 1; i > -1; i--)
                {

                    if (yListdouble[i] < yListdouble[zeroIndex] / 100)
                    {
                        if (i < yListdouble.Count - 1)
                        {
                            var xtwopointList = new List<double> { xListdouble[i], xListdouble[i + 1] };
                            var ytwopointList = new List<double> { yListdouble[i], yListdouble[i + 1] };
                            var interpolationmethod100 = Interpolate.Linear(ytwopointList, xtwopointList);
                            ugsoffwithinterpolation100List.Add(interpolationmethod100.Interpolate(yListdouble[zeroIndex] / 100));



                        }
                        else
                        {
                            ugsoffwithinterpolation100List.Add(xListdouble[i]);

                        }

                        i = -1;
                    }

                }
                if (!isugsoff)
                {
                    ugsoffList.Add(double.NaN);
                    ugsoffwithinterpolationList.Add(double.NaN);
                }



            }
            if (ugsoffList.Count(x => Double.IsNaN(x)) == ugsoffList.Count())
            {
                foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
                {
                    ugsoffList.Clear();
                    var isugsoff = false;
                    for (int i = yListdouble.Count - 1; i > -1; i--)
                    {

                        if (yListdouble[i] < yListdouble[zeroIndex] / 100)
                        {
                            if (i < yListdouble.Count - 1)
                            {
                                ugsoffList.Add((xListdouble[i] + xListdouble[i + 1]) / 2);
                                var xtwopointList = new List<double> { xListdouble[i], xListdouble[i + 1] };
                                var ytwopointList = new List<double> { yListdouble[i], yListdouble[i + 1] };
                                var interpolationmethod = Interpolate.Linear(ytwopointList, xtwopointList);
                                ugsoffwithinterpolationList.Add(interpolationmethod.Interpolate(yListdouble[zeroIndex] / 100));
                                isugsoff = true;
                            }
                            else
                            {
                                ugsoffwithinterpolationList.Add(xListdouble[i]);
                                ugsoffList.Add(xListdouble[i]);
                                isugsoff = true;
                            }

                            i = -1;
                        }
                    }
                    if (!isugsoff)
                    {
                        ugsoffList.Add(double.NaN);
                        ugsoffwithinterpolationList.Add(double.NaN);
                    }



                }

                var returnexList = new List<Statistics>
                {
                    GetFullStatisticsFromList(idssList, "I<sub>DSS(3V)</sub> (начальный ток стока)", unit, 3, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(ugsoffwithinterpolationList, "U<sub>GS(off)</sub> (напряжение отсечки при Idss/100) !Транзисторы не закрываются!", "В", 4),
                    GetFullStatisticsFromList(id05List, "Id<sub>max</sub> (ток при Vgs=0.5В)", "А", 0, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(ugsoffminList, "U<sub>GS(min)</sub> (напряжение отсечки при Imin)", "В"),

                };
                return returnexList;

            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(idssList, "I<sub>DSS(3V)</sub> (начальный ток стока)", unit, 3, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(ugsoffwithinterpolationList, "U<sub>GS(off)</sub> (напряжение отсечки при Idss/1000)", "В", 4),
                    GetFullStatisticsFromList(id05List, "Id<sub>max</sub> (ток при Vgs=0.5В)", "А", 0, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(ugsoffwithinterpolation100List, "U<sub>GS-100(off)</sub> (напряжение отсечки при Idss/100)", "В"),
                    GetFullStatisticsFromList(ugsoffminList, "U<sub>GS(min)</sub> (напряжение при Imin)", "В"),

                };
            return returnList;
        }

        private List<Statistics> GetLR(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var zeroIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.05 - item)).FirstOrDefault());
            var zeroList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => zeroIndex < 0 ? yListdouble[0] : yListdouble[zeroIndex]).ToList();

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(zeroList, "R<sub>ind</sub> при U=0.05В (сопротивление катушки)", "Ом"),

                };
            return returnList;
        }

        private List<Statistics> GetHarmony3(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var minList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => yListdouble.Min()).ToList();
            var deltaList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => Math.Abs(yListdouble.Max() - yListdouble.Min())).ToList();
            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(minList, "Harmony_MIN", "дБм"),
                    GetFullStatisticsFromList(deltaList, "Harmony_DELTA", "дБм"),

                };
            return returnList;
        }

        private List<Statistics> GetHarmony(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var maxList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => yListdouble.Max()).ToList();
            var deltaList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => Math.Abs(yListdouble.Max() - yListdouble.Min())).ToList();
            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(maxList, "Harmony_MAX", "дБм"),
                    GetFullStatisticsFromList(deltaList, "Harmony_DELTA", "дБм"),

                };
            return returnList;
        }

        private List<Statistics> GetALFA1(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var maxList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => yListdouble.Max()).ToList();
            var minList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => yListdouble.Min()).ToList();
            var deltaList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => Math.Abs(yListdouble.Max() - yListdouble.Min())).ToList();


            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(maxList.Select(Math.Abs).ToList(), "ALFA1_MAX", "дБм", 48),
                    GetFullStatisticsFromList(minList.Select(Math.Abs).ToList(), "ALFA1_MIN", "дБм"),
                    GetFullStatisticsFromList(deltaList, "ALFA1_DELTA", "дБм"),

                };
            return returnList;
        }

        private List<Statistics> GetALFA2(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var maxList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => yListdouble.Max()).ToList();
            var minList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => yListdouble.Min()).ToList();
            var deltaList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => Math.Abs(yListdouble.Max() - yListdouble.Min())).ToList();


            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(maxList.Select(Math.Abs).ToList(), "ALFA2_MAX", "дБм", 46),
                    GetFullStatisticsFromList(minList.Select(Math.Abs).ToList(), "ALFA2_MIN", "дБм"),
                    GetFullStatisticsFromList(deltaList, "ALFA2_DELTA", "дБм"),

                };
            return returnList;
        }

        private List<Statistics> GetALFA3(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var maxList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => yListdouble.Max()).ToList();
            var minList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => yListdouble.Min()).ToList();
            var deltaList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => Math.Abs(yListdouble.Max() - yListdouble.Min())).ToList();


            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(maxList.Select(Math.Abs).ToList(), "ALFA3_MAX", "дБм", 49),
                    GetFullStatisticsFromList(minList.Select(Math.Abs).ToList(), "ALFA3_MIN", "дБм"),
                    GetFullStatisticsFromList(deltaList, "ALFA3_DELTA", "дБм"),

                };
            return returnList;
        }


        private List<Statistics> GetALFA4(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var maxList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => yListdouble.Max()).ToList();
            var minList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => yListdouble.Min()).ToList();
            var deltaList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => Math.Abs(yListdouble.Max() - yListdouble.Min())).ToList();



            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(maxList.Select(Math.Abs).ToList(), "ALFA4_MAX", "дБм", 47),
                    GetFullStatisticsFromList(minList.Select(Math.Abs).ToList(), "ALFA4_MIN", "дБм"),
                    GetFullStatisticsFromList(deltaList, "ALFA4_DELTA", "дБм"),

                };
            return returnList;
        }

        private List<Statistics> GetALFA5(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var maxList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => yListdouble.Max()).ToList();
            var minList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => yListdouble.Min()).ToList();
            var deltaList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => Math.Abs(yListdouble.Max() - yListdouble.Min())).ToList();


            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(maxList.Select(Math.Abs).ToList(), "ALFA5_MAX", "дБм"),
                    GetFullStatisticsFromList(minList.Select(Math.Abs).ToList(), "ALFA5_MIN", "дБм"),
                    GetFullStatisticsFromList(deltaList, "ALFA5_DELTA", "дБм"),

                };
            return returnList;
        }


        private List<Statistics> GetCL(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var maxList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => yListdouble.Max()).ToList();
            var minList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => yListdouble.Min()).ToList();
            var deltaList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => Math.Abs(yListdouble.Max() - yListdouble.Min())).ToList();


            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(maxList, "ConversionLoss_MAX", "дБм", 45),
                    GetFullStatisticsFromList(minList, "ConversionLoss_MIN", "дБм"),
                    GetFullStatisticsFromList(deltaList, "ConversionLoss_DELTA", "дБм"),

                };
            return returnList;
        }



        private List<Statistics> GetIdssAndUgsoffX5(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var zeroIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.0 - item)).FirstOrDefault());
            var ugsoffList = new List<double>();
            var iMinList = new List<double>();


            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                var isugsoff = false;
                iMinList.Add(yListdouble.Min());
                for (int i = yListdouble.Count - 1; i > -1; i--)
                {

                    if (yListdouble[i] < yListdouble[zeroIndex] / 1000)
                    {
                        if (i < yListdouble.Count - 1)
                        {
                            ugsoffList.Add((xListdouble[i] + xListdouble[i + 1]) / 2);
                            isugsoff = true;
                        }
                        else
                        {
                            ugsoffList.Add(xListdouble[i]);
                            isugsoff = true;
                        }

                        i = -1;
                    }
                }
                if (!isugsoff)
                {
                    ugsoffList.Add(double.NaN);
                }



            }
            if (ugsoffList.Count(x => Double.IsNaN(x)) == ugsoffList.Count())
            {
                foreach (
                    var yListdouble in
                        commonYList.Select(
                            yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)/divider).ToList()))
                {
                    var isugsoff = false;
                    for (int i = yListdouble.Count - 1; i > -1; i--)
                    {
                        if (yListdouble[i] < yListdouble[zeroIndex]/10)
                        {
                            if (i < yListdouble.Count - 1)
                            {
                                ugsoffList.Add((xListdouble[i] + xListdouble[i + 1])/2);
                                isugsoff = true;
                            }
                            else
                            {
                                ugsoffList.Add(xListdouble[i]);
                                isugsoff = true;
                            }

                            i = -1;
                        }

                    }
                    if (!isugsoff)
                    {
                        ugsoffList.Add(double.NaN);
                    }


                }
                var returnexList = new List<Statistics>
                {

                    GetFullStatisticsFromList(ugsoffList, "U<sub>gs(off)-10</sub>", "", 4),
                    GetFullStatisticsFromList(iMinList, "I<sub>minimal</sub>", "A", 0, DividerProfile.WithDivider)

                };
                return returnexList;

            }

            var returnList = new List<Statistics>
                {
                   GetFullStatisticsFromList(ugsoffList, "U<sub>gs(off)</sub>", "В", 4),
                   GetFullStatisticsFromList(iMinList, "I<sub>minimal</sub>", "A", 0, DividerProfile.WithDivider)
                };
            return returnList;
        }


        private List<Statistics> GetSingleStatistics(List<string> list, Graphic graphics, StatisticParameter statisticParameter)
        {

            var listdouble = list.Select(x => String.IsNullOrEmpty(x) ? Double.NaN : double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var statname = graphics.Ordinate;
            var statID = 0;
            if (statisticParameter != null)
            {
                statname = $"{statisticParameter.HTMLName} ({statisticParameter.Description})";
                statID = statisticParameter.Id;
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(listdouble, statname, graphics.OrdinateUnit, statID),
                };
            return returnList;
        }

        private List<Statistics> GetRDivided01(List<string> valueList, Graphic graphics)
        {

            var listdouble = valueList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)*10).ToList();
            var statname = graphics.Ordinate;
            var statID = 0;

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(listdouble, "Удельное " + statname, graphics.OrdinateUnit +  "/кв", statID),
                };
            return returnList;
        }

        private List<Statistics> GetRDivided01_1DOT1(List<string> valueList, Graphic graphics)
        {

            var listdouble = valueList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) * 10 / 1.1).ToList();
            var statname = graphics.Ordinate;
            var statID = 0;

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(listdouble, "Удельное " + statname, graphics.OrdinateUnit +  "/кв", statID),
                };
            return returnList;
        }

        private List<Statistics> GetCAPLEAKVA50N(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {

            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var vbrList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList()))
            {
                var indexes = new List<int>();
                for (int index = 0; index < yListdouble.Count; index++)
                {
                    double d = yListdouble[index];
                    if (d >= 1E-7)
                    {
                        if (index < xListdouble.Count)
                        {
                            indexes.Add(index);
                        }

                    }
                }
                vbrList.Add(indexes.Count == 0
                                ? xListdouble.Last()
                                : indexes.Select(index => xListdouble[index]).ToList().Min());
            }
            var twentyIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(20 - item)).FirstOrDefault());
            var twentyList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => yListdouble[twentyIndex]).ToList();

            var returnList = new List<Statistics>
                {

                   GetFullStatisticsFromList(vbrList, "V<sub>brc</sub> (пробивное напряжение МДМ-конденсатора)", "В", 50),

                };
            return returnList;
        }

          private List<Statistics> GetS21VA50N_PASSIVE(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {

            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5 - item)).FirstOrDefault());
            var twIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(20 - item)).FirstOrDefault());


            List<double> id10List = new List<double>();
            List<double> id20List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id20List.Add(yListdouble[twIndex]);
                id10List.Add(yListdouble[tenIndex]);
            }

            var returnList = new List<Statistics>
                {
                   GetFullStatisticsFromList(id10List, "S21<sub>(5GHz)</sub>", "дБ"),
                   GetFullStatisticsFromList(id20List, "S21<sub>(20GHz)</sub>", "дБ")

                };
            return returnList;
        }

        private List<Statistics> GetS11VA50N_PASSIVE(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {

            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5 - item)).FirstOrDefault());
            var twIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(20 - item)).FirstOrDefault());


            List<double> id10List = new List<double>();
            List<double> id20List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id20List.Add(yListdouble[twIndex]);
                id10List.Add(yListdouble[tenIndex]);
            }

            var returnList = new List<Statistics>
                {
                   GetFullStatisticsFromList(id10List, "S11<sub>(5GHz)</sub>", "дБ"),
                   GetFullStatisticsFromList(id20List, "S11<sub>(20GHz)</sub>", "дБ")

                };
            return returnList;
        }

        private List<Statistics> GetRDivided01_1DOT2(List<string> valueList, Graphic graphics)
        {

            var listdouble = valueList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) * 10 / 1.2).ToList();
            var statname = graphics.Ordinate;
            var statID = 0;

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(listdouble, "Удельное " + statname, graphics.OrdinateUnit +  "/кв", statID),
                };
            return returnList;
        }

        private List<Statistics> GetRDivided_NO(List<string> valueList, Graphic graphics)
        {

            var listdouble = valueList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var statname = graphics.Ordinate;
            var statID = 0;

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(listdouble, "Удельное " + statname, graphics.OrdinateUnit +  "/кв", statID),
                };
            return returnList;
        }



        private List<Statistics> GetRDivided10(List<string> valueList, Graphic graphics)
        {

            var listdouble = valueList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / 10).ToList();
            var statname = graphics.Ordinate;
            var statID = 0;

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(listdouble, "Удельное " + statname, graphics.OrdinateUnit +  "/кв", statID),
                };
            return returnList;
        }

        private List<Statistics> GetRDivided10_1DOT23(List<string> valueList, Graphic graphics)
        {

            var listdouble = valueList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / 10 / 1.23).ToList();
            var statname = graphics.Ordinate;
            var statID = 0;

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(listdouble, "Удельное " + statname, graphics.OrdinateUnit + "/кв", statID),
                };
            return returnList;
        }

        private List<Statistics> GetRDivided10_1DOT22(List<string> valueList, Graphic graphics)
        {

            var listdouble = valueList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / 10 / 1.22).ToList();
            var statname = graphics.Ordinate;
            var statID = 0;

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(listdouble, "Удельное " + statname, graphics.OrdinateUnit + "/кв", statID),
                };
            return returnList;
        }




        private List<Statistics> GetIdss(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {

            int zeroIndex = xList.IndexOf("0");
            var idssList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => yListdouble[zeroIndex]).ToList();

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(idssList, "I<sub>dss</sub> (ток при Uзи = 0В, Uси = 10В)", "А", 3, DividerProfile.WithDivider),

                };
            return returnList;
        }


        private List<Statistics> GetMongoFirstPoint(List<double> xListdouble, List<List<double>> commonYList, double divider, string type)
        {


            var idssList = commonYList.Select(yList => yList.Select(x => x / divider).ToList()).Select(yListdouble => yListdouble[0]).ToList();

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(idssList, "FirstPoint " + type , "", 3),

                };
            return returnList;
        }

        private List<Statistics> GetIdssVknee(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {

            int maxIndex = xList.IndexOf("0");
            var idssList = new List<double>();
            var ronList = new List<double>();
            var vkneeList = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.5 - item)).FirstOrDefault());
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                idssList.Add(yListdouble.Max());
                vkneeList.Add(xListdouble[yListdouble.IndexOf(yListdouble.Max())]);
                ronList.Add(xListdouble[tenIndex]/yListdouble[tenIndex]);
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(idssList, "Idss", "А", 3, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(vkneeList, "Vknee", "В"),
                    GetFullStatisticsFromList(ronList, "Ron", "Ом", 12, DividerProfile.ROnFamily)

                };
            return returnList;
        }

        private List<Statistics> GetFalconC(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {

            var c0List = new List<double>();
            var c2List = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var zeroIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.0 - item)).FirstOrDefault());
            var twoIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.0 - item)).FirstOrDefault());
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                c0List.Add(yListdouble[zeroIndex]);
                c2List.Add(twoIndex < yListdouble.Count ? yListdouble[twoIndex] : 0.0);
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(c0List, "C<sub>0V</sub>", "Ф"),
                    GetFullStatisticsFromList(c2List, "C<sub>2V</sub>", "Ф")

                };
            return returnList;
        }

        private List<Statistics> GetSnowC(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {

            var c0List = new List<double>();
            var c10List = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var zeroIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.0 - item)).FirstOrDefault());
            var twoIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10.0 - item)).FirstOrDefault());
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                c0List.Add(yListdouble[zeroIndex]);
                c10List.Add(twoIndex < yListdouble.Count ? yListdouble[twoIndex] : 0.0);
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(c0List, "C<sub>0V</sub>", "Ф"),
                    GetFullStatisticsFromList(c10List, "C<sub>10V</sub>", "Ф")

                };
            return returnList;
        }



        private List<Statistics> GetRon(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            int zeroIndex = 0;
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            for (int index = 0; index < xListdouble.Count; index++)
            {
                double d = xListdouble[index];
                if (Math.Abs(d - 0.02) < 0.005)
                {
                    zeroIndex = index;
                }

            }

            var ronList = new List<double>();
            if (divider < 1)
            {
                divider = 1 / divider;
            }
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                ronList.Add(yListdouble[zeroIndex]);
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(ronList, "r<sub>DS(on)</sub> (сопротивление открытого канала при Uси = 0.02В)", "Ом", 12, DividerProfile.ROnFamily),
                };
            return returnList;
        }

        private List<Statistics> GetRchain(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            int zeroIndex = 0;
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            for (int index = 0; index < xListdouble.Count; index++)
            {
                double d = xListdouble[index];
                if (Math.Abs(d - 0.3) < 0.005)
                {
                    zeroIndex = index;
                }

            }

            var ronList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                ronList.Add(yListdouble[zeroIndex]);
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(ronList, "R<sub>chain</sub> (сопротивление при U=0.3В)", "Ом"),
                };
            return returnList;
        }

        private List<Statistics> GetRind(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            int zeroIndex = 0;
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            for (int index = 0; index < xListdouble.Count; index++)
            {
                double d = xListdouble[index];
                if (Math.Abs(d - 0.05) < 0.003)
                {
                    zeroIndex = index;
                }

            }

            var ronList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                ronList.Add(yListdouble[zeroIndex]);
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(ronList, "R<sub>ind</sub> (сопротивление при U=0.5В)", "Ом"),
                };
            return returnList;
        }

        private List<Statistics> GetRchain01(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            int zeroIndex = 0;
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            for (int index = 0; index < xListdouble.Count; index++)
            {
                double d = xListdouble[index];
                if (Math.Abs(d - 0.1) < 0.005)
                {
                    zeroIndex = index;
                }

            }

            var ronList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                ronList.Add(yListdouble[zeroIndex]);
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(ronList, "R<sub>chain</sub> (сопротивление при U=0.1В)", "Ом"),
                };
            return returnList;
        }

        private List<Statistics> GetGmax(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var unit = "См";
            var gmaxList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => yListdouble.Max()).ToList();
            var vpeakList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => xListdouble[yListdouble.IndexOf(yListdouble.Max())]).ToList();
            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(gmaxList, "g<sub>max</sub> (максимум крутизны)", unit, 6, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(vpeakList, "V<sub>GMmpeak</sub> (напряжение при Gmax)", "В")
                };
            return returnList;
        }


        private List<Statistics> GetVbr(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var vbrList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList()))
            {
                var indexes = new List<int>();
                for (int index = 0; index < yListdouble.Count; index++)
                {
                    double d = yListdouble[index];
                    if (Math.Abs(d) > 4.5E-4)
                    {
                        if (index < xListdouble.Count)
                        {
                            indexes.Add(index);
                        }

                    }
                }
                vbrList.Add(indexes.Count == 0
                                ? xListdouble.Last()
                                : indexes.Select(index => xListdouble[index]).ToList().Min());
            }
            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vbrList, "U<sub>BR</sub> (напряжение пробоя сток-исток)", "В", 15),
                };
            return returnList;
        }

        private List<Statistics> GetVbrCherry(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var vbrList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList()))
            {
                var indexes = new List<int>();
                for (int index = 0; index < yListdouble.Count; index++)
                {
                    double d = yListdouble[index];
                    if (Math.Abs(d) > 9.9E-4)
                    {
                        if (index < xListdouble.Count)
                        {
                            indexes.Add(index);
                        }

                    }
                }
                vbrList.Add(indexes.Count == 0
                                ? xListdouble.Last()
                                : indexes.Select(index => xListdouble[index]).ToList().Min());
            }
            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vbrList, "V<sub>br</sub> (напряжение пробоя сток-исток)", "В", 15),
                };
            return returnList;
        }



        private List<Statistics> GetLeak(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10.0 - item)).FirstOrDefault());
            var fiftyIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(50.0 - item)).FirstOrDefault());
            var hundredIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(100.0 - item)).FirstOrDefault());

            var ig10List = new List<double>();
            var ig50List = new List<double>();
            var ig100List = new List<double>();

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                ig10List.Add(tenIndex < 0 ? yListdouble[0] : yListdouble[tenIndex]);
                ig50List.Add(fiftyIndex < 0 ? yListdouble[0] : yListdouble[fiftyIndex]);
                ig100List.Add(hundredIndex < 0 ? yListdouble[0] : yListdouble[hundredIndex]);
            }

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(ig10List, "I(V=10В) ток утечки мезы", "А"),
                    GetFullStatisticsFromList(ig50List, "I(V=50В) ток утечки мезы", "А"),
                    GetFullStatisticsFromList(ig100List, "I(V=100В) ток утечки мезы", "А")
                };
            return returnList;
        }



        private List<Statistics> GetRLine(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var zeroIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.3 - item)).FirstOrDefault());
            var zeroList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => zeroIndex < 0 ? yListdouble[0] : yListdouble[zeroIndex]).ToList();

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(zeroList, "R<sub>16</sub> при R=0.3В (гальваника)", "Ом"),

                };
            return returnList;
        }

        private List<Statistics> GetRLine05(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var zeroIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.5 - item)).FirstOrDefault());
            var zeroList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => zeroIndex < 0 ? yListdouble[0] : yListdouble[zeroIndex]).ToList();

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(zeroList, "R<sub>line</sub> при R=0.5В ", "Ом"),

                };
            return returnList;
        }

        private List<Statistics> GetR02(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var zeroIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.2 - item)).FirstOrDefault());
            var zeroList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => zeroIndex < 0 ? yListdouble[0] : yListdouble[zeroIndex]).ToList();

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(zeroList, "R(V=0.2В)", "Ом"),

                };

            return returnList;
        }

        private List<Statistics> GetRgate(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var zeroIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.2 - item)).FirstOrDefault());


            var zeroList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => zeroIndex < 0 ? yListdouble[0] : yListdouble[zeroIndex]).ToList();

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(zeroList, "R<sub>6</sub> при U=0.3B (затвор)", "Ом"),

                };
            return returnList;
        }

        private List<Statistics> GetRchain1(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var zeroIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.2 - item)).FirstOrDefault());


            var zeroList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => zeroIndex < 0 ? yListdouble[0] : yListdouble[zeroIndex]).ToList();

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(zeroList, "R<sub>11</sub>(при U=0.3B)", "Ом"),

                };
            return returnList;
        }

        private List<Statistics> GetRchain2(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var zeroIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.2 - item)).FirstOrDefault());


            var zeroList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => zeroIndex < 0 ? yListdouble[0] : yListdouble[zeroIndex]).ToList();

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(zeroList, "R<sub>12</sub>(при U=0.3B)", "Ом"),

                };
            return returnList;
        }
        private List<Statistics> GetRTFR(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var zeroIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.2 - item)).FirstOrDefault());


            var zeroList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => zeroIndex < 0 ? yListdouble[0] : yListdouble[zeroIndex]).ToList();

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(zeroList, "R<sub>TFR</sub> при U=0.3B (TFR-резистор)", "Ом"),

                };
            return returnList;
        }

        private List<Statistics> GetRisol(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());
            var sixIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(6 - item)).FirstOrDefault());

            var tenList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => tenIndex < 0 ? 10 / yListdouble[0] * 1E-9 : 10 / yListdouble[tenIndex] * 1E-9).ToList();
            var sixList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => sixIndex < 0 ? 6 / yListdouble[0] * 1E-9 : 6 / yListdouble[sixIndex] * 1E-9).ToList();

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(sixList, "R<sub>isol</sub>(V=6В)", "ГОм"),
                    GetFullStatisticsFromList(tenList, "R<sub>isol</sub>(V=10В)", "ГОм")


                };
            return returnList;
        }

        private List<Statistics> GetTFRLong(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var zeroIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.3 - item)).FirstOrDefault());


            var zeroList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => zeroIndex < 0 ? yListdouble[0] : yListdouble[zeroIndex]).ToList();

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(zeroList, "R<sub>14</sub> при R=0.6В (змейка TFR)", "кОм"),

                };

            return returnList;
        }

        private List<Statistics> GetCAPPCM(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var zeroIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.0 - item)).FirstOrDefault());


            var zeroList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => zeroIndex < 0 ? yListdouble[0] : yListdouble[zeroIndex]).ToList();

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(zeroList, "C при U=0В", "Ф"),
                    GetFullStatisticsFromList(zeroList.Select(x=>x*1E14).ToList(), "C<sub>MIM</sub>при U=0B (удельная ёмкость МДМ-конденсатора)", "пФ/мм²", 37),

                };
            return returnList;
        }

        private List<Statistics> GetCAPPCM025(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var zeroIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.0 - item)).FirstOrDefault());


            var zeroList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => zeroIndex < 0 ? yListdouble[0] : yListdouble[zeroIndex]).ToList();

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(zeroList, "C при U=0В", "Ф")


                };
            return returnList;
        }


        private List<Statistics> GetCMIM_BURN(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var zeroIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.06 - item)).FirstOrDefault());


            var zeroList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => zeroIndex < 0 ? yListdouble[0] : yListdouble[zeroIndex]).ToList();

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(zeroList, "C при U=0.06В", "Ф"),
                    GetFullStatisticsFromList(zeroList.Select(x=>x*1E12/0.0625).ToList(), "C<sub>MIM</sub> при U=0.06B (удельная ёмкость МДМ-конденсатора)", "пФ/мм²", 37)

                };
            return returnList;
        }

        private List<Statistics> GetCMIM_SNOW(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var zeroIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.06 - item)).FirstOrDefault());


            var zeroList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => zeroIndex < 0 ? yListdouble[0] : yListdouble[zeroIndex]).ToList();

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(zeroList, "C при U=0.06В", "Ф"),
                    GetFullStatisticsFromList(zeroList.Select(x=>x*1E12/0.01).ToList(), "C<sub>MIM</sub> при U=0.06B (удельная ёмкость МДМ-конденсатора)", "пФ/мм²", 37)

                };
            return returnList;
        }

        private List<Statistics> GetCMIM(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var zeroIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.06 - item)).FirstOrDefault());


            var zeroList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => zeroIndex < 0 ? yListdouble[0] : yListdouble[zeroIndex]).ToList();

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(zeroList, "C при U=0.06В", "Ф"),
                    GetFullStatisticsFromList(zeroList.Select(x=>x*1E12/0.09).ToList(), "C<sub>MIM</sub> при U=0.06B (удельная ёмкость МДМ-конденсатора)", "пФ/мм²", 37)

                };
            return returnList;
        }


        private List<Statistics> GetCMIM003(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var zeroIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.06 - item)).FirstOrDefault());


            var zeroList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => zeroIndex < 0 ? yListdouble[0] : yListdouble[zeroIndex]).ToList();

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(zeroList, "C при U=0.06В", "Ф"),
                    GetFullStatisticsFromList(zeroList.Select(x=>x*1E12/0.03).ToList(), "C<sub>MIM</sub> при U=0.06B (удельная ёмкость МДМ-конденсатора)", "пФ/мм²", 37)

                };
            return returnList;
        }

        private List<Statistics> GetCMIM0025(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var zeroIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.06 - item)).FirstOrDefault());


            var zeroList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => zeroIndex < 0 ? yListdouble[0] : yListdouble[zeroIndex]).ToList();

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(zeroList, "C при U=0.06В", "Ф"),
                    GetFullStatisticsFromList(zeroList.Select(x=>x*1E12/0.0025).ToList(), "C<sub>MIM</sub> при U=0.06B (удельная ёмкость МДМ-конденсатора)", "пФ/мм²", 37)

                };
            return returnList;
        }

        private List<Statistics> GetI(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var zeroIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.2 - item)).FirstOrDefault());


            var zeroList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => zeroIndex < 0 ? yListdouble[0] : yListdouble[zeroIndex]).ToList();

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(zeroList, "I(V=0.2В)", "А"),

                };
            return returnList;
        }




        private List<Statistics> GetCapLeaks(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var zeroIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(20.0 - item)).FirstOrDefault());


            var zeroList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => zeroIndex < 0 ? yListdouble[0] : yListdouble[zeroIndex]).ToList();

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(zeroList, "I(V=20В)", "А"),

                };
            return returnList;
        }



        private List<Statistics> GetIgss(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(-10 - item)).FirstOrDefault());
            var threeIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(-3 - item)).FirstOrDefault());
            var ig3List = new List<double>();
            var ig10List = new List<double>();
            var vbdgList = new List<double>();
            var unit = "A";
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                ig3List.Add(yListdouble[threeIndex]);
                ig10List.Add(tenIndex < 0 ? yListdouble[0] : yListdouble[tenIndex]);
                var indexes = new List<int>();
                for (int index = 0; index < yListdouble.Count; index++)
                {
                    double d = yListdouble[index];
                    if (Math.Abs(d) >= 0.0001)
                    {
                        if (index < xListdouble.Count)
                        {
                            indexes.Add(index);
                        }

                    }
                }

                vbdgList.Add(indexes.Count == 0
                                ? xListdouble[0]
                                : indexes.Select(index => xListdouble[index]).ToList().Max());
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(ig3List, "I<sub>GSS(-3V)</sub> (ток утечки затвора при Uзи=-3В)", unit, 7, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(ig10List, "I<sub>GSS(-10V)</sub> (ток утечки затвора при Uзи=-10В)", unit, 8, DividerProfile.WithDivider)

                };
            return returnList;
        }

        private List<Statistics> GetIgss10V(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {

            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(-5 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(-10 - item)).FirstOrDefault());
            var ig5List = new List<double>();
            var ig10List = new List<double>();
            var igMaxList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                ig5List.Add(fiveIndex < 0 ? yListdouble[0] : yListdouble[fiveIndex]);
                ig10List.Add(tenIndex < 0 ? yListdouble[0] : yListdouble[tenIndex]);
                igMaxList.Add(xListdouble[yListdouble.Select(x => Math.Abs(x)).ToList().IndexOf(yListdouble.Select(x => Math.Abs(x)).Max())]);
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(ig5List, "I(V=-5В)", "А", 0, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(ig10List, "I(V=-10В)", "А", 0, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(igMaxList, "V(Ig_max)", "В", 0, DividerProfile.WithDivider)
                };
            return returnList;
        }

        private List<Statistics> GetIgss1E6(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {

            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var twoIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.0 - item)).FirstOrDefault());
            var ig2List = new List<double>();
            var ig10List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                ig2List.Add(twoIndex < 0 ? yListdouble[0] : yListdouble[twoIndex]);
                ig10List.Add(xListdouble[yListdouble.IndexOf(yListdouble.OrderBy(item => Math.Abs(1E-6 - item)).FirstOrDefault())]);
            }


            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(ig10List, "V(I=1E-6В)", "В", 0, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(ig2List, "I(V=2В)", "А", 0, DividerProfile.WithDivider)
                };
            return returnList;
        }

        private List<Statistics> GetS21OFFMarkers(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var twohalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(1 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());
            var eightteenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(20 - item)).FirstOrDefault());
            var id25List = new List<double>();
            var id5List = new List<double>();
            var id10List = new List<double>();
            var id18List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id25List.Add(yListdouble[twohalfIndex]);
                id5List.Add(yListdouble[fiveIndex]);
                id10List.Add(yListdouble[tenIndex]);
                id18List.Add(yListdouble[eightteenIndex]);



            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id25List, "S<sub>21OFF(1GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id5List,  "S<sub>21OFF(5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id10List, "S<sub>21OFF(10GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id18List, "S<sub>21OFF(20GHz)</sub>", "дБ"),

                };
            return returnList;
        }

        private List<Statistics> GetS21OFFMarkersBURN(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var twohalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(1 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());
            var eightteenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(20 - item)).FirstOrDefault());
            var id25List = new List<double>();
            var id5List = new List<double>();
            var id10List = new List<double>();
            var id18List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                id25List.Add(yListdouble[twohalfIndex]);
                id5List.Add(yListdouble[fiveIndex]);
                id10List.Add(yListdouble[tenIndex]);
                id18List.Add(yListdouble[eightteenIndex]);
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id25List, "S<sub>21OFF(1GHz)</sub>", "дБ")
                };
            return returnList;
        }

        private List<Statistics> GetFilterMarkersORION010(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var index3 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(3.0 - item)).FirstOrDefault());
            var index11 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(11.0 - item)).FirstOrDefault());

            var id3List = new List<double>();
            var id11List = new List<double>();
            var A0List = new List<double>();

            var bandfnList = new List<double>();
            var bandfbList = new List<double>();
            var bandfdleftList = new List<double>();
            var bandfdrightList = new List<double>();


            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id3List.Add(yListdouble[index3]);
                id11List.Add(yListdouble[index11]);

                var maxIndex = yListdouble.IndexOf(yListdouble.Max());
                A0List.Add(yListdouble.Max());

                var leftsideList = yListdouble.Take(maxIndex + 1).ToList();
                var rightsideList = yListdouble.Skip(maxIndex + 1).ToList();
                var leftIndex =
                    leftsideList.IndexOf(
                        leftsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 1 - item)).FirstOrDefault());
                var rightIndex =
                    rightsideList.IndexOf(
                        rightsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 1 - item)).FirstOrDefault()) +
                    maxIndex + 1;

                bandfnList.Add(xListdouble[leftIndex]);
                bandfbList.Add(xListdouble[rightIndex]);
                //bandfdleftList.Add(Math.Abs(yListdouble[xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(xListdouble[leftIndex] - 0.5 - item)).FirstOrDefault())]));
                //bandfdrightList.Add(Math.Abs(yListdouble[xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(xListdouble[rightIndex] + 0.5 - item)).FirstOrDefault())]));


            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id3List, "Потери на 3ГГц", "дБ"),
                    GetFullStatisticsFromList(id11List, "Потери на 11ГГц", "дБ"),
                    GetFullStatisticsFromList(A0List, "A0", "дБ"),
                    GetFullStatisticsFromList(bandfnList, "Fn", "ГГц"),
                    GetFullStatisticsFromList(bandfbList, "Fb", "ГГц"),
                    //GetFullStatisticsFromList(bandfdleftList, "FD_left", "ГГц", 55),
                    //GetFullStatisticsFromList(bandfdrightList, "FD_right", "ГГц", 56)

                };
            return returnList;
        }
        private List<Statistics> GetFilterCKBA_PF1(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var indexA0F0 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(1.1 - item)).FirstOrDefault());

            var indexAIn1 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.1 - item)).FirstOrDefault());
            var indexAIn2 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.3 - item)).FirstOrDefault());
            var indexAIb1 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(1.75 - item)).FirstOrDefault());
            var indexAIb2 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5 - item)).FirstOrDefault());
            var bandainList = new List<double>();
            var bandaibList = new List<double>();

            var A0F0List = new List<double>();
            var bandfnList = new List<double>();
            var bandfbList = new List<double>();


            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                A0F0List.Add(yListdouble[indexA0F0]);
                var maxIndex = yListdouble.IndexOf(yListdouble.Max());
                var leftsideList = yListdouble.Take(maxIndex + 1).ToList();
                var rightsideList = yListdouble.Skip(maxIndex + 1).ToList();
                var leftIndex = leftsideList.IndexOf(leftsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3.0 - item)).FirstOrDefault());
                var rightIndex = rightsideList.IndexOf(rightsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3.0 - item)).FirstOrDefault()) + maxIndex + 1;
                var interpolationfn = leftIndex == yListdouble.Count - 1 ? MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[leftIndex - 1], yListdouble[leftIndex] }, new List<double> { xListdouble[leftIndex -1], xListdouble[leftIndex] }) : MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[leftIndex], yListdouble[leftIndex + 1]}, new List<double> { xListdouble[leftIndex], xListdouble[leftIndex + 1]});
                var interpolationfb = rightIndex == yListdouble.Count - 1 ? MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex - 1], yListdouble[rightIndex]}, new List<double> { xListdouble[rightIndex - 1], xListdouble[rightIndex]}) : MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex], yListdouble[rightIndex + 1]}, new List<double> { xListdouble[rightIndex], xListdouble[rightIndex + 1]});
                bandfnList.Add(interpolationfn.Interpolate(yListdouble.Max() - 3.0));
                bandfbList.Add(interpolationfb.Interpolate(yListdouble.Max() - 3.0));
                if (Double.IsInfinity(interpolationfn.Interpolate(yListdouble.Max() - 3.0)))
                {
                    bandfnList.RemoveAll(x => Double.IsInfinity(x));
                    bandfnList.Add(yListdouble[leftIndex]);
                }
                if (Double.IsInfinity(interpolationfb.Interpolate(yListdouble.Max() - 3.0)))
                {
                    bandfbList.RemoveAll(x => Double.IsInfinity(x));
                    bandfbList.Add(yListdouble[rightIndex]);
                }

                bandainList.Add(Math.Abs(yListdouble.GetRange(indexAIn1, indexAIn2 - indexAIn1).Max() - yListdouble.Max()));
                bandaibList.Add(Math.Abs(yListdouble.GetRange(indexAIb1, indexAIb2 - indexAIb1).Max() - yListdouble.Max()));
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(A0F0List, "A0/F0<sub>(1.1GHz)</sub>", "дБ", 42),
                    GetFullStatisticsFromList(bandfnList, "Fn", "ГГц", 55),
                    GetFullStatisticsFromList(bandfbList, "Fb", "ГГц", 56),
                    GetFullStatisticsFromList(bandainList, "AIn<sub>(0.1Ghz-0.3Ghz)</sub>", "дБ", 60),
                    GetFullStatisticsFromList(bandaibList, "AIb<sub>(1.75Ghz-5Ghz)</sub>", "дБ", 61)
                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF1_VSWR1(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var vswrMaxList = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var leftIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x >= 1.0));
            var rightIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x > 1.2)) - 1;
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                vswrMaxList.Add(yListdouble.GetRange(leftIndex, rightIndex - leftIndex).Max());
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vswrMaxList, "КСВН1 в полосе<sub>(1GHz-1.2GHz)</sub>", "", 57)

                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF1_VSWR2(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var vswrMaxList = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var leftIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x >= 1.0));
            var rightIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x > 1.2)) - 1;
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                vswrMaxList.Add(yListdouble.GetRange(leftIndex, rightIndex - leftIndex).Max());
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vswrMaxList, "КСВН2 в полосе<sub>(1GHz-1.2GHz)</sub>", "", 58)

                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF2(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var indexA0F0 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(1.35 - item)).FirstOrDefault());
            var A0F0List = new List<double>();
            var bandfnList = new List<double>();
            var bandfbList = new List<double>();
            var bandaiList = new List<double>();

            var indexAIn1 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.1 - item)).FirstOrDefault());
            var indexAIn2 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.42 - item)).FirstOrDefault());
            var indexAIb1 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.16 - item)).FirstOrDefault());
            var indexAIb2 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5 - item)).FirstOrDefault());
            var bandainList = new List<double>();
            var bandaibList = new List<double>();

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                A0F0List.Add(yListdouble[indexA0F0]);
                var maxIndex = yListdouble.IndexOf(yListdouble.Max());
                var leftsideList = yListdouble.Take(maxIndex + 1).ToList();
                var rightsideList = yListdouble.Skip(maxIndex + 1).ToList();
                var leftIndex = leftsideList.IndexOf(leftsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3.0 - item)).FirstOrDefault());
                var rightIndex = rightsideList.IndexOf(rightsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3.0 - item)).FirstOrDefault()) + maxIndex + 1;
                var interpolationfn = leftIndex == yListdouble.Count - 1 ? MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[leftIndex - 1], yListdouble[leftIndex] }, new List<double> { xListdouble[leftIndex - 1], xListdouble[leftIndex] }) : MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[leftIndex], yListdouble[leftIndex + 1] }, new List<double> { xListdouble[leftIndex], xListdouble[leftIndex + 1] });
                var interpolationfb = rightIndex == yListdouble.Count - 1 ? MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex - 1], yListdouble[rightIndex] }, new List<double> { xListdouble[rightIndex - 1], xListdouble[rightIndex] }) : MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex], yListdouble[rightIndex + 1] }, new List<double> { xListdouble[rightIndex], xListdouble[rightIndex + 1] });
                bandfnList.Add(interpolationfn.Interpolate(yListdouble.Max() - 3.0));
                bandfbList.Add(interpolationfb.Interpolate(yListdouble.Max() - 3.0));
                if (Double.IsInfinity(interpolationfn.Interpolate(yListdouble.Max() - 3.0)))
                {
                    bandfnList.RemoveAll(x => Double.IsInfinity(x));
                    bandfnList.Add(yListdouble[leftIndex]);
                }
                if (Double.IsInfinity(interpolationfb.Interpolate(yListdouble.Max() - 3.0)))
                {
                    bandfbList.RemoveAll(x => Double.IsInfinity(x));
                    bandfbList.Add(yListdouble[rightIndex]);
                }
                bandainList.Add(Math.Abs(yListdouble.GetRange(indexAIn1, indexAIn2 - indexAIn1).Max() - yListdouble.Max()));
                bandaibList.Add(Math.Abs(yListdouble.GetRange(indexAIb1, indexAIb2 - indexAIb1).Max() - yListdouble.Max()));
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(A0F0List, "A0/F0<sub>(1.35GHz)</sub>", "дБ", 42),
                    GetFullStatisticsFromList(bandfnList, "Fn", "ГГц", 55),
                    GetFullStatisticsFromList(bandfbList, "Fb", "ГГц", 56),
                    GetFullStatisticsFromList(bandainList, "AIn<sub>(0.1Ghz-0.42Ghz)</sub>", "дБ", 60),
                    GetFullStatisticsFromList(bandaibList, "AIb<sub>(2.16Ghz-5Ghz)</sub>", "дБ", 61)
                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF2_VSWR1(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var vswrMaxList = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var leftIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x >= 1.2));
            var rightIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x > 1.5)) - 1;
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                vswrMaxList.Add(yListdouble.GetRange(leftIndex, rightIndex - leftIndex).Max());
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vswrMaxList, "КСВН1 в полосе<sub>(1.2GHz-1.5GHz)</sub>", "", 57)

                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF2_VSWR2(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var vswrMaxList = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var leftIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x >= 1.2));
            var rightIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x > 1.5)) - 1;
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                vswrMaxList.Add(yListdouble.GetRange(leftIndex, rightIndex - leftIndex).Max());
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vswrMaxList, "КСВН2 в полосе<sub>(1.2GHz-1.5GHz)</sub>", "", 58)

                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF3(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var indexA0F0 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(1.75 - item)).FirstOrDefault());
            var A0F0List = new List<double>();
            var bandfnList = new List<double>();
            var bandfbList = new List<double>();
            var bandaiList = new List<double>();

            var indexAIn1 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.1 - item)).FirstOrDefault());
            var indexAIn2 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(1.12 - item)).FirstOrDefault());
            var indexAIb1 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.55 - item)).FirstOrDefault());
            var indexAIb2 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5 - item)).FirstOrDefault());
            var bandainList = new List<double>();
            var bandaibList = new List<double>();

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                A0F0List.Add(yListdouble[indexA0F0]);
                var maxIndex = yListdouble.IndexOf(yListdouble.Max());
                var leftsideList = yListdouble.Take(maxIndex + 1).ToList();
                var rightsideList = yListdouble.Skip(maxIndex + 1).ToList();
                var leftIndex = leftsideList.IndexOf(leftsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3.0 - item)).FirstOrDefault());
                var rightIndex = rightsideList.IndexOf(rightsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3.0 - item)).FirstOrDefault()) + maxIndex + 1;
                var interpolationfn = leftIndex == yListdouble.Count - 1 ? MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[leftIndex - 1], yListdouble[leftIndex] }, new List<double> { xListdouble[leftIndex - 1], xListdouble[leftIndex] }) : MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[leftIndex], yListdouble[leftIndex + 1] }, new List<double> { xListdouble[leftIndex], xListdouble[leftIndex + 1] });
                var interpolationfb = rightIndex == yListdouble.Count - 1 ? MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex - 1], yListdouble[rightIndex] }, new List<double> { xListdouble[rightIndex - 1], xListdouble[rightIndex] }) : MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex], yListdouble[rightIndex + 1] }, new List<double> { xListdouble[rightIndex], xListdouble[rightIndex + 1] });
                bandfnList.Add(interpolationfn.Interpolate(yListdouble.Max() - 3.0));
                bandfbList.Add(interpolationfb.Interpolate(yListdouble.Max() - 3.0));
                if (Double.IsInfinity(interpolationfn.Interpolate(yListdouble.Max() - 3.0)))
                {
                    bandfnList.RemoveAll(x => Double.IsInfinity(x));
                    bandfnList.Add(yListdouble[leftIndex]);
                }
                if (Double.IsInfinity(interpolationfb.Interpolate(yListdouble.Max() - 3.0)))
                {
                    bandfbList.RemoveAll(x => Double.IsInfinity(x));
                    bandfbList.Add(yListdouble[rightIndex]);
                }
                bandainList.Add(Math.Abs(yListdouble.GetRange(indexAIn1, indexAIn2 - indexAIn1).Max() - yListdouble.Max()));
                bandaibList.Add(Math.Abs(yListdouble.GetRange(indexAIb1, indexAIb2 - indexAIb1).Max() - yListdouble.Max()));
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(A0F0List, "A0/F0<sub>(1.75GHz)</sub>", "дБ", 42),
                    GetFullStatisticsFromList(bandfnList, "Fn", "ГГц", 55),
                    GetFullStatisticsFromList(bandfbList, "Fb", "ГГц", 56),
                    GetFullStatisticsFromList(bandainList, "AIn<sub>(0.1Ghz-1.12Ghz)</sub>", "дБ", 60),
                    GetFullStatisticsFromList(bandaibList, "AIb<sub>(2.55Ghz-5Ghz)</sub>", "дБ", 61)
                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF3_VSWR1(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var vswrMaxList = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var leftIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x >= 1.6));
            var rightIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x > 1.9)) - 1;
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                vswrMaxList.Add(yListdouble.GetRange(leftIndex, rightIndex - leftIndex).Max());
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vswrMaxList, "КСВН1 в полосе<sub>(1.6GHz-1.9GHz)</sub>", "", 57)

                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF3_VSWR2(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var vswrMaxList = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var leftIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x >= 1.6));
            var rightIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x > 1.9)) - 1;
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                vswrMaxList.Add(yListdouble.GetRange(leftIndex, rightIndex - leftIndex).Max());
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vswrMaxList, "КСВН2 в полосе<sub>(1.6GHz-1.9GHz)</sub>", "", 58)

                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF4(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var indexA0F0 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.35 - item)).FirstOrDefault());
            var A0F0List = new List<double>();
            var bandfnList = new List<double>();
            var bandfbList = new List<double>();
            var bandaiList = new List<double>();

            var indexAIn1 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.1 - item)).FirstOrDefault());
            var indexAIn2 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(1.55 - item)).FirstOrDefault());
            var indexAIb1 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(3.17 - item)).FirstOrDefault());
            var indexAIb2 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5 - item)).FirstOrDefault());
            var bandainList = new List<double>();
            var bandaibList = new List<double>();

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                A0F0List.Add(yListdouble[indexA0F0]);
                var maxIndex = yListdouble.IndexOf(yListdouble.Max());
                var leftsideList = yListdouble.Take(maxIndex + 1).ToList();
                var rightsideList = yListdouble.Skip(maxIndex + 1).ToList();
                var leftIndex = leftsideList.IndexOf(leftsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3.0 - item)).FirstOrDefault());
                var rightIndex = rightsideList.IndexOf(rightsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3.0 - item)).FirstOrDefault()) + maxIndex + 1;
                var interpolationfn = leftIndex == yListdouble.Count - 1 ? MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[leftIndex - 1], yListdouble[leftIndex] }, new List<double> { xListdouble[leftIndex - 1], xListdouble[leftIndex] }) : MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[leftIndex], yListdouble[leftIndex + 1] }, new List<double> { xListdouble[leftIndex], xListdouble[leftIndex + 1] });
                var interpolationfb = rightIndex == yListdouble.Count - 1 ? MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex - 1], yListdouble[rightIndex] }, new List<double> { xListdouble[rightIndex - 1], xListdouble[rightIndex] }) : MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex], yListdouble[rightIndex + 1] }, new List<double> { xListdouble[rightIndex], xListdouble[rightIndex + 1] });
                bandfnList.Add(interpolationfn.Interpolate(yListdouble.Max() - 3.0));
                bandfbList.Add(interpolationfb.Interpolate(yListdouble.Max() - 3.0));
                if (Double.IsInfinity(interpolationfn.Interpolate(yListdouble.Max() - 3.0)))
                {
                    bandfnList.RemoveAll(x => Double.IsInfinity(x));
                    bandfnList.Add(yListdouble[leftIndex]);
                }
                if (Double.IsInfinity(interpolationfb.Interpolate(yListdouble.Max() - 3.0)))
                {
                    bandfbList.RemoveAll(x => Double.IsInfinity(x));
                    bandfbList.Add(yListdouble[rightIndex]);
                }
                bandainList.Add(Math.Abs(yListdouble.GetRange(indexAIn1, indexAIn2 - indexAIn1).Max() - yListdouble.Max()));
                bandaibList.Add(Math.Abs(yListdouble.GetRange(indexAIb1, indexAIb2 - indexAIb1).Max() - yListdouble.Max()));
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(A0F0List, "A0/F0<sub>(2.35GHz)</sub>", "дБ", 42),
                    GetFullStatisticsFromList(bandfnList, "Fn", "ГГц", 55),
                    GetFullStatisticsFromList(bandfbList, "Fb", "ГГц", 56),
                    GetFullStatisticsFromList(bandainList, "AIn<sub>(0.1Ghz-1.55Ghz)</sub>", "дБ", 60),
                    GetFullStatisticsFromList(bandaibList, "AIb<sub>(3.17Ghz-5Ghz)</sub>", "дБ", 61)
                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF4_VSWR1(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var vswrMaxList = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var leftIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x >= 2.2));
            var rightIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x > 2.5)) - 1;
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                vswrMaxList.Add(yListdouble.GetRange(leftIndex, rightIndex - leftIndex).Max());
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vswrMaxList, "КСВН1 в полосе<sub>(2.2GHz-2.5GHz)</sub>", "", 57)

                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF4_VSWR2(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var vswrMaxList = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var leftIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x >= 2.2));
            var rightIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x > 2.5)) - 1;
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                vswrMaxList.Add(yListdouble.GetRange(leftIndex, rightIndex - leftIndex).Max());
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vswrMaxList, "КСВН2 в полосе<sub>(2.2GHz-2.5GHz)</sub>", "", 58)

                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF5(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var indexA0F0 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(3.25 - item)).FirstOrDefault());
            var A0F0List = new List<double>();
            var bandfnList = new List<double>();
            var bandfbList = new List<double>();
            var bandaiList = new List<double>();

            var indexAIn1 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.1 - item)).FirstOrDefault());
            var indexAIn2 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.15 - item)).FirstOrDefault());
            var indexAIb1 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(4.55 - item)).FirstOrDefault());
            var indexAIb2 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(14 - item)).FirstOrDefault());
            var bandainList = new List<double>();
            var bandaibList = new List<double>();

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                A0F0List.Add(yListdouble[indexA0F0]);
                var maxIndex = yListdouble.IndexOf(yListdouble.Max());
                var leftsideList = yListdouble.Take(maxIndex + 1).ToList();
                var rightsideList = yListdouble.Skip(maxIndex + 1).ToList();
                var leftIndex = leftsideList.IndexOf(leftsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3.0 - item)).FirstOrDefault());
                var rightIndex = rightsideList.IndexOf(rightsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3.0 - item)).FirstOrDefault()) + maxIndex + 1;
                var interpolationfn = leftIndex == yListdouble.Count - 1 ? MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[leftIndex - 1], yListdouble[leftIndex] }, new List<double> { xListdouble[leftIndex - 1], xListdouble[leftIndex] }) : MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[leftIndex], yListdouble[leftIndex + 1] }, new List<double> { xListdouble[leftIndex], xListdouble[leftIndex + 1] });
                var interpolationfb = rightIndex == yListdouble.Count - 1 ? MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex - 1], yListdouble[rightIndex] }, new List<double> { xListdouble[rightIndex - 1], xListdouble[rightIndex] }) : MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex], yListdouble[rightIndex + 1] }, new List<double> { xListdouble[rightIndex], xListdouble[rightIndex + 1] });
                bandfnList.Add(interpolationfn.Interpolate(yListdouble.Max() - 3.0));
                bandfbList.Add(interpolationfb.Interpolate(yListdouble.Max() - 3.0));
                if (Double.IsInfinity(interpolationfn.Interpolate(yListdouble.Max() - 3.0)))
                {
                    bandfnList.RemoveAll(x => Double.IsInfinity(x));
                    bandfnList.Add(yListdouble[leftIndex]);
                }
                if (Double.IsInfinity(interpolationfb.Interpolate(yListdouble.Max() - 3.0)))
                {
                    bandfbList.RemoveAll(x => Double.IsInfinity(x));
                    bandfbList.Add(yListdouble[rightIndex]);
                }
                bandainList.Add(Math.Abs(yListdouble.GetRange(indexAIn1, indexAIn2 - indexAIn1).Max() - yListdouble.Max()));
                bandaibList.Add(Math.Abs(yListdouble.GetRange(indexAIb1, indexAIb2 - indexAIb1).Max() - yListdouble.Max()));
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(A0F0List, "A0/F0<sub>(3.25GHz)</sub>", "дБ", 42),
                    GetFullStatisticsFromList(bandfnList, "Fn", "ГГц", 55),
                    GetFullStatisticsFromList(bandfbList, "Fb", "ГГц", 56),
                    GetFullStatisticsFromList(bandainList, "AIn<sub>(0.1Ghz-2.15Ghz)</sub>", "дБ", 60),
                    GetFullStatisticsFromList(bandaibList, "AIb<sub>(4.55Ghz-14Ghz)</sub>", "дБ", 61)
                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF5_VSWR1(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var vswrMaxList = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var leftIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x >= 2.9));
            var rightIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x > 3.5)) - 1;
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                vswrMaxList.Add(yListdouble.GetRange(leftIndex, rightIndex - leftIndex).Max());
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vswrMaxList, "КСВН1 в полосе<sub>(2.9GHz-3.5GHz)</sub>", "", 57)

                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF5_VSWR2(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var vswrMaxList = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var leftIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x >= 2.9));
            var rightIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x > 3.5)) - 1;
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                vswrMaxList.Add(yListdouble.GetRange(leftIndex, rightIndex - leftIndex).Max());
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vswrMaxList, "КСВН2 в полосе<sub>(2.9GHz-3.5GHz)</sub>", "", 58)

                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF6(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var indexA0F0 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(4.6 - item)).FirstOrDefault());
            var A0F0List = new List<double>();
            var bandfnList = new List<double>();
            var bandfbList = new List<double>();
            var bandaiList = new List<double>();

            var indexAIn1 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.1 - item)).FirstOrDefault());
            var indexAIn2 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(3.1 - item)).FirstOrDefault());
            var indexAIb1 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(6.55 - item)).FirstOrDefault());
            var indexAIb2 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(14 - item)).FirstOrDefault());
            var bandainList = new List<double>();
            var bandaibList = new List<double>();

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                A0F0List.Add(yListdouble[indexA0F0]);
                var maxIndex = yListdouble.IndexOf(yListdouble.Max());
                var leftsideList = yListdouble.Take(maxIndex + 1).ToList();
                var rightsideList = yListdouble.Skip(maxIndex + 1).ToList();
                var leftIndex = leftsideList.IndexOf(leftsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3.0 - item)).FirstOrDefault());
                var rightIndex = rightsideList.IndexOf(rightsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3.0 - item)).FirstOrDefault()) + maxIndex + 1;
                var interpolationfn = leftIndex == yListdouble.Count - 1 ? MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[leftIndex - 1], yListdouble[leftIndex] }, new List<double> { xListdouble[leftIndex - 1], xListdouble[leftIndex] }) : MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[leftIndex], yListdouble[leftIndex + 1] }, new List<double> { xListdouble[leftIndex], xListdouble[leftIndex + 1] });
                var interpolationfb = rightIndex == yListdouble.Count - 1 ? MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex - 1], yListdouble[rightIndex] }, new List<double> { xListdouble[rightIndex - 1], xListdouble[rightIndex] }) : MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex], yListdouble[rightIndex + 1] }, new List<double> { xListdouble[rightIndex], xListdouble[rightIndex + 1] });
                bandfnList.Add(interpolationfn.Interpolate(yListdouble.Max() - 3.0));
                bandfbList.Add(interpolationfb.Interpolate(yListdouble.Max() - 3.0));
                if (Double.IsInfinity(interpolationfn.Interpolate(yListdouble.Max() - 3.0)))
                {
                    bandfnList.RemoveAll(x => Double.IsInfinity(x));
                    bandfnList.Add(yListdouble[leftIndex]);
                }
                if (Double.IsInfinity(interpolationfb.Interpolate(yListdouble.Max() - 3.0)))
                {
                    bandfbList.RemoveAll(x => Double.IsInfinity(x));
                    bandfbList.Add(yListdouble[rightIndex]);
                }
                bandainList.Add(Math.Abs(yListdouble.GetRange(indexAIn1, indexAIn2 - indexAIn1).Max() - yListdouble.Max()));
                bandaibList.Add(Math.Abs(yListdouble.GetRange(indexAIb1, indexAIb2 - indexAIb1).Max() - yListdouble.Max()));
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(A0F0List, "A0/F0<sub>(4.6GHz)</sub>", "дБ", 42),
                    GetFullStatisticsFromList(bandfnList, "Fn", "ГГц", 55),
                    GetFullStatisticsFromList(bandfbList, "Fb", "ГГц", 56),
                    GetFullStatisticsFromList(bandainList, "AIn<sub>(0.1Ghz-3.1Ghz)</sub>", "дБ", 60),
                    GetFullStatisticsFromList(bandaibList, "AIb<sub>(6.55Ghz-14Ghz)</sub>", "дБ", 61)
                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF6_VSWR1(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var vswrMaxList = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var leftIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x >= 4.1));
            var rightIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x > 5.1)) - 1;
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                vswrMaxList.Add(yListdouble.GetRange(leftIndex, rightIndex - leftIndex).Max());
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vswrMaxList, "КСВН1 в полосе<sub>(4.1GHz-5.1GHz)</sub>", "", 57)

                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF6_VSWR2(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var vswrMaxList = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var leftIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x >= 4.1));
            var rightIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x > 5.1)) - 1;
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                vswrMaxList.Add(yListdouble.GetRange(leftIndex, rightIndex - leftIndex).Max());
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vswrMaxList, "КСВН2 в полосе<sub>(4.1GHz-5.1GHz)</sub>", "", 58)

                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF7(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var indexA0F0 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(6.65 - item)).FirstOrDefault());
            var A0F0List = new List<double>();
            var bandfnList = new List<double>();
            var bandfbList = new List<double>();
            var bandaiList = new List<double>();

            var indexAIn1 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.1 - item)).FirstOrDefault());
            var indexAIn2 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(4.25 - item)).FirstOrDefault());
            var indexAIb1 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(8.95 - item)).FirstOrDefault());
            var indexAIb2 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(14 - item)).FirstOrDefault());
            var bandainList = new List<double>();
            var bandaibList = new List<double>();

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                A0F0List.Add(yListdouble[indexA0F0]);
                var maxIndex = yListdouble.IndexOf(yListdouble.Max());
                var leftsideList = yListdouble.Take(maxIndex + 1).ToList();
                var rightsideList = yListdouble.Skip(maxIndex + 1).ToList();
                var leftIndex = leftsideList.IndexOf(leftsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3.0 - item)).FirstOrDefault());
                var rightIndex = rightsideList.IndexOf(rightsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3.0 - item)).FirstOrDefault()) + maxIndex + 1;
                var interpolationfn = leftIndex == yListdouble.Count - 1 ? MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[leftIndex - 1], yListdouble[leftIndex] }, new List<double> { xListdouble[leftIndex - 1], xListdouble[leftIndex] }) : MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[leftIndex], yListdouble[leftIndex + 1] }, new List<double> { xListdouble[leftIndex], xListdouble[leftIndex + 1] });
                var interpolationfb = rightIndex == yListdouble.Count - 1 ? MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex - 1], yListdouble[rightIndex] }, new List<double> { xListdouble[rightIndex - 1], xListdouble[rightIndex] }) : MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex], yListdouble[rightIndex + 1] }, new List<double> { xListdouble[rightIndex], xListdouble[rightIndex + 1] });
                bandfnList.Add(interpolationfn.Interpolate(yListdouble.Max() - 3.0));
                    bandfbList.Add(interpolationfb.Interpolate(yListdouble.Max() - 3.0));
                if (Double.IsInfinity(interpolationfn.Interpolate(yListdouble.Max() - 3.0)))
                {
                    bandfnList.RemoveAll(x => Double.IsInfinity(x));
                    bandfnList.Add(yListdouble[leftIndex]);
                }
                if (Double.IsInfinity(interpolationfb.Interpolate(yListdouble.Max() - 3.0)))
                {
                    bandfbList.RemoveAll(x => Double.IsInfinity(x));
                    bandfbList.Add(yListdouble[rightIndex]);
                }
                bandainList.Add(Math.Abs(yListdouble.GetRange(indexAIn1, indexAIn2 - indexAIn1).Max() - yListdouble.Max()));
                bandaibList.Add(Math.Abs(yListdouble.GetRange(indexAIb1, indexAIb2 - indexAIb1).Max() - yListdouble.Max()));
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(A0F0List, "A0/F0<sub>(6.65GHz)</sub>", "дБ", 42),
                    GetFullStatisticsFromList(bandfnList, "Fn", "ГГц", 55),
                    GetFullStatisticsFromList(bandfbList, "Fb", "ГГц", 56),
                    GetFullStatisticsFromList(bandainList, "AIn<sub>(0.1Ghz-4.25Ghz)</sub>", "дБ", 60),
                    GetFullStatisticsFromList(bandaibList, "AIb<sub>(8.95Ghz-14Ghz)</sub>", "дБ", 61)
                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF7_VSWR1(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var vswrMaxList = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var leftIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x >= 5.8));
            var rightIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x > 7.65)) - 1;
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                vswrMaxList.Add(yListdouble.GetRange(leftIndex, rightIndex - leftIndex).Max());
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vswrMaxList, "КСВН1 в полосе<sub>(5.8GHz-7.65GHz)</sub>", "", 57)

                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF7_VSWR2(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var vswrMaxList = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var leftIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x >= 5.8));
            var rightIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x > 7.65)) - 1;
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                vswrMaxList.Add(yListdouble.GetRange(leftIndex, rightIndex - leftIndex).Max());
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vswrMaxList, "КСВН2 в полосе<sub>(5.8GHz-7.65GHz)</sub>", "", 58)

                };
            return returnList;
        }


        private List<Statistics> GetFilterCKBA_PF8(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var indexA0F0 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10.2 - item)).FirstOrDefault());
            var A0F0List = new List<double>();
            var bandfnList = new List<double>();
            var bandfbList = new List<double>();
            var bandaiList = new List<double>();

            var indexAIn1 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.1 - item)).FirstOrDefault());
            var indexAIn2 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(6.65 - item)).FirstOrDefault());
            var indexAIb1 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(13.85 - item)).FirstOrDefault());
            var indexAIb2 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(20 - item)).FirstOrDefault());
            var bandainList = new List<double>();
            var bandaibList = new List<double>();

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                A0F0List.Add(yListdouble[indexA0F0]);
                var maxIndex = yListdouble.IndexOf(yListdouble.Max());
                var leftsideList = yListdouble.Take(maxIndex + 1).ToList();
                var rightsideList = yListdouble.Skip(maxIndex + 1).ToList();
                var leftIndex = leftsideList.IndexOf(leftsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3.0 - item)).FirstOrDefault());
                var rightIndex = rightsideList.IndexOf(rightsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3.0 - item)).FirstOrDefault()) + maxIndex + 1;
                var interpolationfn = leftIndex == yListdouble.Count - 1 ? MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[leftIndex - 1], yListdouble[leftIndex] }, new List<double> { xListdouble[leftIndex - 1], xListdouble[leftIndex] }) : MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[leftIndex], yListdouble[leftIndex + 1] }, new List<double> { xListdouble[leftIndex], xListdouble[leftIndex + 1] });
                var interpolationfb = rightIndex == yListdouble.Count - 1 ? MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex - 1], yListdouble[rightIndex] }, new List<double> { xListdouble[rightIndex - 1], xListdouble[rightIndex] }) : MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex], yListdouble[rightIndex + 1] }, new List<double> { xListdouble[rightIndex], xListdouble[rightIndex + 1] });
                bandfnList.Add(interpolationfn.Interpolate(yListdouble.Max() - 3.0));
                bandfbList.Add(interpolationfb.Interpolate(yListdouble.Max() - 3.0));
                if (Double.IsInfinity(interpolationfn.Interpolate(yListdouble.Max() - 3.0)))
                {
                    bandfnList.RemoveAll(x => Double.IsInfinity(x));
                    bandfnList.Add(yListdouble[leftIndex]);
                }
                if (Double.IsInfinity(interpolationfb.Interpolate(yListdouble.Max() - 3.0)))
                {
                    bandfbList.RemoveAll(x => Double.IsInfinity(x));
                    bandfbList.Add(yListdouble[rightIndex]);
                }
                bandainList.Add(Math.Abs(yListdouble.GetRange(indexAIn1, indexAIn2 - indexAIn1).Max() - yListdouble.Max()));
                bandaibList.Add(Math.Abs(yListdouble.GetRange(indexAIb1, indexAIb2 - indexAIb1).Max() - yListdouble.Max()));
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(A0F0List, "A0/F0<sub>(10.2GHz)</sub>", "дБ", 42),
                    GetFullStatisticsFromList(bandfnList, "Fn", "ГГц", 55),
                    GetFullStatisticsFromList(bandfbList, "Fb", "ГГц", 56),
                    GetFullStatisticsFromList(bandainList, "AIn<sub>(0.1Ghz-6.65Ghz)</sub>", "дБ", 60),
                    GetFullStatisticsFromList(bandaibList, "AIb<sub>(13.85Ghz-20Ghz)</sub>", "дБ", 61)
                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF8_VSWR1(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var vswrMaxList = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var leftIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x >= 8.9));
            var rightIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x > 11.5)) - 1;
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                vswrMaxList.Add(yListdouble.GetRange(leftIndex, rightIndex - leftIndex).Max());
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vswrMaxList, "КСВН1 в полосе<sub>(8.9GHz-11.5GHz)</sub>", "", 57)

                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF8_VSWR2(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var vswrMaxList = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var leftIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x >= 8.9));
            var rightIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x > 11.5)) - 1;
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                vswrMaxList.Add(yListdouble.GetRange(leftIndex, rightIndex - leftIndex).Max());
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vswrMaxList, "КСВН2 в полосе<sub>(8.9GHz-11.5GHz)</sub>", "", 58)

                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF9(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var indexA0F0 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.45 - item)).FirstOrDefault());
            var A0F0List = new List<double>();
            var bandfnList = new List<double>();
            var bandfbList = new List<double>();
            var bandaiList = new List<double>();

            var indexAIn1 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.1 - item)).FirstOrDefault());
            var indexAIn2 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.19 - item)).FirstOrDefault());
            var indexAIb1 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.75 - item)).FirstOrDefault());
            var indexAIb2 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(1.4 - item)).FirstOrDefault());
            var bandainList = new List<double>();
            var bandaibList = new List<double>();

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                A0F0List.Add(yListdouble[indexA0F0]);
                var maxIndex = yListdouble.IndexOf(yListdouble.Max());
                var leftsideList = yListdouble.Take(maxIndex + 1).ToList();
                var rightsideList = yListdouble.Skip(maxIndex + 1).ToList();
                var leftIndex = leftsideList.IndexOf(leftsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3.0 - item)).FirstOrDefault());
                var rightIndex = rightsideList.IndexOf(rightsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3.0 - item)).FirstOrDefault()) + maxIndex + 1;
                var interpolationfn = leftIndex == yListdouble.Count - 1 ? MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[leftIndex - 1], yListdouble[leftIndex] }, new List<double> { xListdouble[leftIndex - 1], xListdouble[leftIndex] }) : MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[leftIndex], yListdouble[leftIndex + 1] }, new List<double> { xListdouble[leftIndex], xListdouble[leftIndex + 1] });
                var interpolationfb = rightIndex == yListdouble.Count - 1 ? MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex - 1], yListdouble[rightIndex] }, new List<double> { xListdouble[rightIndex - 1], xListdouble[rightIndex] }) : MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex], yListdouble[rightIndex + 1] }, new List<double> { xListdouble[rightIndex], xListdouble[rightIndex + 1] });
                bandfnList.Add(interpolationfn.Interpolate(yListdouble.Max() - 3.0));
                bandfbList.Add(interpolationfb.Interpolate(yListdouble.Max() - 3.0));
                if (Double.IsInfinity(interpolationfn.Interpolate(yListdouble.Max() - 3.0)))
                {
                    bandfnList.RemoveAll(x => Double.IsInfinity(x));
                    bandfnList.Add(yListdouble[leftIndex]);
                }
                if (Double.IsInfinity(interpolationfb.Interpolate(yListdouble.Max() - 3.0)))
                {
                    bandfbList.RemoveAll(x => Double.IsInfinity(x));
                    bandfbList.Add(yListdouble[rightIndex]);
                }
                bandainList.Add(Math.Abs(yListdouble.GetRange(indexAIn1, indexAIn2 - indexAIn1).Max() - yListdouble.Max()));
                bandaibList.Add(Math.Abs(yListdouble.GetRange(indexAIb1, indexAIb2 - indexAIb1).Max() - yListdouble.Max()));
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(A0F0List, "A0/F0<sub>(0.45GHz)</sub>", "дБ", 42),
                    GetFullStatisticsFromList(bandfnList, "Fn", "ГГц", 55),
                    GetFullStatisticsFromList(bandfbList, "Fb", "ГГц", 56),
                    GetFullStatisticsFromList(bandainList, "AIn<sub>(0.1Ghz-0.19Ghz)</sub>", "дБ", 60),
                    GetFullStatisticsFromList(bandaibList, "AIb<sub>(0.75Ghz-2Ghz)</sub>", "дБ", 61)
                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF9_VSWR1(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var vswrMaxList = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var leftIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x >= 0.37));
            var rightIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x > 0.53)) - 1;
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                vswrMaxList.Add(yListdouble.GetRange(leftIndex, rightIndex - leftIndex).Max());
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vswrMaxList, "КСВН1 в полосе<sub>(0.37GHz-0.53GHz)</sub>", "", 57)

                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF9_VSWR2(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var vswrMaxList = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var leftIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x >= 0.37));
            var rightIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x > 0.53)) - 1;
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                vswrMaxList.Add(yListdouble.GetRange(leftIndex, rightIndex - leftIndex).Max());
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vswrMaxList, "КСВН2 в полосе<sub>(0.37GHz-0.53GHz)</sub>", "", 58)

                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF10(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var indexA0F0 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.675 - item)).FirstOrDefault());
            var A0F0List = new List<double>();
            var bandfnList = new List<double>();
            var bandfbList = new List<double>();
            var bandaiList = new List<double>();

            var indexAIn1 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.1 - item)).FirstOrDefault());
            var indexAIn2 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.37 - item)).FirstOrDefault());
            var indexAIb1 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(1.25 - item)).FirstOrDefault());
            var indexAIb2 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2 - item)).FirstOrDefault());
            var bandainList = new List<double>();
            var bandaibList = new List<double>();

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                A0F0List.Add(yListdouble[indexA0F0]);
                var maxIndex = yListdouble.IndexOf(yListdouble.Max());
                var leftsideList = yListdouble.Take(maxIndex + 1).ToList();
                var rightsideList = yListdouble.Skip(maxIndex + 1).ToList();
                var leftIndex = leftsideList.IndexOf(leftsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3.0 - item)).FirstOrDefault());
                var rightIndex = rightsideList.IndexOf(rightsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3.0 - item)).FirstOrDefault()) + maxIndex + 1;
                var interpolationfn = leftIndex == yListdouble.Count - 1 ? MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[leftIndex - 1], yListdouble[leftIndex] }, new List<double> { xListdouble[leftIndex - 1], xListdouble[leftIndex] }) : MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[leftIndex], yListdouble[leftIndex + 1] }, new List<double> { xListdouble[leftIndex], xListdouble[leftIndex + 1] });
                var interpolationfb = rightIndex == yListdouble.Count - 1 ? MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex - 1], yListdouble[rightIndex] }, new List<double> { xListdouble[rightIndex - 1], xListdouble[rightIndex] }) : MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex], yListdouble[rightIndex + 1] }, new List<double> { xListdouble[rightIndex], xListdouble[rightIndex + 1] });
                bandfnList.Add(interpolationfn.Interpolate(yListdouble.Max() - 3.0));
                bandfbList.Add(interpolationfb.Interpolate(yListdouble.Max() - 3.0));
                if (Double.IsInfinity(interpolationfn.Interpolate(yListdouble.Max() - 3.0)))
                {
                    bandfnList.RemoveAll(x => Double.IsInfinity(x));
                    bandfnList.Add(yListdouble[leftIndex]);
                }
                if (Double.IsInfinity(interpolationfb.Interpolate(yListdouble.Max() - 3.0)))
                {
                    bandfbList.RemoveAll(x => Double.IsInfinity(x));
                    bandfbList.Add(yListdouble[rightIndex]);
                }
                bandainList.Add(Math.Abs(yListdouble.GetRange(indexAIn1, indexAIn2 - indexAIn1).Max() - yListdouble.Max()));
                bandaibList.Add(Math.Abs(yListdouble.GetRange(indexAIb1, indexAIb2 - indexAIb1).Max() - yListdouble.Max()));
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(A0F0List, "A0/F0<sub>(0.675GHz)</sub>", "дБ", 42),
                    GetFullStatisticsFromList(bandfnList, "Fn", "ГГц", 55),
                    GetFullStatisticsFromList(bandfbList, "Fb", "ГГц", 56),
                    GetFullStatisticsFromList(bandainList, "AIn<sub>(0.1Ghz-0.37Ghz)</sub>", "дБ", 60),
                    GetFullStatisticsFromList(bandaibList, "AIb<sub>(1.25Ghz-2Ghz)</sub>", "дБ", 61)
                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF10_VSWR1(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var vswrMaxList = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var leftIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x >= 0.6));
            var rightIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x > 0.75)) - 1;
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                vswrMaxList.Add(yListdouble.GetRange(leftIndex, rightIndex - leftIndex).Max());
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vswrMaxList, "КСВН1 в полосе<sub>(0.6GHz-0.75GHz)</sub>", "", 55)

                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF10_VSWR2(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var vswrMaxList = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var leftIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x >= 0.6));
            var rightIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x > 0.75)) - 1;
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                vswrMaxList.Add(yListdouble.GetRange(leftIndex, rightIndex - leftIndex).Max());
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vswrMaxList, "КСВН2 в полосе<sub>(0.6GHz-0.75GHz)</sub>", "", 56)

                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF11(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var indexA0F0 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(1.85 - item)).FirstOrDefault());
            var A0F0List = new List<double>();
            var bandfbList = new List<double>();
            var bandaiList = new List<double>();

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                A0F0List.Add(yListdouble[indexA0F0]);
                var maxIndex = yListdouble.IndexOf(yListdouble.Max());
                var rightsideList = yListdouble.Skip(maxIndex + 1).ToList();
                var rightIndex = rightsideList.IndexOf(rightsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3.0 - item)).FirstOrDefault()) + maxIndex + 1;
                var interpolationfb = rightIndex == yListdouble.Count - 1 ? MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex - 1], yListdouble[rightIndex] }, new List<double> { xListdouble[rightIndex - 1], xListdouble[rightIndex] }) : MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex], yListdouble[rightIndex + 1] }, new List<double> { xListdouble[rightIndex], xListdouble[rightIndex + 1] });
                bandfbList.Add(interpolationfb.Interpolate(yListdouble.Max() - 3.0));
                if (Double.IsInfinity(interpolationfb.Interpolate(yListdouble.Max() - 3.0)))
                {
                    bandfbList.RemoveAll(x => Double.IsInfinity(x));
                    bandfbList.Add(yListdouble[rightIndex]);
                }
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(A0F0List, "A0/F0<sub>(1.85GHz)</sub>", "дБ", 42),
                    GetFullStatisticsFromList(bandfbList, "Fcp", "ГГц", 56)
                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF11_VSWR1(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var vswrMaxList = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var leftIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x >= 0.1));
            var rightIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x > 3.5)) - 1;
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                vswrMaxList.Add(yListdouble.GetRange(leftIndex, rightIndex - leftIndex).Max());
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vswrMaxList, "КСВН1 в полосе<sub>(0.1GHz-3.5GHz)</sub>", "", 57)

                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF11_VSWR2(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var vswrMaxList = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var leftIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x >= 0.1));
            var rightIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x > 3.5)) - 1;
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                vswrMaxList.Add(yListdouble.GetRange(leftIndex, rightIndex - leftIndex).Max());
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vswrMaxList, "КСВН2 в полосе<sub>(0.1GHz-3.5GHz)</sub>", "", 58)

                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF12(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var indexA0F0 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(7.55 - item)).FirstOrDefault());
            var A0F0List = new List<double>();
            var bandfbList = new List<double>();
            var bandaiList = new List<double>();

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                A0F0List.Add(yListdouble[indexA0F0]);
                var maxIndex = yListdouble.IndexOf(yListdouble.Max());
                var rightsideList = yListdouble.Skip(maxIndex + 1).ToList();
                var rightIndex = rightsideList.IndexOf(rightsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3.0 - item)).FirstOrDefault()) + maxIndex + 1;
                var interpolationfb = rightIndex == yListdouble.Count - 1 ? MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex - 1], yListdouble[rightIndex] }, new List<double> { xListdouble[rightIndex - 1], xListdouble[rightIndex] }) : MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex], yListdouble[rightIndex + 1] }, new List<double> { xListdouble[rightIndex], xListdouble[rightIndex + 1] });
                bandfbList.Add(interpolationfb.Interpolate(yListdouble.Max() - 3.0));
                if (Double.IsInfinity(interpolationfb.Interpolate(yListdouble.Max() - 3.0)))
                {
                    bandfbList.RemoveAll(x => Double.IsInfinity(x));
                    bandfbList.Add(yListdouble[rightIndex]);
                }
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(A0F0List, "A0/F0<sub>(7.55GHz)</sub>", "дБ", 42),
                    GetFullStatisticsFromList(bandfbList, "Fcp", "ГГц", 56)
                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF12_VSWR1(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var vswrMaxList = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var leftIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x >= 0.1));
            var rightIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x > 12.5)) - 1;
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                vswrMaxList.Add(yListdouble.GetRange(leftIndex, rightIndex - leftIndex).Max());
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vswrMaxList, "КСВН1 в полосе<sub>(0.1GHz-12.5GHz)</sub>", "", 57)

                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF12_VSWR2(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var vswrMaxList = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var leftIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x >= 0.1));
            var rightIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x > 12.5)) - 1;
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                vswrMaxList.Add(yListdouble.GetRange(leftIndex, rightIndex - leftIndex).Max());
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vswrMaxList, "КСВН2 в полосе<sub>(0.1GHz-12.5GHz)</sub>", "", 58)

                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF15(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var indexA0F0 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.378 - item)).FirstOrDefault());
            var A0F0List = new List<double>();
            var bandfnList = new List<double>();
            var bandfbList = new List<double>();
            var bandaiList = new List<double>();

            var indexAIn1 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.01 - item)).FirstOrDefault());
            var indexAIn2 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.16 - item)).FirstOrDefault());
            var indexAIb1 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.65 - item)).FirstOrDefault());
            var indexAIb2 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2 - item)).FirstOrDefault());
            var bandainList = new List<double>();
            var bandaibList = new List<double>();

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                A0F0List.Add(yListdouble[indexA0F0]);
                var maxIndex = yListdouble.IndexOf(yListdouble.Max());
                var leftsideList = yListdouble.Take(maxIndex + 1).ToList();
                var rightsideList = yListdouble.Skip(maxIndex + 1).ToList();
                var leftIndex = leftsideList.IndexOf(leftsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3.0 - item)).FirstOrDefault());
                var rightIndex = rightsideList.IndexOf(rightsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3.0 - item)).FirstOrDefault()) + maxIndex + 1;
                var interpolationfn = leftIndex == yListdouble.Count - 1 ? MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[leftIndex - 1], yListdouble[leftIndex] }, new List<double> { xListdouble[leftIndex - 1], xListdouble[leftIndex] }) : MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[leftIndex], yListdouble[leftIndex + 1] }, new List<double> { xListdouble[leftIndex], xListdouble[leftIndex + 1] });
                var interpolationfb = rightIndex == yListdouble.Count - 1 ? MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex - 1], yListdouble[rightIndex] }, new List<double> { xListdouble[rightIndex - 1], xListdouble[rightIndex] }) : MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex], yListdouble[rightIndex + 1] }, new List<double> { xListdouble[rightIndex], xListdouble[rightIndex + 1] });
                bandfnList.Add(interpolationfn.Interpolate(yListdouble.Max() - 3.0));
                bandfbList.Add(interpolationfb.Interpolate(yListdouble.Max() - 3.0));
                if (Double.IsInfinity(interpolationfn.Interpolate(yListdouble.Max() - 3.0)))
                {
                    bandfnList.RemoveAll(x => Double.IsInfinity(x));
                    bandfnList.Add(yListdouble[leftIndex]);
                }
                if (Double.IsInfinity(interpolationfb.Interpolate(yListdouble.Max() - 3.0)))
                {
                    bandfbList.RemoveAll(x => Double.IsInfinity(x));
                    bandfbList.Add(yListdouble[rightIndex]);
                }
                bandainList.Add(Math.Abs(yListdouble.GetRange(indexAIn1, indexAIn2 - indexAIn1).Max() - yListdouble.Max()));
                bandaibList.Add(Math.Abs(yListdouble.GetRange(indexAIb1, indexAIb2 - indexAIb1).Max() - yListdouble.Max()));
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(A0F0List, "A0/F0<sub>(0.378GHz)</sub>", "дБ", 42),
                    GetFullStatisticsFromList(bandfnList, "Fn", "ГГц", 55),
                    GetFullStatisticsFromList(bandfbList, "Fb", "ГГц", 56),
                    GetFullStatisticsFromList(bandainList, "AIn<sub>(0.01Ghz-0.16Ghz)</sub>", "дБ", 60),
                    GetFullStatisticsFromList(bandaibList, "AIb<sub>(0.65Ghz-2Ghz)</sub>", "дБ", 61)
                };
            return returnList;
        }



        private List<Statistics> GetFilterCKBA_PF15_VSWR1(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var vswrMaxList = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var leftIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x >= 0.6));
            var rightIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x > 0.75)) - 1;
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                vswrMaxList.Add(yListdouble.GetRange(leftIndex, rightIndex - leftIndex).Max());
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vswrMaxList, "КСВН1 в полосе<sub>(0.6GHz-0.75GHz)</sub>", "", 55)

                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF15_VSWR2(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var vswrMaxList = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var leftIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x >= 0.6));
            var rightIndex = xListdouble.IndexOf(xListdouble.FirstOrDefault(x => x > 0.75)) - 1;
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                vswrMaxList.Add(yListdouble.GetRange(leftIndex, rightIndex - leftIndex).Max());
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vswrMaxList, "КСВН2 в полосе<sub>(0.6GHz-0.75GHz)</sub>", "", 56)

                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF16(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var indexA0F0 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.175 - item)).FirstOrDefault());
            var A0F0List = new List<double>();
            var bandfbList = new List<double>();
            var bandaiList = new List<double>();

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                A0F0List.Add(yListdouble[indexA0F0]);
                var maxIndex = yListdouble.IndexOf(yListdouble.Max());
                var rightsideList = yListdouble.Skip(maxIndex + 1).ToList();
                var rightIndex = rightsideList.IndexOf(rightsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3.0 - item)).FirstOrDefault()) + maxIndex + 1;
                var interpolationfb = rightIndex == yListdouble.Count - 1 ? MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex - 1], yListdouble[rightIndex] }, new List<double> { xListdouble[rightIndex - 1], xListdouble[rightIndex] }) : MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex], yListdouble[rightIndex + 1] }, new List<double> { xListdouble[rightIndex], xListdouble[rightIndex + 1] });
                bandfbList.Add(interpolationfb.Interpolate(yListdouble.Max() - 3.0));
                if (Double.IsInfinity(interpolationfb.Interpolate(yListdouble.Max() - 3.0)))
                {
                    bandfbList.RemoveAll(x => Double.IsInfinity(x));
                    bandfbList.Add(yListdouble[rightIndex]);
                }
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(A0F0List, "A0/F0<sub>(0.175GHz)</sub>", "дБ", 42),
                    GetFullStatisticsFromList(bandfbList, "Fcp", "ГГц", 56)
                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF81_PF82(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var indexA0F0 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10.0 - item)).FirstOrDefault());
            var A0F0List = new List<double>();
            var bandfnList = new List<double>();
            var bandfbList = new List<double>();
            var bandaiList = new List<double>();

            var indexAIn1 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.1 - item)).FirstOrDefault());
            var indexAIn2 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(6.65 - item)).FirstOrDefault());
            var indexAIb1 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(13.85 - item)).FirstOrDefault());
            var indexAIb2 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(20 - item)).FirstOrDefault());
            var bandainList = new List<double>();
            var bandaibList = new List<double>();

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                A0F0List.Add(yListdouble[indexA0F0]);
                var maxIndex = yListdouble.IndexOf(yListdouble.Max());
                var leftsideList = yListdouble.Take(maxIndex + 1).ToList();
                var rightsideList = yListdouble.Skip(maxIndex + 1).ToList();
                var leftIndex = leftsideList.IndexOf(leftsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3.0 - item)).FirstOrDefault());
                var rightIndex = rightsideList.IndexOf(rightsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3.0 - item)).FirstOrDefault()) + maxIndex + 1;
                var interpolationfn = leftIndex == yListdouble.Count - 1 ? MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[leftIndex - 1], yListdouble[leftIndex] }, new List<double> { xListdouble[leftIndex - 1], xListdouble[leftIndex] }) : MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[leftIndex], yListdouble[leftIndex + 1] }, new List<double> { xListdouble[leftIndex], xListdouble[leftIndex + 1] });
                var interpolationfb = rightIndex == yListdouble.Count - 1 ? MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex - 1], yListdouble[rightIndex] }, new List<double> { xListdouble[rightIndex - 1], xListdouble[rightIndex] }) : MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex], yListdouble[rightIndex + 1] }, new List<double> { xListdouble[rightIndex], xListdouble[rightIndex + 1] });
                bandfnList.Add(interpolationfn.Interpolate(yListdouble.Max() - 3.0));
                bandfbList.Add(interpolationfb.Interpolate(yListdouble.Max() - 3.0));
                if (Double.IsInfinity(interpolationfn.Interpolate(yListdouble.Max() - 3.0)))
                {
                    bandfnList.RemoveAll(x => Double.IsInfinity(x));
                    bandfnList.Add(yListdouble[leftIndex]);
                }
                if (Double.IsInfinity(interpolationfb.Interpolate(yListdouble.Max() - 3.0)))
                {
                    bandfbList.RemoveAll(x => Double.IsInfinity(x));
                    bandfbList.Add(yListdouble[rightIndex]);
                }
                bandainList.Add(Math.Abs(yListdouble.GetRange(indexAIn1, indexAIn2 - indexAIn1).Max() - yListdouble.Max()));
                bandaibList.Add(Math.Abs(yListdouble.GetRange(indexAIb1, indexAIb2 - indexAIb1).Max() - yListdouble.Max()));
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(A0F0List, "A0/F0<sub>(10GHz)</sub>", "дБ", 42),
                    GetFullStatisticsFromList(bandfnList, "Fn", "ГГц", 55),
                    GetFullStatisticsFromList(bandfbList, "Fb", "ГГц", 56),
                    GetFullStatisticsFromList(bandainList, "AIn<sub>(0.1Ghz-6.65Ghz)</sub>", "дБ", 60),
                    GetFullStatisticsFromList(bandaibList, "AIb<sub>(13.85Ghz-20Ghz)</sub>", "дБ", 61)
                };
            return returnList;
        }

        private List<Statistics> GetFilterCKBA_PF83(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var indexA0F0 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(13.0 - item)).FirstOrDefault());
            var A0F0List = new List<double>();
            var bandfbList = new List<double>();
            var bandaiList = new List<double>();

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                A0F0List.Add(yListdouble[indexA0F0]);
                var maxIndex = yListdouble.IndexOf(yListdouble.Max());
                var rightsideList = yListdouble.Take(maxIndex + 1).ToList();
                var rightIndex = rightsideList.IndexOf(rightsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3.0 - item)).FirstOrDefault());
                var interpolationfb = rightIndex == yListdouble.Count - 1 ? MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex - 1], yListdouble[rightIndex] }, new List<double> { xListdouble[rightIndex - 1], xListdouble[rightIndex] }) : MathNet.Numerics.Interpolate.Linear(new List<double> { yListdouble[rightIndex], yListdouble[rightIndex + 1] }, new List<double> { xListdouble[rightIndex], xListdouble[rightIndex + 1] });
                bandfbList.Add(interpolationfb.Interpolate(yListdouble.Max() - 3.0));
                if (Double.IsInfinity(interpolationfb.Interpolate(yListdouble.Max() - 3.0)))
                {
                    bandfbList.RemoveAll(x => Double.IsInfinity(x));
                    bandfbList.Add(yListdouble[rightIndex]);
                }
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(A0F0List, "A0/F0<sub>(13GHz)</sub>", "дБ", 42),
                    GetFullStatisticsFromList(bandfbList, "Fcp", "ГГц", 56)
                };
            return returnList;
        }


        private List<Statistics> GetFilterMarkers15(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var onehalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(1.5 - item)).FirstOrDefault());
            var id15List = new List<double>();
            var bandfnList = new List<double>();
            var bandfbList = new List<double>();
            var bandfdleftList = new List<double>();
            var bandfdrightList = new List<double>();


            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id15List.Add(yListdouble[onehalfIndex]);
                var maxIndex = yListdouble.IndexOf(yListdouble.Max());
                var leftsideList = yListdouble.Take(maxIndex + 1).ToList();
                var rightsideList = yListdouble.Skip(maxIndex + 1).ToList();
                var leftIndex =
                    leftsideList.IndexOf(
                        leftsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3 - item)).FirstOrDefault());
                var rightIndex =
                    rightsideList.IndexOf(
                        rightsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3 - item)).FirstOrDefault()) +
                    maxIndex + 1;
                bandfnList.Add(xListdouble[leftIndex]);
                bandfbList.Add(xListdouble[rightIndex]);
                bandfdleftList.Add(Math.Abs(yListdouble[xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(xListdouble[leftIndex] - 0.5 - item)).FirstOrDefault())]));
                bandfdrightList.Add(Math.Abs(yListdouble[xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(xListdouble[rightIndex] + 0.5 - item)).FirstOrDefault())]));


            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id15List, "Потери на 1.5ГГц", "дБ", 51),
                    GetFullStatisticsFromList(bandfnList, "Fn", "ГГц", 53),
                    GetFullStatisticsFromList(bandfbList, "Fb", "ГГц", 54),
                    GetFullStatisticsFromList(bandfdleftList, "FD_left", "ГГц", 55),
                    GetFullStatisticsFromList(bandfdrightList, "FD_right", "ГГц", 56)

                };
            return returnList;
        }


        private List<Statistics> GetFilterMarkers2000_1000(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var onehalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.075 - item)).FirstOrDefault());
            var id15List = new List<double>();
            var bandfnList = new List<double>();
            var bandfbList = new List<double>();
            var bandfdleftList = new List<double>();
            var bandfdrightList = new List<double>();


            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id15List.Add(yListdouble[onehalfIndex]);
                var maxIndex = yListdouble.IndexOf(yListdouble.Max());
                var leftsideList = yListdouble.Take(maxIndex + 1).ToList();
                var rightsideList = yListdouble.Skip(maxIndex + 1).ToList();
                var leftIndex =
                    leftsideList.IndexOf(
                        leftsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3 - item)).FirstOrDefault());
                var rightIndex =
                    rightsideList.IndexOf(
                        rightsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3 - item)).FirstOrDefault()) +
                    maxIndex + 1;
                bandfnList.Add(xListdouble[leftIndex]);
                bandfbList.Add(xListdouble[rightIndex]);
                bandfdleftList.Add(Math.Abs(yListdouble[xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(Math.Abs(xListdouble[leftIndex] - 0.5 - item))).FirstOrDefault())]));
                bandfdrightList.Add(Math.Abs(yListdouble[xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(Math.Abs(xListdouble[rightIndex] + 0.5 - item))).FirstOrDefault())]));


            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id15List, "Потери на 2ГГц", "дБ", 52),
                    GetFullStatisticsFromList(bandfnList, "Fn", "ГГц", 53),
                    GetFullStatisticsFromList(bandfbList, "Fb", "ГГц", 54),
                    GetFullStatisticsFromList(bandfdleftList, "FD_left", "ГГц", 55),
                    GetFullStatisticsFromList(bandfdrightList, "FD_right", "ГГц", 56)

                };
            return returnList;
        }


        private List<Statistics> GetFilterMarkers2000_900(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var onehalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.075 - item)).FirstOrDefault());
            var id15List = new List<double>();
            var bandfnList = new List<double>();
            var bandfbList = new List<double>();
            var bandfdleftList = new List<double>();
            var bandfdrightList = new List<double>();


            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id15List.Add(yListdouble[onehalfIndex]);
                var maxIndex = yListdouble.IndexOf(yListdouble.Max());
                var leftsideList = yListdouble.Take(maxIndex + 1).ToList();
                var rightsideList = yListdouble.Skip(maxIndex + 1).ToList();
                var leftIndex =
                    leftsideList.IndexOf(
                        leftsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3 - item)).FirstOrDefault());
                var rightIndex =
                    rightsideList.IndexOf(
                        rightsideList.OrderBy(item => Math.Abs(yListdouble.Max() - 3 - item)).FirstOrDefault()) +
                    maxIndex + 1;
                bandfnList.Add(xListdouble[leftIndex]);
                bandfbList.Add(xListdouble[rightIndex]);
                bandfdleftList.Add(Math.Abs(yListdouble[xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(Math.Abs(xListdouble[leftIndex] - 0.5 - item))).FirstOrDefault())]));
                bandfdrightList.Add(Math.Abs(yListdouble[xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(Math.Abs(xListdouble[rightIndex] + 0.5 - item))).FirstOrDefault())]));


            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id15List, "Потери на 2ГГц", "дБ", 52),
                    GetFullStatisticsFromList(bandfnList, "Fn", "ГГц", 53),
                    GetFullStatisticsFromList(bandfbList, "Fb", "ГГц", 54),
                    GetFullStatisticsFromList(bandfdleftList, "FD_left", "ГГц", 55),
                    GetFullStatisticsFromList(bandfdrightList, "FD_right", "ГГц", 56)

                };
            return returnList;
        }

        private List<Statistics> GetFilterMarkers20S21S22(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var onehalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.075 - item)).FirstOrDefault());

            var id15List = new List<double>();



            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id15List.Add(yListdouble[onehalfIndex]);

            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id15List, "Потери на 1.5ГГц", "дБ")

                };
            return returnList;
        }

        private List<Statistics> GetFilterMarkers15S21S22(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var onehalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(1.5 - item)).FirstOrDefault());

            var id15List = new List<double>();



            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id15List.Add(yListdouble[onehalfIndex]);

            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id15List, "Потери на 1.5ГГц", "дБ")

                };
            return returnList;
        }

        private List<Statistics> GetS21ONMarkers(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var twohalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(1 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());
            var eightteenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(20 - item)).FirstOrDefault());
            var id25List = new List<double>();
            var id5List = new List<double>();
            var id10List = new List<double>();
            var id18List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id25List.Add(yListdouble[twohalfIndex]);
                id5List.Add(yListdouble[fiveIndex]);
                id10List.Add(yListdouble[tenIndex]);
                id18List.Add(yListdouble[eightteenIndex]);



            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id25List, "S<sub>21ON(1GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id5List,  "S<sub>21ON(5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id10List, "S<sub>21ON(10GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id18List, "S<sub>21ON(20GHz)</sub>", "дБ"),

                };
            return returnList;
        }

        private List<Statistics> GetS21ONMarkersBURN(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var twohalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(1 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());
            var eightteenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(20 - item)).FirstOrDefault());
            var id25List = new List<double>();
            var id5List = new List<double>();
            var id10List = new List<double>();
            var id18List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id25List.Add(yListdouble[twohalfIndex]);
                id5List.Add(yListdouble[fiveIndex]);
                id10List.Add(yListdouble[tenIndex]);
                id18List.Add(yListdouble[eightteenIndex]);



            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id18List, "S<sub>21ON(20GHz)</sub>", "дБ")
                };
            return returnList;
        }

        private List<Statistics> GetS22OFFMarkers(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var twohalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.5 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());
            var eightteenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(18 - item)).FirstOrDefault());
            var id25List = new List<double>();
            var id5List = new List<double>();
            var id10List = new List<double>();
            var id18List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id25List.Add(yListdouble[twohalfIndex]);
                id5List.Add(yListdouble[fiveIndex]);
                id10List.Add(yListdouble[tenIndex]);
                id18List.Add(yListdouble[eightteenIndex]);



            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id25List, "S<sub>22OFF(2.5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id5List,  "S<sub>22OFF(5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id10List, "S<sub>22OFF(10GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id18List, "S<sub>22OFF(18GHz)</sub>", "дБ"),

                };
            return returnList;
        }
        private List<Statistics> GetS22ONMarkers(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var twohalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.5 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());
            var eightteenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(18 - item)).FirstOrDefault());
            var id25List = new List<double>();
            var id5List = new List<double>();
            var id10List = new List<double>();
            var id18List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id25List.Add(yListdouble[twohalfIndex]);
                id5List.Add(yListdouble[fiveIndex]);
                id10List.Add(yListdouble[tenIndex]);
                id18List.Add(yListdouble[eightteenIndex]);



            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id25List, "S<sub>22ON(2.5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id5List,  "S<sub>22ON(5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id10List, "S<sub>22ON(10GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id18List, "S<sub>22ON(18GHz)</sub>", "дБ"),

                };
            return returnList;
        }

        private List<Statistics> GetS11OFFMarkers(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var twohalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.5 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());
            var eightteenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(18 - item)).FirstOrDefault());
            var id25List = new List<double>();
            var id5List = new List<double>();
            var id10List = new List<double>();
            var id18List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id25List.Add(yListdouble[twohalfIndex]);
                id5List.Add(yListdouble[fiveIndex]);
                id10List.Add(yListdouble[tenIndex]);
                id18List.Add(yListdouble[eightteenIndex]);



            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id25List, "S<sub>11OFF(2.5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id5List,  "S<sub>11OFF(5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id10List, "S<sub>11OFF(10GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id18List, "S<sub>11OFF(18GHz)</sub>", "дБ"),

                };
            return returnList;
        }
        private List<Statistics> GetS11ONMarkers(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var twohalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.5 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());
            var eightteenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(18 - item)).FirstOrDefault());
            var id25List = new List<double>();
            var id5List = new List<double>();
            var id10List = new List<double>();
            var id18List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id25List.Add(yListdouble[twohalfIndex]);
                id5List.Add(yListdouble[fiveIndex]);
                id10List.Add(yListdouble[tenIndex]);
                id18List.Add(yListdouble[eightteenIndex]);



            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id25List, "S<sub>11ON(2.5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id5List,  "S<sub>11ON(5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id10List, "S<sub>11ON(10GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id18List, "S<sub>11ON(18GHz)</sub>", "дБ"),

                };
            return returnList;
        }



        private List<Statistics> GetS21BURN(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {

            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var threeIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(3 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());
            var twentyIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(20 - item)).FirstOrDefault());

            List<double> id3List = new List<double>();
            List<double> id5List = new List<double>();
            List<double> id10List = new List<double>();
            List<double> id20List = new List<double>();

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id3List.Add(yListdouble[threeIndex]);
                id5List.Add(yListdouble[fiveIndex]);
                id10List.Add(yListdouble[tenIndex]);
                id20List.Add(yListdouble[twentyIndex]);
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id3List, "S21<sub>(3GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id5List,  "S21<sub>(5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id10List, "S21<sub>(10GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id20List, "S21<sub>(20GHz)</sub>", "дБ")

                };
            return returnList;
        }
        private List<Statistics> GetS21BURN_PASSIVE(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {

            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());
            var twIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(20 - item)).FirstOrDefault());


            List<double> id10List = new List<double>();
            List<double> id20List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id20List.Add(yListdouble[twIndex]);
                id10List.Add(yListdouble[tenIndex]);
            }

            var returnList = new List<Statistics>
                {
                   GetFullStatisticsFromList(id10List, "S21<sub>(10GHz)</sub>", "дБ"),
                   GetFullStatisticsFromList(id10List, "S21<sub>(20GHz)</sub>", "дБ")

                };
            return returnList;
        }
        private List<Statistics> GetS11BURN(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {

            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var threeIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(3 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());

            List<double> id3List = new List<double>();
            List<double> id5List = new List<double>();
            List<double> id10List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id3List.Add(yListdouble[threeIndex]);
                id5List.Add(yListdouble[fiveIndex]);
                id10List.Add(yListdouble[tenIndex]);
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id3List, "S11<sub>(3GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id5List,  "S11<sub>(5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id10List, "S11<sub>(10GHz)</sub>", "дБ")

                };
            return returnList;
        }

        private List<Statistics> GetS11BURN_PASSIVE(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {

            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());
            var twIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(20 - item)).FirstOrDefault());


            List<double> id10List = new List<double>();
            List<double> id20List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id20List.Add(yListdouble[twIndex]);
                id10List.Add(yListdouble[tenIndex]);
            }

            var returnList = new List<Statistics>
                {
                   GetFullStatisticsFromList(id10List, "S11<sub>(10GHz)</sub>", "дБ"),
                   GetFullStatisticsFromList(id10List, "S11<sub>(20GHz)</sub>", "дБ")

                };
            return returnList;
        }

        private List<Statistics> GetS22BURN(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {

            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var threeIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(3 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());

            List<double> id3List = new List<double>();
            List<double> id5List = new List<double>();
            List<double> id10List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id3List.Add(yListdouble[threeIndex]);
                id5List.Add(yListdouble[fiveIndex]);
                id10List.Add(yListdouble[tenIndex]);
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id3List, "S22<sub>(3GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id5List,  "S22<sub>(5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id10List, "S22<sub>(10GHz)</sub>", "дБ")

                };
            return returnList;
        }

        private List<Statistics> GetS22BURN_PASSIVE(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {

            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());
            var twIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(20 - item)).FirstOrDefault());


            List<double> id10List = new List<double>();
            List<double> id20List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id20List.Add(yListdouble[twIndex]);
                id10List.Add(yListdouble[tenIndex]);
            }

            var returnList = new List<Statistics>
                {
                   GetFullStatisticsFromList(id10List, "S22<sub>(10GHz)</sub>", "дБ"),
                   GetFullStatisticsFromList(id10List, "S22<sub>(20GHz)</sub>", "дБ")

                };
            return returnList;
        }

        private List<Statistics> GetS12BURN(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {

            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var threeIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(3 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());

            List<double> id3List = new List<double>();
            List<double> id5List = new List<double>();
            List<double> id10List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id3List.Add(yListdouble[threeIndex]);
                id5List.Add(yListdouble[fiveIndex]);
                id10List.Add(yListdouble[tenIndex]);
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id3List, "S12<sub>(3GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id5List,  "S12<sub>(5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id10List, "S12<sub>(10GHz)</sub>", "дБ")

                };
            return returnList;
        }

        private List<Statistics> GetS12BURN_PASSIVE(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {

            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());
            var twIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(20 - item)).FirstOrDefault());


            List<double> id10List = new List<double>();
            List<double> id20List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id20List.Add(yListdouble[twIndex]);
                id10List.Add(yListdouble[tenIndex]);
            }

            var returnList = new List<Statistics>
                {
                   GetFullStatisticsFromList(id10List, "S12<sub>(10GHz)</sub>", "дБ"),
                   GetFullStatisticsFromList(id10List, "S12<sub>(20GHz)</sub>", "дБ")

                };
            return returnList;
        }


        private List<Statistics> GetS21X5(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var s21List = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());


            List<double> id5List = new List<double>();
            List<double> id10List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                s21List.Add(yListdouble[0]);
                id5List.Add(yListdouble[fiveIndex]);
                id10List.Add(yListdouble[tenIndex]);
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(s21List, "S<sub>21(минимальная частота)</sub>", "дБ"),
                    GetFullStatisticsFromList(id5List,  "S21<sub>(5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id10List, "S21<sub>(10GHz)</sub>", "дБ")

                };
            return returnList;
        }

        private List<Statistics> GetS11X5(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var s21List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                s21List.Add(yListdouble[0]);
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(s21List, "S<sub>21(минимальная частота)</sub>", "дБ"),


                };
            return returnList;
        }



        private List<Statistics> GetS22X5(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var s21List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                s21List.Add(yListdouble[0]);
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(s21List, "S<sub>21(минимальная частота)</sub>", "дБ"),


                };
            return returnList;
        }


        private List<Statistics> GetS12X5(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var s21List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                s21List.Add(yListdouble[0]);
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(s21List, "S<sub>21(минимальная частота)</sub>", "дБ"),


                };
            return returnList;
        }


        private List<Statistics> GetIgss025(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var twoIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(-2 - item)).FirstOrDefault());
            var eightIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(-8 - item)).FirstOrDefault());
            var ig2List = new List<double>();
            var ig8List = new List<double>();
            var vbdgList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                ig2List.Add(twoIndex < 0 ? yListdouble[0] : yListdouble[twoIndex]);
                ig8List.Add(eightIndex < 0 ? yListdouble[0] : yListdouble[eightIndex]);
                var indexes = new List<int>();
                for (int index = 0; index < yListdouble.Count; index++)
                {
                    double d = yListdouble[index];
                    if (Math.Abs(d) >= 0.0001)
                    {
                        if (index < xListdouble.Count)
                        {
                            indexes.Add(index);
                        }

                    }
                }
                vbdgList.Add(indexes.Count == 0
                                ? xListdouble[0]
                                : indexes.Select(index => xListdouble[index]).ToList().Max());
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(ig2List, "I<sub>g2V</sub> (ток при Uзи=-2В)", "А"),
                    GetFullStatisticsFromList(ig8List, "I<sub>g8V</sub> (ток при Uзи=-8В)", "А")

                };
            return returnList;
        }

        private List<Statistics> GetCV(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var zeroIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0 - item)).FirstOrDefault());
            var cvList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                cvList.Add(yListdouble[zeroIndex]);
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(cvList, "С (емкость при V=0В)", "Ф"),

                };
            return returnList;
        }

        private List<Statistics> GetVbrdg(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var vbrdgList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList()))
            {
                var hundredList = new List<int>();
                foreach (var d in yListdouble.Where(item => Math.Abs(100E-6 - item) < 5E-6).ToList())
                {
                    hundredList.Add(yListdouble.IndexOf(d));
                }
                var vbrdg = xListdouble[hundredList.OrderBy(x => x).FirstOrDefault()];
                vbrdgList.Add(vbrdg == xListdouble.FirstOrDefault() ? xListdouble.LastOrDefault() : vbrdg);
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vbrdgList, "V<sub>(br)dg</sub> (напряжение при Ig=100мкА)", "В"),

                };
            return returnList;
        }

        private List<Statistics> GetIdVd_Progress(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var ocIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.15 - item)).FirstOrDefault());
            var oneIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(1.0 - item)).FirstOrDefault());
            var twoIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.0 - item)).FirstOrDefault());
            var threeIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(3.0 - item)).FirstOrDefault());
            var fourIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(4.0 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5.0 - item)).FirstOrDefault());
            var onehalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(1.5 - item)).FirstOrDefault());

            var id2List = new List<double>();
            var id3List = new List<double>();
            var id4List = new List<double>();
            var id5List = new List<double>();
            var s31List = new List<double>();
            var s51List = new List<double>();
            var sfList = new List<double>();
            var ocList = new List<double>();
            var enumerable = commonYList as IList<List<string>> ?? commonYList.ToList();
            foreach (List<double> yListdouble in enumerable.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                var interpolationmethod = Interpolate.Linear(xListdouble, yListdouble);
                var tempList = xListdouble.Where(x => x >= xListdouble[onehalfIndex]).Select(interpolationmethod.Differentiate).ToList();
                sfList.Add(Math.Abs(tempList.Max() - tempList.Min()) * 1000);

                s31List.Add(100 - (yListdouble[onehalfIndex] / yListdouble[threeIndex]) * 100);
                s51List.Add(100 - (yListdouble[onehalfIndex] / yListdouble[fiveIndex]) * 100);



            }

            foreach (List<double> yListdouble in enumerable.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList()))
            {
                ocList.Add((xListdouble[ocIndex] / yListdouble[ocIndex] - 1.1) * divider);
                id2List.Add(yListdouble[twoIndex] * 1.11 / divider);
                id3List.Add(yListdouble[threeIndex] * 1.11 / divider);
                id5List.Add(yListdouble[fiveIndex] * 1.11 / divider);
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id2List, "I<sub>dss(2V)</sub> (ток при Uси=2В)", "А", 28, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(id3List, "I<sub>dss(3V)</sub> (ток при Uси=3В)", "А", 29, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(id5List, "I<sub>dss(5V)</sub> (ток при Uси=5В)", "А", 30, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(ocList, "R<sub>ds(on)</sub> (сопротивление открытого канала)", "Ом", 12, DividerProfile.ROnFamily),
                    GetFullStatisticsFromList(s31List, "S<sub>3-1.5</sub> (критерий S-образности)", "%"),
                    GetFullStatisticsFromList(s51List, "S<sub>5-1.5</sub> (критерий S-образности)", "%"),
                    GetFullStatisticsFromList(sfList,  "S<sub>f</sub> (критерий S-образности)", "мСм")
                };
            return returnList;
        }


        private List<Statistics> GetIdVd(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var ocIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.05 - item)).FirstOrDefault());
            var oneIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(1.0 - item)).FirstOrDefault());
            var twoIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.0 - item)).FirstOrDefault());
            var threeIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(3.0 - item)).FirstOrDefault());
            var fourIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(4.0 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5.0 - item)).FirstOrDefault());
            var onehalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(1.5 - item)).FirstOrDefault());

            var id2List = new List<double>();
            var id3List = new List<double>();
            var id4List = new List<double>();
            var id5List = new List<double>();
            var s31List = new List<double>();
            var s51List = new List<double>();
            var sfList = new List<double>();
            var ocList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                var interpolationmethod = Interpolate.Linear(xListdouble, yListdouble);
                var tempList = xListdouble.Where(x => x >= xListdouble[onehalfIndex]).Select(interpolationmethod.Differentiate).ToList();
                sfList.Add(Math.Abs(tempList.Max() - tempList.Min()) * 1000);
                id2List.Add(yListdouble[twoIndex]);
                id3List.Add(yListdouble[threeIndex]);
                id4List.Add(yListdouble[fourIndex]);
                id5List.Add(yListdouble[fiveIndex]);
                s31List.Add(100 - (yListdouble[onehalfIndex] / yListdouble[threeIndex]) * 100);
                s51List.Add(100 - (yListdouble[onehalfIndex] / yListdouble[fiveIndex]) * 100);
                ocList.Add(xListdouble[ocIndex] / yListdouble[ocIndex]);


            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id2List, "I<sub>dss(2V)</sub> (ток при Uси=2В)", "А", 28, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(id3List, "I<sub>dss(3V)</sub> (ток при Uси=3В)", "А", 29, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(id5List, "I<sub>dss(5V)</sub> (ток при Uси=5В)", "А", 30, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(ocList, "R<sub>ds(on)</sub> (сопротивление открытого канала)", "Ом", 12, DividerProfile.ROnFamily),
                    GetFullStatisticsFromList(s31List, "S<sub>3-1.5</sub> (критерий S-образности)", "%"),
                    GetFullStatisticsFromList(s51List, "S<sub>5-1.5</sub> (критерий S-образности)", "%"),
                    GetFullStatisticsFromList(sfList,  "S<sub>f</sub> (критерий S-образности)", "мСм")
                };
            return returnList;
        }

        private List<Statistics> GetIdVd025(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var ocIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.15 - item)).FirstOrDefault());
            var twoIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.0 - item)).FirstOrDefault());
            var threeIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(3.0 - item)).FirstOrDefault());
            var fourIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(4.0 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5.0 - item)).FirstOrDefault());
            var onehalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(1.5 - item)).FirstOrDefault());


            var id2List = new List<double>();
            var id3List = new List<double>();
            var id4List = new List<double>();
            var id5List = new List<double>();
            var s31List = new List<double>();
            var s51List = new List<double>();
            var sfList = new List<double>();
            var ocList = new List<double>();

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                var interpolationmethod = Interpolate.Linear(xListdouble, yListdouble);
                var tempList = xListdouble.Where(x => x >= xListdouble[onehalfIndex]).Select(interpolationmethod.Differentiate).ToList();
                sfList.Add(Math.Abs(tempList.Max() - tempList.Min()) * 1000);
                id2List.Add(yListdouble[twoIndex]);
                id3List.Add(yListdouble[threeIndex]);
                id4List.Add(yListdouble[fourIndex]);
                id5List.Add(yListdouble[fiveIndex]);
                s31List.Add(100 - (yListdouble[onehalfIndex] / yListdouble[threeIndex]) * 100);
                s51List.Add(100 - (yListdouble[onehalfIndex] / yListdouble[fiveIndex]) * 100);
                ocList.Add(xListdouble[ocIndex]/yListdouble[ocIndex]);

            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id2List, "I<sub>dss(2V)</sub> (ток при Uси=2В)", "А", 28, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(id3List, "I<sub>dss(3V)</sub> (ток при Uси=3В)", "А", 29, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(id5List, "I<sub>dss(5V)</sub> (ток при Uси=5В)", "А", 30, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(ocList, "R<sub>ds(on)</sub> (сопротивление открытого канала)", "Ом", 12, DividerProfile.ROnFamily),
                    GetFullStatisticsFromList(s31List, "S<sub>3-1.5</sub> (критерий S-образности)", "%"),
                    GetFullStatisticsFromList(s51List, "S<sub>5-1.5</sub> (критерий S-образности)", "%"),
                    GetFullStatisticsFromList(sfList, "S<sub>f</sub> (критерий S-образности)", "мСм")
                };
            return returnList;
        }



        private List<Statistics> GetS21Markers025(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var twohalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.5 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());
            var eightteenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(18 - item)).FirstOrDefault());
            var id25List = new List<double>();
            var id5List = new List<double>();
            var id10List = new List<double>();
            var id18List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id25List.Add(yListdouble[twohalfIndex]);
                id5List.Add(yListdouble[fiveIndex]);
                id10List.Add(yListdouble[tenIndex]);
                id18List.Add(yListdouble[eightteenIndex]);
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id25List, "S21<sub>(2.5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id5List,  "S21<sub>(5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id10List, "S21<sub>(10GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id18List, "S21<sub>(18GHz)</sub>", "дБ"),

                };
            return returnList;
        }



        private List<Statistics> GetS12MarkersORION002(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var twohalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(3 - item)).FirstOrDefault());

            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(4 - item)).FirstOrDefault());
            var eightteenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(6 - item)).FirstOrDefault());
            var id25List = new List<double>();

            var id10List = new List<double>();
            var id18List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id25List.Add(yListdouble[twohalfIndex]);

                id10List.Add(yListdouble[tenIndex]);
                id18List.Add(yListdouble[eightteenIndex]);



            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id25List, "S12<sub>(3GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id10List, "S12<sub>(4GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id18List, "S12<sub>(6GHz)</sub>", "дБ"),

                };
            return returnList;
        }

        private List<Statistics> GetS22MarkersORION002(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var twohalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(3 - item)).FirstOrDefault());

            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(4 - item)).FirstOrDefault());
            var eightteenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(6 - item)).FirstOrDefault());
            var id25List = new List<double>();

            var id10List = new List<double>();
            var id18List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id25List.Add(yListdouble[twohalfIndex]);

                id10List.Add(yListdouble[tenIndex]);
                id18List.Add(yListdouble[eightteenIndex]);



            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id25List, "S22<sub>(3GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id10List, "S22<sub>(4GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id18List, "S22<sub>(6GHz)</sub>", "дБ"),

                };
            return returnList;
        }

        private List<Statistics> GetS11MarkersORION002(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var twohalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(3 - item)).FirstOrDefault());

            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(4 - item)).FirstOrDefault());
            var eightteenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(6 - item)).FirstOrDefault());
            var id25List = new List<double>();

            var id10List = new List<double>();
            var id18List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id25List.Add(yListdouble[twohalfIndex]);

                id10List.Add(yListdouble[tenIndex]);
                id18List.Add(yListdouble[eightteenIndex]);



            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id25List, "S11<sub>(3GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id10List, "S11<sub>(4GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id18List, "S11<sub>(6GHz)</sub>", "дБ"),

                };
            return returnList;
        }

        private List<Statistics> GetS21MarkersORION002(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var twohalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(3 - item)).FirstOrDefault());

            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(4 - item)).FirstOrDefault());
            var eightteenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(6 - item)).FirstOrDefault());
            var id25List = new List<double>();

            var id10List = new List<double>();
            var id18List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id25List.Add(yListdouble[twohalfIndex]);

                id10List.Add(yListdouble[tenIndex]);
                id18List.Add(yListdouble[eightteenIndex]);



            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id25List, "S21<sub>(3GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id10List, "S21<sub>(4GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id18List, "S21<sub>(6GHz)</sub>", "дБ"),

                };
            return returnList;
        }


        private List<Statistics> GetS21MarkersORION006(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var twohalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2 - item)).FirstOrDefault());

            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());
            var eightteenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(18 - item)).FirstOrDefault());
            var id25List = new List<double>();

            var id10List = new List<double>();
            var id18List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id25List.Add(yListdouble[twohalfIndex]);

                id10List.Add(yListdouble[tenIndex]);
                id18List.Add(yListdouble[eightteenIndex]);



            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id25List, "S21<sub>(2GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id10List, "S21<sub>(10GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id18List, "S21<sub>(18GHz)</sub>", "дБ"),

                };
            return returnList;
        }





        private List<Statistics> GetS22MarkersORION006(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var twohalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2 - item)).FirstOrDefault());

            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());
            var eightteenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(18 - item)).FirstOrDefault());
            var id25List = new List<double>();

            var id10List = new List<double>();
            var id18List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id25List.Add(yListdouble[twohalfIndex]);

                id10List.Add(yListdouble[tenIndex]);
                id18List.Add(yListdouble[eightteenIndex]);



            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id25List, "S22<sub>(2GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id10List, "S22<sub>(10GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id18List, "S22<sub>(18GHz)</sub>", "дБ"),

                };
            return returnList;
        }

        private List<Statistics> GetS11MarkersORION006(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var twohalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2 - item)).FirstOrDefault());

            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());
            var eightteenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(18 - item)).FirstOrDefault());
            var id25List = new List<double>();

            var id10List = new List<double>();
            var id18List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id25List.Add(yListdouble[twohalfIndex]);

                id10List.Add(yListdouble[tenIndex]);
                id18List.Add(yListdouble[eightteenIndex]);



            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id25List, "S11<sub>(2GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id10List, "S11<sub>(10GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id18List, "S11<sub>(18GHz)</sub>", "дБ"),

                };
            return returnList;
        }

        private List<Statistics> GetS22Markers025(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var twohalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.5 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());
            var eightteenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(18 - item)).FirstOrDefault());
            var id25List = new List<double>();
            var id5List = new List<double>();
            var id10List = new List<double>();
            var id18List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id25List.Add(yListdouble[twohalfIndex]);
                id5List.Add(yListdouble[fiveIndex]);
                id10List.Add(yListdouble[tenIndex]);
                id18List.Add(yListdouble[eightteenIndex]);



            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id25List, "S22<sub>(2.5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id5List,  "S22<sub>(5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id10List, "S22<sub>(10GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id18List, "S22<sub>(18GHz)</sub>", "дБ"),

                };
            return returnList;
        }


        private List<Statistics> GetS12Markers_ORION003(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var index06 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.6 - item)).FirstOrDefault());
            var index225 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.25 - item)).FirstOrDefault());

            var id06List = new List<double>();
            var id225List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {


                id06List.Add(yListdouble[index06]);
                id225List.Add(yListdouble[index225]);



            }

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList( id06List, "S12<sub>(0.6GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( id225List, "S12<sub>(2.25GHz)</sub>", "дБ"),

                };
            return returnList;
        }

        private List<Statistics> GetS22Markers_ORION003(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var index06 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.6 - item)).FirstOrDefault());
            var index225 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.25 - item)).FirstOrDefault());

            var id06List = new List<double>();
            var id225List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {


                id06List.Add(yListdouble[index06]);
                id225List.Add(yListdouble[index225]);



            }

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList( id06List, "S22<sub>(0.6GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( id225List, "S22<sub>(2.25GHz)</sub>", "дБ"),

                };
            return returnList;
        }


        private List<Statistics> GetS21Markers_ORION003(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var index06 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.6 - item)).FirstOrDefault());
            var index225 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.25 - item)).FirstOrDefault());

            var id06List = new List<double>();
            var id225List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {


                id06List.Add(yListdouble[index06]);
                id225List.Add(yListdouble[index225]);



            }

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList( id06List, "S21<sub>(0.6GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( id225List, "S21<sub>(2.25GHz)</sub>", "дБ"),

                };
            return returnList;
        }

        private List<Statistics> GetS21Markers_ORION001(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var index4 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(4.0 - item)).FirstOrDefault());
            var index6 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(6.0 - item)).FirstOrDefault());

            var id4List = new List<double>();
            var id6List = new List<double>();
            var deltaList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {


                id4List.Add(yListdouble[index4]);
                id6List.Add(yListdouble[index6]);
                var y46List = new List<double>();
                for (var i = index4; i < index6 + 1; i++)
                {
                    y46List.Add(yListdouble[i]);
                }
                deltaList.Add(Math.Abs(y46List.Max() - y46List.Min()));



            }

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList( id4List, "S21<sub>(4GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( id6List, "S21<sub>(6GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( deltaList, "S21_delta<sub>(4-6GHz)</sub>", "дБ"),

                };
            return returnList;
        }

        private List<Statistics> GetS21MarkersMP245(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var index4 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(8.5 - item)).FirstOrDefault());
            var index5 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(11.0 - item)).FirstOrDefault());
            var index6 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(12.5 - item)).FirstOrDefault());

            var id4List = new List<double>();
            var id5List = new List<double>();
            var id6List = new List<double>();
            var deltaList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {


                id4List.Add(yListdouble[index4]);
                id5List.Add(yListdouble[index5]);
                id6List.Add(yListdouble[index6]);
                var y46List = new List<double>();
                for (var i = index4; i < index6 + 1; i++)
                {
                    y46List.Add(yListdouble[i]);
                }
                deltaList.Add(Math.Abs(y46List.Max() - y46List.Min()));



            }

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList( id4List, "S21<sub>(8.5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( id5List, "S21<sub>(11GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( id6List, "S21<sub>(12.5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( deltaList, "S21_delta<sub>(12.5-8.5GHz)</sub>", "дБ"),

                };
            return returnList;
        }

        private List<Statistics> GetS12MarkersMP245(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var index4 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(8.5 - item)).FirstOrDefault());
            var index5 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(11.0 - item)).FirstOrDefault());
            var index6 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(12.5 - item)).FirstOrDefault());

            var id4List = new List<double>();
            var id5List = new List<double>();
            var id6List = new List<double>();
            var deltaList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id5List.Add(yListdouble[index5]);
                id4List.Add(yListdouble[index4]);
                id6List.Add(yListdouble[index6]);
                var y46List = new List<double>();
                for (var i = index4; i < index6 + 1; i++)
                {
                    y46List.Add(yListdouble[i]);
                }
                deltaList.Add(Math.Abs(y46List.Max() - y46List.Min()));



            }

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList( id4List, "S11<sub>(8.5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( id5List, "S11<sub>(11GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( id6List, "S11<sub>(12.5GHz)</sub>", "дБ"),

                };
            return returnList;
        }
        private List<Statistics> GetS11MarkersMP245(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var index4 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(8.5 - item)).FirstOrDefault());
            var index5 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(11.0 - item)).FirstOrDefault());
            var index6 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(12.5 - item)).FirstOrDefault());

            var id4List = new List<double>();
            var id5List = new List<double>();
            var id6List = new List<double>();
            var deltaList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {


                id4List.Add(yListdouble[index4]);
                id5List.Add(yListdouble[index5]);
                id6List.Add(yListdouble[index6]);
                var y46List = new List<double>();
                for (var i = index4; i < index6 + 1; i++)
                {
                    y46List.Add(yListdouble[i]);
                }
                deltaList.Add(Math.Abs(y46List.Max() - y46List.Min()));



            }

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList( id4List, "S11<sub>(8.5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( id5List, "S11<sub>(11GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( id6List, "S11<sub>(12.5GHz)</sub>", "дБ"),

                };
            return returnList;
        }

        private List<Statistics> GetS22MarkersMP245(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var index4 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(8.5 - item)).FirstOrDefault());
            var index5 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(11.0 - item)).FirstOrDefault());
            var index6 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(12.5 - item)).FirstOrDefault());
            var id5List = new List<double>();
            var id4List = new List<double>();
            var id6List = new List<double>();
            var deltaList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id5List.Add(yListdouble[index5]);
                id4List.Add(yListdouble[index4]);
                id6List.Add(yListdouble[index6]);
                var y46List = new List<double>();
                for (var i = index4; i < index6 + 1; i++)
                {
                    y46List.Add(yListdouble[i]);
                }
                deltaList.Add(Math.Abs(y46List.Max() - y46List.Min()));



            }

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList( id4List, "S22<sub>(8.5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( id5List, "S22<sub>(11GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( id6List, "S22<sub>(12.5GHz)</sub>", "дБ"),

                };
            return returnList;
        }

        private List<Statistics> GetS21Markers_FLASH_001_002(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var index4 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(4.0 - item)).FirstOrDefault());
            var index6 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(6.0 - item)).FirstOrDefault());

            var id4List = new List<double>();
            var id6List = new List<double>();
            var deltaList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {


                id4List.Add(yListdouble[index4]);
                id6List.Add(yListdouble[index6]);
                var y46List = new List<double>();
                for (var i = index4; i < index6 + 1; i++)
                {
                    y46List.Add(yListdouble[i]);
                }
                deltaList.Add(Math.Abs(y46List.Max() - y46List.Min()));



            }

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList( id4List, "S21<sub>(4GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( id6List, "S21<sub>(6GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( deltaList, "S21_delta<sub>(4-6GHz)</sub>", "дБ"),

                };
            return returnList;
        }

        private List<Statistics> GetS21Markers_FLASH_003_004(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var index8 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(8.0 - item)).FirstOrDefault());
            var index10 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10.0 - item)).FirstOrDefault());
            var index12 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(12.0 - item)).FirstOrDefault());

            var id8List = new List<double>();
            var id10List = new List<double>();
            var id12List = new List<double>();
            var deltaList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {


                id8List.Add(yListdouble[index8]);
                id10List.Add(yListdouble[index10]);
                id12List.Add(yListdouble[index12]);
                var y46List = new List<double>();
                for (var i = index8; i < index12 + 1; i++)
                {
                    y46List.Add(yListdouble[i]);
                }
                deltaList.Add(Math.Abs(y46List.Max() - y46List.Min()));



            }

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList( id8List, "S21<sub>(8GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( id10List, "S21<sub>(10GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( id12List, "S21<sub>(12GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( deltaList, "S21_delta<sub>(8-12GHz)</sub>", "дБ"),

                };
            return returnList;
        }

        private List<Statistics> GetS21Markers_FLASH_005_006(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var index16 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(16.0 - item)).FirstOrDefault());
            var index18 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(18.0 - item)).FirstOrDefault());

            var id16List = new List<double>();
            var id18List = new List<double>();
            var deltaList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {


                id16List.Add(yListdouble[index16]);
                id18List.Add(yListdouble[index18]);
                var y46List = new List<double>();
                for (var i = index16; i < index18 + 1; i++)
                {
                    y46List.Add(yListdouble[i]);
                }
                deltaList.Add(Math.Abs(y46List.Max() - y46List.Min()));



            }

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList( id16List, "S21<sub>(16GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( id18List, "S21<sub>(18GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( deltaList, "S21_delta<sub>(16-18GHz)</sub>", "дБ"),

                };
            return returnList;
        }
        private List<Statistics> GetS11Markers_FLASH_001_002(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var index4 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(4.0 - item)).FirstOrDefault());
            var index6 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(6.0 - item)).FirstOrDefault());

            var id4List = new List<double>();
            var id6List = new List<double>();
            var deltaList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {


                id4List.Add(yListdouble[index4]);
                id6List.Add(yListdouble[index6]);
                var y46List = new List<double>();
                for (var i = index4; i < index6 + 1; i++)
                {
                    y46List.Add(yListdouble[i]);
                }
                deltaList.Add(Math.Abs(y46List.Max() - y46List.Min()));



            }

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList( id4List, "S11<sub>(4GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( id6List, "S11<sub>(6GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( deltaList, "S11_delta<sub>(4-6GHz)</sub>", "дБ"),

                };
            return returnList;
        }

        private List<Statistics> GetS11Markers_FLASH_003_004(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var index8 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(8.0 - item)).FirstOrDefault());
            var index10 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10.0 - item)).FirstOrDefault());
            var index12 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(12.0 - item)).FirstOrDefault());

            var id8List = new List<double>();
            var id10List = new List<double>();
            var id12List = new List<double>();
            var deltaList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {


                id8List.Add(yListdouble[index8]);
                id10List.Add(yListdouble[index10]);
                id12List.Add(yListdouble[index12]);
                var y46List = new List<double>();
                for (var i = index8; i < index12 + 1; i++)
                {
                    y46List.Add(yListdouble[i]);
                }
                deltaList.Add(Math.Abs(y46List.Max() - y46List.Min()));



            }

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList( id8List, "S11<sub>(8GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( id10List, "S11<sub>(10GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( id12List, "S11<sub>(12GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( deltaList, "S11_delta<sub>(8-12GHz)</sub>", "дБ"),

                };
            return returnList;
        }

        private List<Statistics> GetS11Markers_FLASH_005_006(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var index16 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(16.0 - item)).FirstOrDefault());
            var index18 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(18.0 - item)).FirstOrDefault());

            var id16List = new List<double>();
            var id18List = new List<double>();
            var deltaList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {


                id16List.Add(yListdouble[index16]);
                id18List.Add(yListdouble[index18]);
                var y46List = new List<double>();
                for (var i = index16; i < index18 + 1; i++)
                {
                    y46List.Add(yListdouble[i]);
                }
                deltaList.Add(Math.Abs(y46List.Max() - y46List.Min()));



            }

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList( id16List, "S11<sub>(16GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( id18List, "S11<sub>(18GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( deltaList, "S11_delta<sub>(16-18GHz)</sub>", "дБ"),

                };
            return returnList;
        }

        private List<Statistics> GetS22Markers_FLASH_001_002(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var index4 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(4.0 - item)).FirstOrDefault());
            var index6 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(6.0 - item)).FirstOrDefault());

            var id4List = new List<double>();
            var id6List = new List<double>();
            var deltaList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {


                id4List.Add(yListdouble[index4]);
                id6List.Add(yListdouble[index6]);
                var y46List = new List<double>();
                for (var i = index4; i < index6 + 1; i++)
                {
                    y46List.Add(yListdouble[i]);
                }
                deltaList.Add(Math.Abs(y46List.Max() - y46List.Min()));



            }

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList( id4List, "S22<sub>(4GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( id6List, "S22<sub>(6GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( deltaList, "S22_delta<sub>(4-6GHz)</sub>", "дБ"),

                };
            return returnList;
        }

        private List<Statistics> GetS22Markers_FLASH_003_004(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var index8 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(8.0 - item)).FirstOrDefault());
            var index10 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10.0 - item)).FirstOrDefault());
            var index12 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(12.0 - item)).FirstOrDefault());

            var id8List = new List<double>();
            var id10List = new List<double>();
            var id12List = new List<double>();
            var deltaList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {


                id8List.Add(yListdouble[index8]);
                id10List.Add(yListdouble[index10]);
                id12List.Add(yListdouble[index12]);
                var y46List = new List<double>();
                for (var i = index8; i < index12 + 1; i++)
                {
                    y46List.Add(yListdouble[i]);
                }
                deltaList.Add(Math.Abs(y46List.Max() - y46List.Min()));



            }

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList( id8List, "S22<sub>(8GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( id10List, "S22<sub>(10GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( id12List, "S22<sub>(12GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( deltaList, "S22_delta<sub>(8-12GHz)</sub>", "дБ"),

                };
            return returnList;
        }

        private List<Statistics> GetS22Markers_FLASH_005_006(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var index16 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(16.0 - item)).FirstOrDefault());
            var index18 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(18.0 - item)).FirstOrDefault());

            var id16List = new List<double>();
            var id18List = new List<double>();
            var deltaList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {


                id16List.Add(yListdouble[index16]);
                id18List.Add(yListdouble[index18]);
                var y46List = new List<double>();
                for (var i = index16; i < index18 + 1; i++)
                {
                    y46List.Add(yListdouble[i]);
                }
                deltaList.Add(Math.Abs(y46List.Max() - y46List.Min()));



            }

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList( id16List, "S22<sub>(16GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( id18List, "S22<sub>(18GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( deltaList, "S22_delta<sub>(16-18GHz)</sub>", "дБ"),

                };
            return returnList;
        }

        private List<Statistics> GetS12Markers_ORION001(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var index4 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(4.0 - item)).FirstOrDefault());
            var index6 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(6.0 - item)).FirstOrDefault());

            var id4List = new List<double>();
            var id6List = new List<double>();
            var deltaList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {


                id4List.Add(yListdouble[index4]);
                id6List.Add(yListdouble[index6]);




            }

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList( id4List, "S12<sub>(4GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( id6List, "S12<sub>(6GHz)</sub>", "дБ"),


                };
            return returnList;
        }

        private List<Statistics> GetS22Markers_ORION001(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var index4 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(4.0 - item)).FirstOrDefault());
            var index6 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(6.0 - item)).FirstOrDefault());

            var id4List = new List<double>();
            var id6List = new List<double>();
            var deltaList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {


                id4List.Add(yListdouble[index4]);
                id6List.Add(yListdouble[index6]);




            }

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList( id4List, "S22<sub>(4GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( id6List, "S22<sub>(6GHz)</sub>", "дБ"),


                };
            return returnList;
        }

        private List<Statistics> GetS11Markers_ORION001(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var index4 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(4.0 - item)).FirstOrDefault());
            var index6 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(6.0 - item)).FirstOrDefault());

            var id4List = new List<double>();
            var id6List = new List<double>();
            var deltaList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {


                id4List.Add(yListdouble[index4]);
                id6List.Add(yListdouble[index6]);




            }

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList( id4List, "S11<sub>(4GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( id6List, "S11<sub>(6GHz)</sub>", "дБ"),


                };
            return returnList;
        }
        private List<Statistics> GetS11Markers_ORION003(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var index06 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.6 - item)).FirstOrDefault());
            var index225 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.25 - item)).FirstOrDefault());

            var id06List = new List<double>();
            var id225List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {


                id06List.Add(yListdouble[index06]);
                id225List.Add(yListdouble[index225]);



            }

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList( id06List, "S11<sub>(0.6GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList( id225List, "S11<sub>(2.25GHz)</sub>", "дБ"),

                };
            return returnList;
        }

        private List<Statistics> GetS12Markers025(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var twohalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.5 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());
            var eightteenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(18 - item)).FirstOrDefault());
            var id25List = new List<double>();
            var id5List = new List<double>();
            var id10List = new List<double>();
            var id18List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id25List.Add(yListdouble[twohalfIndex]);
                id5List.Add(yListdouble[fiveIndex]);
                id10List.Add(yListdouble[tenIndex]);
                id18List.Add(yListdouble[eightteenIndex]);



            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id25List, "S12<sub>(2.5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id5List,  "S12<sub>(5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id10List, "S12<sub>(10GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id18List, "S12<sub>(18GHz)</sub>", "дБ"),

                };
            return returnList;
        }

        private List<Statistics> GetS11Markers025(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var twohalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.5 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());
            var eightteenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(18 - item)).FirstOrDefault());
            var id25List = new List<double>();
            var id5List = new List<double>();
            var id10List = new List<double>();
            var id18List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id25List.Add(yListdouble[twohalfIndex]);
                id5List.Add(yListdouble[fiveIndex]);
                id10List.Add(yListdouble[tenIndex]);
                id18List.Add(yListdouble[eightteenIndex]);



            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id25List, "S11<sub>(2.5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id5List,  "S11<sub>(5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id10List, "S11<sub>(10GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id18List, "S11<sub>(18GHz)</sub>", "дБ"),

                };
            return returnList;
        }

        private List<Statistics> GetS21Markers(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var twohalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.5 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());
            var eightteenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(18 - item)).FirstOrDefault());
            var id25List = new List<double>();
            var id5List = new List<double>();
            var id10List = new List<double>();
            var id18List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id25List.Add(yListdouble[twohalfIndex]);
                id5List.Add(yListdouble[fiveIndex]);
                id10List.Add(yListdouble[tenIndex]);
                id18List.Add(yListdouble[eightteenIndex]);



            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id25List, "S21<sub>(2.5GHz)</sub>(коэффициент передачи)", "дБ", 18),
                    GetFullStatisticsFromList(id5List,  "S21<sub>(5GHz)</sub>(коэффициент передачи)", "дБ", 19),
                    GetFullStatisticsFromList(id10List, "S21<sub>(10GHz)</sub>(коэффициент передачи)", "дБ", 20),
                    GetFullStatisticsFromList(id18List, "S21<sub>(18GHz)</sub>(коэффициент передачи)", "дБ", 21),

                };
            return returnList;
        }

        private List<Statistics> GetATTMongo(List<double> xListdouble, List<List<double>> commonYList, double divider, string type)
        {

            var twelveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(12 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());
            var eightIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(8 - item)).FirstOrDefault());
            var s21sigmaList = new List<double>();
            var s21at10List = new List<double>();

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => x / divider).ToList()))
            {

                s21sigmaList.Add(Math.Abs(yListdouble.Skip(eightIndex).Take(twelveIndex - eightIndex).Max() - yListdouble.Skip(eightIndex).Take(twelveIndex - eightIndex).Min()));
                s21at10List.Add(yListdouble[tenIndex]);

            }

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(s21at10List, "S21<sub>(10GHz)</sub>$$" + type, "дБ"),
                    GetFullStatisticsFromList(s21sigmaList, "S21<sub>sigma(8GHz-12GHz)</sub>$$" + type, "дБ"),

                };
            return returnList;
        }

        private string GetATTMongoPoint(List<double> xListdouble, List<List<double>> commonYList, string type, Dictionary<string, double> points)
        {

            var index = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(points["SinglePoint"] - item)).FirstOrDefault());
            var s21at10List = new List<double>();

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => x).ToList()))
            {
                s21at10List.Add(yListdouble[index]);

            }

            return GetFullStatisticsFromList(s21at10List, "S21<sub>(10GHz)</sub>", "дБ").ExpectedValue;
        }



        private string GetATTMongoSigma(List<double> xListdouble, List<List<double>> commonYList, string type, Dictionary<string, double> points)
        {

            var eightIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(points["StartRangeGatePoint"] - item)).FirstOrDefault());
            var twelveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(points["EndRangeGatePoint"] - item)).FirstOrDefault());
            var s21sigmaList = new List<double>();

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => x).ToList()))
            {

                s21sigmaList.Add(Math.Abs(yListdouble.Skip(eightIndex).Take(twelveIndex - eightIndex).Max() - yListdouble.Skip(eightIndex).Take(twelveIndex - eightIndex).Min()));

            }

            return GetFullStatisticsFromList(s21sigmaList, "S21<sub>sigma(8GHz-12GHz)</sub>", "дБ").ExpectedValue;
        }

        private string GetATTMongoPointPair(List<double> xListdouble, Dictionary<string, List<List<double>>> commonYList, Dictionary<string,string> type, Dictionary<string, double> points)
        {

            var index = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(points["SinglePoint"] - item)).FirstOrDefault());

            var firstslash = commonYList[type["FirstSlashType"]].Select(yList => yList.Select(x => x).ToList()).Select(yListdouble => yListdouble[index]).ToList();
            var secondslash = commonYList[type["SecondSlashType"]].Select(yList => yList.Select(x => x).ToList()).Select(yListdouble => yListdouble[index]).ToList();

            return GetFullStatisticsFromList(firstslash, "S21<sub>(10GHz)</sub> - type: ", "дБ").ExpectedValue + "/" + GetFullStatisticsFromList(secondslash, "S21<sub>(10GHz)</sub> - type: ", "дБ").ExpectedValue;
        }

        private List<Statistics> GetPSMongo(List<double> xListdouble, List<List<double>> commonYList, double divider, string type)
        {

            var twelveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(12 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());
            var eightIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(8 - item)).FirstOrDefault());
            var s21sigmaList = new List<double>();
            var s21at10List = new List<double>();

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => x / divider).ToList()))
            {

                s21sigmaList.Add(Math.Abs(yListdouble.Skip(eightIndex).Take(twelveIndex - eightIndex).Max() - yListdouble.Skip(eightIndex).Take(twelveIndex - eightIndex).Min()));
                s21at10List.Add(yListdouble[tenIndex]);

            }

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(s21at10List, "S21<sub>(10GHz)</sub> - type: " + type, "дБ"),
                    GetFullStatisticsFromList(s21sigmaList, "S21<sub>sigma(8GHz-12GHz)</sub> - type: " + type, "дБ"),

                };
            return returnList;
        }



        private List<Statistics> GetATTMongoPhase(List<double> xListdouble, List<List<double>> commonYList, double divider, string type)
        {

            var twelveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(12 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());
            var eightIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(8 - item)).FirstOrDefault());
            var s21sigmaList = new List<double>();
            var s21at10List = new List<double>();

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => x / divider).ToList()))
            {

                s21sigmaList.Add(Math.Abs(yListdouble.Skip(eightIndex).Take(twelveIndex - eightIndex).Max() - yListdouble.Skip(eightIndex).Take(twelveIndex - eightIndex).Min()));
                s21at10List.Add(yListdouble[tenIndex]);

            }

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(s21at10List, "Phase_S21<sub>(10GHz)</sub>$$" + type, "дБ"),
                    GetFullStatisticsFromList(s21sigmaList, "Phase_S21<sub>sigma(8GHz-12GHz)</sub>$$" + type, "дБ"),

                };
            return returnList;
        }

        private List<Statistics> GetPSMongoPhase(List<double> xListdouble, List<List<double>> commonYList, double divider, string type)
        {

            var twelveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(12 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());
            var eightIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(8 - item)).FirstOrDefault());
            var s21sigmaList = new List<double>();
            var s21at10List = new List<double>();

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => x / divider).ToList()))
            {

                s21sigmaList.Add(
                    Math.Abs(yListdouble.Skip(eightIndex).Take(twelveIndex - eightIndex).Max() -
                             yListdouble.Skip(eightIndex).Take(twelveIndex - eightIndex).Min()));
                s21at10List.Add(yListdouble[tenIndex]);

            }

            var returnList = new List<Statistics>
            {

                GetFullStatisticsFromList(s21at10List, "Phase_S21<sub>(10GHz)</sub> - type: " + type, "дБ"),
                GetFullStatisticsFromList(s21sigmaList, "Phase_S21<sub>sigma(8GHz-12GHz)</sub> - type: " + type, "дБ"),

            };
            return returnList;
        }

        private List<Statistics> GetATTMongoS21(List<double> xListdouble, List<List<double>> commonYList, double divider, string type)
        {

            var twelveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(12 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());
            var eightIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(8 - item)).FirstOrDefault());

            var s21at8List = new List<double>();
            var s21at10List = new List<double>();
            var s21at12List = new List<double>();

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => x / divider).ToList()))
            {
                s21at8List.Add(yListdouble[eightIndex]);
                s21at10List.Add(yListdouble[tenIndex]);
                s21at12List.Add(yListdouble[twelveIndex]);
            }

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(s21at8List, type + "<sub>(8GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(s21at10List, type + "<sub>(10GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(s21at12List, type + "<sub>(12GHz)</sub>", "дБ")


                };
            return returnList;
        }

        private List<Statistics> GetS21MarkersFilter(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)

        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var twohalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.85 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(1.22 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(1.77 - item)).FirstOrDefault());
            var eightteenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.1 - item)).FirstOrDefault());
            var eightteen2Index = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(3 - item)).FirstOrDefault());
            var eightteen1Index = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(7.1 - item)).FirstOrDefault());
            var a0Index = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(1.7 - item)).FirstOrDefault());
            var a45Index = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(4.5 - item)).FirstOrDefault());
            var a078Index = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.78 - item)).FirstOrDefault());
            var id085List = new List<double>();
            var id122List = new List<double>();
            var id177List = new List<double>();
            var id21List = new List<double>();
            var id3List = new List<double>();
            var id71List = new List<double>();
            var a0List = new List<double>();
            var a45List = new List<double>();
            List<double> a078List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id085List.Add(yListdouble[twohalfIndex]);
                id122List.Add(yListdouble[fiveIndex]);
                id177List.Add(yListdouble[tenIndex]);
                id21List.Add(yListdouble[eightteenIndex]);
                id3List.Add(yListdouble[eightteen2Index]);
                id71List.Add(yListdouble[eightteen1Index]);
                a0List.Add(yListdouble[a0Index]);
                a45List.Add(yListdouble[a45Index]);
                a078List.Add(yListdouble[a078Index]);


            }


            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(a0List, "A0/F0<sub>(1.7GHz)</sub>", "дБ", 42),
                    GetFullStatisticsFromList(a45List, "A4.5ГГц<sub>(4.5GHz)</sub>", "дБ", 43),
                    GetFullStatisticsFromList(a078List, "A0.78ГГц<sub>(0.78GHz)</sub>", "дБ", 44),
                    GetFullStatisticsFromList(id085List, "S21<sub>(0.85GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id122List,  "S21<sub>(1.22GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id177List, "S21<sub>(1.77GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id21List, "S21<sub>(2.1GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id3List, "S21<sub>(3GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id71List, "S21<sub>(7.1GHz)</sub>", "дБ")


                };
            return returnList;
        }

        private List<Statistics> GetS11MarkersFilter(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var twohalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.85 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(1.22 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(1.77 - item)).FirstOrDefault());
            var eightteenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.1 - item)).FirstOrDefault());
            var eightteen2Index = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(3 - item)).FirstOrDefault());
            var eightteen1Index = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(7.1 - item)).FirstOrDefault());
            var id085List = new List<double>();
            var id122List = new List<double>();
            var id177List = new List<double>();
            var id21List = new List<double>();
            var id3List = new List<double>();
            var id71List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id085List.Add(yListdouble[twohalfIndex]);
                id122List.Add(yListdouble[fiveIndex]);
                id177List.Add(yListdouble[tenIndex]);
                id21List.Add(yListdouble[eightteenIndex]);
                id3List.Add(yListdouble[eightteen2Index]);
                id71List.Add(yListdouble[eightteen1Index]);



            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id085List, "S11<sub>(0.85GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id122List,  "S11<sub>(1.22GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id177List, "S11<sub>(1.77GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id21List, "S11<sub>(2.1GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id3List, "S11<sub>(3GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id71List, "S11<sub>(7.1GHz)</sub>", "дБ")


                };
            return returnList;
        }

        private List<Statistics> GetS22MarkersFilter(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var twohalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.85 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(1.22 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(1.77 - item)).FirstOrDefault());
            var eightteenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.1 - item)).FirstOrDefault());
            var eightteen2Index = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(3 - item)).FirstOrDefault());
            var eightteen1Index = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(7.1 - item)).FirstOrDefault());
            var id085List = new List<double>();
            var id122List = new List<double>();
            var id177List = new List<double>();
            var id21List = new List<double>();
            var id3List = new List<double>();
            var id71List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id085List.Add(yListdouble[twohalfIndex]);
                id122List.Add(yListdouble[fiveIndex]);
                id177List.Add(yListdouble[tenIndex]);
                id21List.Add(yListdouble[eightteenIndex]);
                id3List.Add(yListdouble[eightteen2Index]);
                id71List.Add(yListdouble[eightteen1Index]);



            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id085List, "S22<sub>(0.85GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id122List,  "S22<sub>(1.22GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id177List, "S22<sub>(1.77GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id21List, "S22<sub>(2.1GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id3List, "S22<sub>(3GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id71List, "S22<sub>(7.1GHz)</sub>", "дБ")


                };
            return returnList;
        }









        private List<Statistics> GetS11Markers(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var twohalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.5 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());
            var eightteenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(18 - item)).FirstOrDefault());
            var id25List = new List<double>();
            var id5List = new List<double>();
            var id10List = new List<double>();
            var id18List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id25List.Add(yListdouble[twohalfIndex]);
                id5List.Add(yListdouble[fiveIndex]);
                id10List.Add(yListdouble[tenIndex]);
                id18List.Add(yListdouble[eightteenIndex]);



            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id25List, "S<sub>11(2.5GHz)</sub>", "дБ", 22),
                    GetFullStatisticsFromList(id5List,  "S<sub>11(5GHz)</sub>", "дБ", 23),
                    GetFullStatisticsFromList(id10List, "S<sub>11(10GHz)</sub>", "дБ", 24),
                    GetFullStatisticsFromList(id18List, "S<sub>11(18GHz)</sub>", "дБ", 25)

                };
            return returnList;
        }


        private List<Statistics> GetNoiseMarkers(IEnumerable<string> xList, IEnumerable<List<string>> commonYList,
            double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var firstIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(9.5 - item)).FirstOrDefault());
            var secondIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(12 - item)).FirstOrDefault());
            var id95List = new List<double>();
            var id12List = new List<double>();
            foreach (
                List<double> yListdouble in
                commonYList.Select(
                    yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                id95List.Add(yListdouble[firstIndex]);
                id12List.Add(yListdouble[secondIndex]);
            }

            var returnList = new List<Statistics>
            {
                GetFullStatisticsFromList(id95List, "Noise(9.5GHz)</sub>", "дБ"),
                GetFullStatisticsFromList(id12List, "Noise(12GHz)</sub>", "дБ")
            };
            return returnList;
        }


        private List<Statistics> GetGainMarkers(IEnumerable<string> xList, IEnumerable<List<string>> commonYList,
            double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var firstIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(9.5 - item)).FirstOrDefault());
            var secondIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(12 - item)).FirstOrDefault());
            var id95List = new List<double>();
            var id12List = new List<double>();
            foreach (
                List<double> yListdouble in
                commonYList.Select(
                    yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                id95List.Add(yListdouble[firstIndex]);
                id12List.Add(yListdouble[secondIndex]);
            }

            var returnList = new List<Statistics>
            {
                GetFullStatisticsFromList(id95List, "Gain(9.5GHz)</sub>", "дБ"),
                GetFullStatisticsFromList(id12List, "Gain(12GHz)</sub>", "дБ")
            };
            return returnList;
        }

        private List<Statistics> GetS22Markers(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var twohalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.5 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());
            var eightteenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(18 - item)).FirstOrDefault());
            var id25List = new List<double>();
            var id5List = new List<double>();
            var id10List = new List<double>();
            var id18List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id25List.Add(yListdouble[twohalfIndex]);
                id5List.Add(yListdouble[fiveIndex]);
                id10List.Add(yListdouble[tenIndex]);
                id18List.Add(yListdouble[eightteenIndex]);



            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id25List, "S<sub>22(2.5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id5List,  "S<sub>22(5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id10List, "S<sub>22(10GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id18List, "S<sub>22(18GHz)</sub>", "дБ")

                };
            return returnList;
        }

        private List<Statistics> GetS12Markers(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var twohalfIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2.5 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10 - item)).FirstOrDefault());
            var eightteenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(18 - item)).FirstOrDefault());
            var id25List = new List<double>();
            var id5List = new List<double>();
            var id10List = new List<double>();
            var id18List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id25List.Add(yListdouble[twohalfIndex]);
                id5List.Add(yListdouble[fiveIndex]);
                id10List.Add(yListdouble[tenIndex]);
                id18List.Add(yListdouble[eightteenIndex]);



            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id25List, "S<sub>12(2.5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id5List,  "S<sub>12(5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id10List, "S<sub>12(10GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id18List, "S<sub>12(18GHz)</sub>", "дБ")

                };
            return returnList;
        }

        private List<Statistics> GetZ11Markers(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var enumerable = xList as IList<string> ?? xList.ToList();
            List<double> xListdouble = enumerable.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var z11List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                z11List.Add(yListdouble[0]);
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(z11List, "Z<sub>11(" + enumerable.FirstOrDefault() + "GHz)" + "</sub>", "дБ")

                };
            return returnList;
        }

        private List<Statistics> GetCAP_LEAK(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {

            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var vbrList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList()))
            {
                var indexes = new List<int>();
                for (int index = 0; index < yListdouble.Count; index++)
                {
                    double d = yListdouble[index];
                    if (d >= 1E-6)
                    {
                        if (index < xListdouble.Count)
                        {
                            indexes.Add(index);
                        }

                    }
                }
                vbrList.Add(indexes.Count == 0
                                ? xListdouble.Last()
                                : indexes.Select(index => xListdouble[index]).ToList().Min());
            }
            var twentyIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(20 - item)).FirstOrDefault());
            var twentyList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)/divider).ToList()).Select(yListdouble => yListdouble[twentyIndex]).ToList();

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(twentyList, "Утечка на 20В", "А"),
                    GetFullStatisticsFromList(vbrList, "U<sub>BRC</sub> (пробивное напряжение МДМ-конденсатора)", "В", 50),

                };
            return returnList;
        }

        private List<Statistics> GetVbrx5(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {

            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var vbr100List = new List<double>();
            var vbr10List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList()))
            {
                var indexes100 = new List<int>();
                var indexes10 = new List<int>();
                for (int index = 0; index < yListdouble.Count; index++)
                {
                    double d = yListdouble[index];
                    if (Math.Abs(d) >= 100E-3 - 1E-3)
                    {
                        if (index < xListdouble.Count)
                        {
                            indexes100.Add(index);
                        }

                    }
                    if (Math.Abs(d) >= 10E-3 - 0.5E-3)
                    {
                        if (index < xListdouble.Count)
                        {
                            indexes10.Add(index);
                        }

                    }
                }
                vbr100List.Add(indexes100.Count == 0
                                ? double.NaN
                                : indexes100.Select(index => Math.Abs(xListdouble[index])).ToList().Min());
                vbr10List.Add(indexes10.Count == 0
                               ? double.NaN
                               : indexes10.Select(index => Math.Abs(xListdouble[index])).ToList().Min());
            }



            var returnList = new List<Statistics>
                {

                   GetFullStatisticsFromList(vbr100List.Select(x => Double.IsNaN(x) ? 999.0 : x).ToList(), "V<sub>brc</sub> (напряжение пробоя, ограничение I=100мА )", "В", 15),
                   GetFullStatisticsFromList(vbr10List.Select(x => Double.IsNaN(x) ? 999.0 : x).ToList(), "V<sub>brc</sub> (напряжение пробоя, ограничение I=10мА )", "В", 62)

                };
            return returnList;
        }

        private List<Statistics> GetPinPout(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(-10 - item)).FirstOrDefault());
            var pinList = new List<double>();
            var poutList = new List<double>();
            var pexit = new List<double>();
            var mainindex = 0;
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                var interpolationmethod = Interpolate.Linear(xListdouble.Take(tenIndex).ToList(), yListdouble.Take(tenIndex).ToList());
                var interpolatedList = xListdouble.Select(interpolationmethod.Interpolate).ToList();
                for (int index = 0; index < interpolatedList.Count; index++)
                {
                    double d = interpolatedList[index];
                    if (Math.Abs(d - yListdouble[index]) > 1)
                    {
                        mainindex = index;
                        break;
                    }
                }

                pinList.Add(xListdouble[mainindex]);
                poutList.Add(yListdouble[mainindex]);
                pexit.Add(yListdouble[yListdouble.Count - 1]);


            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(pinList, "Р<sub>лин вх(Рвых1дБ)</sub>", "дБм"),
                    GetFullStatisticsFromList(poutList,  "Р<sub>вых1дБ</sub> (Рвых при компрессии Ку=1дБ)", "дБм"),
                    GetFullStatisticsFromList(pexit,  "Р<sub>выхнас</sub> (Рвых в режиме насыщения)", "дБм"),


                };
            return returnList;
        }

        private List<Statistics> GetS21PinMax(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var poutList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => yListdouble[0]).ToList();
            var returnList = new List<Statistics>
                {
                     GetFullStatisticsFromList(poutList, "Ку в малосигнальном режиме", "дБ"),

                };
            return returnList;
        }

        private List<Statistics> GetS21Pin(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var pinList = new List<double>();
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                var interpolation = MathNet.Numerics.Interpolate.Linear(xListdouble.Take(15), yListdouble.Take(15));
                var interpolationList = xListdouble.Select(x => interpolation.Interpolate(x)).ToList();
                var pList = new List<double>();
                for (int i = 0; i < xListdouble.Count; i++)
                {
                    if (Math.Abs(interpolationList[i] - yListdouble[i]) > 1)
                    {
                        pList.Add(yListdouble[i]);
                    }
                }

                pinList.Add(pList.FirstOrDefault());

            }


            var returnList = new List<Statistics>
                {
                     GetFullStatisticsFromList(pinList, "Р<sub>вых1дБ</sub> (Рвых при компрессии Ку=1дБ)", "дБм"),

                };
            return returnList;
        }

        private List<Statistics> GetS21PinGain(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var poutList = new List<double>();
            var pinList = new List<double>();
            var mainindex = 0;
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                for (int index = 0; index < yListdouble.Count; index++)
                {
                    double d = yListdouble[index];
                    if (Math.Abs(d - yListdouble[0]) > 1)
                    {
                        mainindex = index;
                        break;
                    }
                }
                pinList.Add(yListdouble[0]);
                poutList.Add(xListdouble[mainindex]);


            }

            var returnList = new List<Statistics>
                {
                     GetFullStatisticsFromList(pinList, "Ку в малосигнальном режиме", "дБ"),
                     GetFullStatisticsFromList(poutList,  "Р<sub>вх1дБ</sub> (Рвх при компрессии Ку=1дБ)", "дБм")
                };
            return returnList;
        }




        private List<Statistics> GetVboandN(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = new List<double>();
            List<int> indexesList = new List<int>();

            List<double> xListdoubleforn = new List<double>();
            List<int> indexesListforn = new List<int>();

            List<double> xListdoublefornn = new List<double>();
            List<int> indexesListfornn = new List<int>();


            for (int index = 0; index < xList.Count; index++)
            {
                var x = double.Parse(xList[index], CultureInfo.InvariantCulture);
                if (0.15 <= x && x <= 0.2)
                {
                    xListdouble.Add(x);
                    indexesList.Add(index);
                }
                if (0.2 <= x && x <= 0.75)
                {
                    xListdoubleforn.Add(x);
                    indexesListforn.Add(index);
                }
                if (0.2 <= x && x <= 0.75)
                {
                    xListdoublefornn.Add(x);
                    indexesListfornn.Add(index);
                }

            }

            var vboList = new List<double>();
            var nList = new List<double>();
            var n1List = new List<double>();




            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList()))
            {
                var yListdoubletemp = new List<double>();
                var yListdoubletempforn = new List<double>();
                var yListdoubletempfornn = new List<double>();
                for (int index = indexesList.FirstOrDefault(); index < indexesList.Last() + 1; index++)
                {
                    var ydouble = yListdouble[index];
                    yListdoubletemp.Add(ydouble);
                }

                for (int index = indexesListforn.FirstOrDefault(); index < indexesListforn.Last() + 1; index++)
                {
                    var ydouble = yListdouble[index];
                    yListdoubletempforn.Add(ydouble);
                }

                for (int index = indexesListfornn.FirstOrDefault(); index < indexesListfornn.Last() + 1; index++)
                {
                    var ydouble = yListdouble[index];
                    yListdoubletempfornn.Add(ydouble);
                }

                var yListdoublelog10 = yListdoubletemp.Select(Math.Log10).ToList();
                var yListdoublelog10Forn = yListdoubletempforn.Select(Math.Log10).ToList();
            //    var yListdoublelog10Fornn = yListdoubletempfornn.Select(Math.Log10).ToList();
                var interpolationmethodvpo = Interpolate.Linear(xListdouble, yListdoublelog10);
                var interpolationmethodvpoforn = Interpolate.Linear(xListdoubleforn, yListdoublelog10Forn);
             //  var interpolationmethodvpofornn = Interpolate.Linear(xListdoublefornn, yListdoublelog10Fornn);
                var Is = interpolationmethodvpo.Interpolate(0);
                var nnList = new List<double>();


             //   var dn = interpolationmethodvpofornn.Differentiate(0.6);
                const double contactArea = 7.5E-9;
                const double A = 8.17E+4;//постоянная Ричардсона
                const int T = 295;
                const double k = 8.617332E-5;
                var k2 = 1.380648E-23;//постоянная Больцмана
                var e = 1.60218E-19;
                var vbo = k * T * Math.Log(10) * (Math.Log10(contactArea * A * Math.Pow(T, 2)) - Is);
                for (double i = 0.2; i < 0.75; i = i + 0.0025)
                {
                    var a = 1/Math.Log(10)*(e/(k2*T))*(1/interpolationmethodvpoforn.Differentiate(i));
                    if (!Double.IsNaN(a))
                    {
                        nnList.Add(a);
                    }

                }
               // var n1 = (1 / Math.Log(10)) * (e / (k2 * T)) * (1 / dn);
                vboList.Add(vbo);
                var newList = nnList.Where(d => !Double.IsNaN(d) && !Double.IsInfinity(d) && Math.Abs(d) > 1.1).Select(Math.Abs).ToList();
             //   var t = MathNet.Numerics.Statistics.Statistics.MovingAverage(newList, newList.Count).ToList().Select(Math.Abs);
                if (newList.Count != 0)
                {
                     nList.Add(newList.Min());
                }

             // n1List.Add(n1);
            }


            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vboList, "Vbo (высота барьера Шоттки)", "эВ", 13),
                    GetFullStatisticsFromList(nList, "n (коэффициент неидеальности)", " ", 16),


                };
            return returnList;
        }

        public bool UseScietificNotation(IEnumerable<List<string>> commonYList)
        {
            var boolList = new List<bool>();
            foreach (var list in commonYList)
            {
                var boolsinglelistList = new List<bool>();
                foreach (string s in list)
                {

                    if (Math.Abs(double.Parse(s, CultureInfo.InvariantCulture)) > 1E-3 && Math.Abs(double.Parse(s, CultureInfo.InvariantCulture)) < 1000)
                    {
                        boolsinglelistList.Add(false);
                    }
                    else
                    {
                        boolsinglelistList.Add(true);
                    }

                }
                var boolcount = boolsinglelistList.Count(x => !x);
                boolList.Add(boolcount > 0);
            }
            var boolcountmain = boolList.Count(x => x);
            return !(boolcountmain > 0);
        }

        //ED
        private List<Statistics> GetRis_ED(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var sixIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(6.0 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10.0 - item)).FirstOrDefault());
            var index100 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(100.0 - item)).FirstOrDefault());
            var index150 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(150.0 - item)).FirstOrDefault());


            var sixList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => sixIndex < 0 ? yListdouble[0] : yListdouble[sixIndex]).ToList();
            var tenList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => tenIndex < 0 ? yListdouble[0] : yListdouble[tenIndex]).ToList();
            var list100 = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => index100 < 0 ? yListdouble[0] : yListdouble[index100]).ToList();
            var list150 = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => index150 < 0 ? yListdouble[0] : yListdouble[index150]).ToList();

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(sixList, "R<sub>IS(6V)</sub>", "Ом"),
                    GetFullStatisticsFromList(tenList, "R<sub>IS(10V)</sub>", "Ом"),
                    GetFullStatisticsFromList(list100, "R<sub>IS(100V)</sub>", "Ом"),
                    GetFullStatisticsFromList(list150, "R<sub>IS(150V)</sub>", "Ом"),
                };
            return returnList;
        }

        private List<Statistics> GetIis_ED(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var sixIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(6.0 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(10.0 - item)).FirstOrDefault());
            var index100 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(100.0 - item)).FirstOrDefault());
            var index150 = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(150.0 - item)).FirstOrDefault());



            var sixList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => sixIndex < 0 ? yListdouble[0] : yListdouble[sixIndex]).ToList();
            var tenList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => tenIndex < 0 ? yListdouble[0] : yListdouble[tenIndex]).ToList();
            var list100 = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => index100 < 0 ? yListdouble[0] : yListdouble[index100]).ToList();
            var list150 = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => index150 < 0 ? yListdouble[0] : yListdouble[index150]).ToList();

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(sixList, "I<sub>IS(6V)</sub>", "А"),
                    GetFullStatisticsFromList(tenList, "I<sub>IS(10V)</sub>", "А"),
                    GetFullStatisticsFromList(list100, "I<sub>IS(100V)</sub>", "А"),
                    GetFullStatisticsFromList(list150, "I<sub>IS(150V)</sub>", "А"),

                };
            return returnList;
        }

        private List<Statistics> GetIDSS35_ED(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var threeIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(3.0 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5.0 - item)).FirstOrDefault());
            var unit = "A";
            var threeList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => threeIndex < 0 ? yListdouble[0] : yListdouble[threeIndex]).ToList();
            var fiveList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => fiveIndex < 0 ? yListdouble[0] : yListdouble[fiveIndex]).ToList();

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(threeList, "I<sub>D(3V)</sub>", unit, 0, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(fiveList, "I<sub>D(5V)</sub>", unit, 0, DividerProfile.WithDivider),

                };
            return returnList;
        }

        private List<Statistics> GetIDSS310_ED(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var threeIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(-3.0 - item)).FirstOrDefault());
            var tenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(-10.0 - item)).FirstOrDefault());
            var unit = "A";
            var threeList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => threeIndex < 0 ? yListdouble[0] : yListdouble[threeIndex]).ToList();
            var tenList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => tenIndex < 0 ? yListdouble[0] : yListdouble[tenIndex]).ToList();

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(threeList, "I<sub>GSS(-3V)</sub>", unit, 0, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(tenList, "I<sub>GSS(-10V)</sub>", unit, 0, DividerProfile.WithDivider),

                };
            return returnList;
        }

        private List<Statistics> GetIdssAndUgsoff_ED_D(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var zeroIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.0 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.5 - item)).FirstOrDefault());
            var idssList = new List<double>();
            var id05List = new List<double>();
            var ugsoffList = new List<double>();
            var unit = "A";
            var level = 50E-6;
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                var fiftyIndex = yListdouble.IndexOf(yListdouble.OrderBy(item => Math.Abs(level - item)).FirstOrDefault());
                ugsoffList.Add(xListdouble[fiftyIndex]);
                idssList.Add(yListdouble[zeroIndex]);
                id05List.Add(yListdouble[fiveIndex]);
            }

            var returnList = new List<Statistics>
            {


                    GetFullStatisticsFromList(ugsoffList, "V<sub>GS(off)</sub>", "В"),
                    GetFullStatisticsFromList(idssList, "I<sub>DSS</sub>", unit, 0, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(id05List, "I<sub>D(max)</sub>", unit, 0, DividerProfile.WithDivider),



            };
            return returnList;
        }

        private List<Statistics> GetIdssAndUgsoff_ED_E(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var zeroIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0.0 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(1.5 - item)).FirstOrDefault());
            var idssList = new List<double>();
            var id05List = new List<double>();
            var ugsoffList = new List<double>();
            var unit = "A";
            var level = 50E-6;
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                var fiftyIndex = yListdouble.IndexOf(yListdouble.OrderBy(item => Math.Abs(level - item)).FirstOrDefault());
                ugsoffList.Add(xListdouble[fiftyIndex]);
                idssList.Add(yListdouble[zeroIndex]);
                id05List.Add(yListdouble[fiveIndex]);
            }

            var returnList = new List<Statistics>
            {

                    GetFullStatisticsFromList(ugsoffList, "V<sub>GS(th)</sub>", "В"),
                    GetFullStatisticsFromList(idssList, "I<sub>DSS</sub>", unit, 0, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(id05List, "I<sub>D(max)</sub>", unit, 0, DividerProfile.WithDivider),



            };
            return returnList;
        }

        private List<Statistics> GetGmax_ED(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            var xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var unit = "См";
            var gmaxList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => yListdouble.Max()).ToList();
            var vpeakList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => xListdouble[yListdouble.IndexOf(yListdouble.Max())]).ToList();
            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(gmaxList, "g<sub>m(max)</sub>", unit, 0, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(vpeakList, "V<sub>gm-peak</sub>", "В")
                };
            return returnList;
        }

        private List<Statistics> GetRon_ED(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            int zeroIndex = 0;
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            for (int index = 0; index < xListdouble.Count; index++)
            {
                double d = xListdouble[index];
                if (Math.Abs(d - 0.05) < 0.0005)
                {
                    zeroIndex = index;
                }

            }

            var ronList = new List<double>();
            if (divider < 1)
            {
                divider = 1 / divider;
            }
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                ronList.Add(yListdouble[zeroIndex]);
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(ronList, "r<sub>DS(on)</sub>", "Ом", 0, DividerProfile.ROnFamily),
                };
            return returnList;
        }

        private List<Statistics> GetVboandN_ED(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = new List<double>();
            List<int> indexesList = new List<int>();

            List<double> xListdoubleforn = new List<double>();
            List<int> indexesListforn = new List<int>();

            List<double> xListdoublefornn = new List<double>();
            List<int> indexesListfornn = new List<int>();


            for (int index = 0; index < xList.Count; index++)
            {
                var x = double.Parse(xList[index], CultureInfo.InvariantCulture);
                if (0.15 <= x && x <= 0.2)
                {
                    xListdouble.Add(x);
                    indexesList.Add(index);
                }
                if (0.2 <= x && x <= 0.75)
                {
                    xListdoubleforn.Add(x);
                    indexesListforn.Add(index);
                }
                if (0.2 <= x && x <= 0.75)
                {
                    xListdoublefornn.Add(x);
                    indexesListfornn.Add(index);
                }

            }

            var vboList = new List<double>();
            var nList = new List<double>();
            var n1List = new List<double>();




            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList()))
            {
                var yListdoubletemp = new List<double>();
                var yListdoubletempforn = new List<double>();
                var yListdoubletempfornn = new List<double>();
                for (int index = indexesList.FirstOrDefault(); index < indexesList.Last() + 1; index++)
                {
                    var ydouble = yListdouble[index];
                    yListdoubletemp.Add(ydouble);
                }

                for (int index = indexesListforn.FirstOrDefault(); index < indexesListforn.Last() + 1; index++)
                {
                    var ydouble = yListdouble[index];
                    yListdoubletempforn.Add(ydouble);
                }

                for (int index = indexesListfornn.FirstOrDefault(); index < indexesListfornn.Last() + 1; index++)
                {
                    var ydouble = yListdouble[index];
                    yListdoubletempfornn.Add(ydouble);
                }

                var yListdoublelog10 = yListdoubletemp.Select(Math.Log10).ToList();
                var yListdoublelog10Forn = yListdoubletempforn.Select(Math.Log10).ToList();
                //    var yListdoublelog10Fornn = yListdoubletempfornn.Select(Math.Log10).ToList();
                var interpolationmethodvpo = Interpolate.Linear(xListdouble, yListdoublelog10);
                var interpolationmethodvpoforn = Interpolate.Linear(xListdoubleforn, yListdoublelog10Forn);
                //  var interpolationmethodvpofornn = Interpolate.Linear(xListdoublefornn, yListdoublelog10Fornn);
                var Is = interpolationmethodvpo.Interpolate(0);
                var nnList = new List<double>();


                //   var dn = interpolationmethodvpofornn.Differentiate(0.6);
                const double contactArea = 7.5E-9;
                const double A = 8.17E+4;//постоянная Ричардсона
                const int T = 295;
                const double k = 8.617332E-5;
                var k2 = 1.380648E-23;//постоянная Больцмана
                var e = 1.60218E-19;
                var vbo = k * T * Math.Log(10) * (Math.Log10(contactArea * A * Math.Pow(T, 2)) - Is);
                for (double i = 0.2; i < 0.75; i = i + 0.0025)
                {
                    var a = 1 / Math.Log(10) * (e / (k2 * T)) * (1 / interpolationmethodvpoforn.Differentiate(i));
                    if (!Double.IsNaN(a))
                    {
                        nnList.Add(a);
                    }

                }
                // var n1 = (1 / Math.Log(10)) * (e / (k2 * T)) * (1 / dn);
                vboList.Add(vbo);
                var newList = nnList.Where(d => !Double.IsNaN(d) && !Double.IsInfinity(d) && Math.Abs(d) > 1.1).Select(Math.Abs).ToList();
                //   var t = MathNet.Numerics.Statistics.Statistics.MovingAverage(newList, newList.Count).ToList().Select(Math.Abs);
                if (newList.Count != 0)
                {
                    nList.Add(newList.Min());
                }

                // n1List.Add(n1);
            }


            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vboList, "Ф<sub>В</sub>", "эВ"),
                    GetFullStatisticsFromList(nList, "n", " ")


                };
            return returnList;
        }

        private List<Statistics> GetS21ZIONTYPE1(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {

            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var threeIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(3 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5 - item)).FirstOrDefault());
            var sevenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(7 - item)).FirstOrDefault());
            var nineIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(9 - item)).FirstOrDefault());
            var twentyIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2 - item)).FirstOrDefault());

            List<double> id3List = new List<double>();
            List<double> id5List = new List<double>();
            List<double> id7List = new List<double>();
            List<double> id9List = new List<double>();
            List<double> id2List = new List<double>();

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id3List.Add(yListdouble[threeIndex]);
                id5List.Add(yListdouble[fiveIndex]);
                id7List.Add(yListdouble[sevenIndex]);
                id9List.Add(yListdouble[nineIndex]);
                id2List.Add(yListdouble[twentyIndex]);
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id2List, "S21<sub>(2GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id5List,  "S21<sub>(5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id7List, "S21<sub>(7GHz)</sub>", "дБ"),

                };
            return returnList;
        }
        private List<Statistics> GetS21ZIONTYPE2(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {

            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var fIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(4.0 - item)).FirstOrDefault());
            var threeIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5.5 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(9.5 - item)).FirstOrDefault());
            var sevenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(13.5 - item)).FirstOrDefault());


            List<double> id3List = new List<double>();
            List<double> id5List = new List<double>();
            List<double> id7List = new List<double>();
            List<double> id9List = new List<double>();
            List<double> id2List = new List<double>();

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                id2List.Add(yListdouble[fIndex]);
                id3List.Add(yListdouble[threeIndex]);
                id5List.Add(yListdouble[fiveIndex]);
                id7List.Add(yListdouble[sevenIndex]);

            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id2List, "S21<sub>(4GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id3List, "S21<sub>(5.5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id5List,  "S21<sub>(9.5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id7List, "S21<sub>(13.5GHz)</sub>", "дБ"),


                };
            return returnList;
        }
        private List<Statistics> GetS31ZIONTYPE1(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {

            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var threeIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(3 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5 - item)).FirstOrDefault());
            var sevenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(7 - item)).FirstOrDefault());
            var nineIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(9 - item)).FirstOrDefault());
            var twentyIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(2 - item)).FirstOrDefault());

            List<double> id3List = new List<double>();
            List<double> id5List = new List<double>();
            List<double> id7List = new List<double>();
            List<double> id9List = new List<double>();
            List<double> id2List = new List<double>();

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                id3List.Add(yListdouble[threeIndex]);
                id5List.Add(yListdouble[fiveIndex]);
                id7List.Add(yListdouble[sevenIndex]);
                id9List.Add(yListdouble[nineIndex]);
                id2List.Add(yListdouble[twentyIndex]);
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id2List, "S31<sub>(2GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id5List, "S31<sub>(5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id7List, "S31<sub>(7GHz)</sub>", "дБ"),


                };
            return returnList;
        }
        private List<Statistics> GetS31ZIONTYPE2(IEnumerable<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {


            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var fIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(4.0 - item)).FirstOrDefault());
            var threeIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(5.5 - item)).FirstOrDefault());
            var fiveIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(9.5 - item)).FirstOrDefault());
            var sevenIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(13.5 - item)).FirstOrDefault());


            List<double> id3List = new List<double>();
            List<double> id5List = new List<double>();
            List<double> id7List = new List<double>();
            List<double> id9List = new List<double>();
            List<double> id2List = new List<double>();

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                id2List.Add(yListdouble[fIndex]);
                id3List.Add(yListdouble[threeIndex]);
                id5List.Add(yListdouble[fiveIndex]);
                id7List.Add(yListdouble[sevenIndex]);

            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(id2List, "S31<sub>(4GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id3List, "S31<sub>(5.5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id5List,  "S31<sub>(9.5GHz)</sub>", "дБ"),
                    GetFullStatisticsFromList(id7List,  "S31<sub>(13.5GHz)</sub>", "дБ"),
                 

                };
            return returnList;
        }

        private List<Statistics> GetVbrdg_ED(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var vbrdgList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList()))
            {
                var hundredList = new List<int>();
                foreach (var d in yListdouble.Where(item => item >= 50E-6).ToList())
                {
                    hundredList.Add(yListdouble.IndexOf(d));
                }
                var vbrdg = xListdouble[hundredList.OrderBy(x => x).FirstOrDefault()];
                vbrdgList.Add(vbrdg == xListdouble.FirstOrDefault() ? xListdouble.LastOrDefault() : vbrdg);
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vbrdgList.Where(x => x < xListdouble.Last()).ToList(), "V<sub>(BR)DG</sub>", "В"),

                };
            returnList[0].FullList = vbrdgList.ToList();
            return returnList;
        }

        private List<Statistics> GetVbrGaN(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var vbrdgList = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList()))
            {
                var hundredList = new List<int>();
                foreach (var d in yListdouble.Where(item => item >= 5E-3).ToList())
                {
                    hundredList.Add(yListdouble.IndexOf(d));
                }
                var vbrdg = xListdouble[hundredList.OrderBy(x => x).FirstOrDefault()];
                vbrdgList.Add(vbrdg == xListdouble.FirstOrDefault() ? xListdouble.LastOrDefault() : vbrdg);
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(vbrdgList.Where(x => x < xListdouble.Last()).ToList(), "V<sub>(BR)</sub>", "В"),

                };
            returnList[0].FullList = vbrdgList.ToList();
            return returnList;
        }


        private List<Statistics> GetCMIM_ED(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var zeroIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(0 - item)).FirstOrDefault());


            var zeroList = commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()).Select(yListdouble => zeroIndex < 0 ? yListdouble[0] : yListdouble[zeroIndex]).ToList();

            var returnList = new List<Statistics>
                {

                    GetFullStatisticsFromList(zeroList, "C", "Ф"),
                    GetFullStatisticsFromList(zeroList.Select(x=>x*1E12/0.04).ToList(), "C<sub>MIM</sub>", "пФ/мм²")

                };
            return returnList;
        }

        private List<Statistics> GetLeak_ED(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var thirtyIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(30.0 - item)).FirstOrDefault());
            var vbrdgList = new List<double>();
            var ig30List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {
                ig30List.Add(thirtyIndex < 0 ? yListdouble[0] : yListdouble[thirtyIndex]);
            }
            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(ig30List, "I<sub>C(leak)</sub>", "А", 0, DividerProfile.WithDivider),
                };
            return returnList;
        }


        private List<Statistics> GetLeak_SKY(List<string> xList, IEnumerable<List<string>> commonYList, double divider)
        {
            List<double> xListdouble = xList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
            var thirtyIndex = xListdouble.IndexOf(xListdouble.OrderBy(item => Math.Abs(30.0 - item)).FirstOrDefault());
            var vbrdgList = new List<double>();
            var ig30List = new List<double>();
            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture) / divider).ToList()))
            {

                ig30List.Add(thirtyIndex < 0 ? yListdouble[0] : yListdouble[thirtyIndex]);

            }

            foreach (List<double> yListdouble in commonYList.Select(yList => yList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList()))
            {
                var hundredList = new List<int>();
                foreach (var d in yListdouble.Where(item => Math.Abs(1E-7 - item) < 5E-8).ToList())
                {
                    hundredList.Add(yListdouble.IndexOf(d));
                }
                var vbrdg = xListdouble[hundredList.OrderBy(x => x).FirstOrDefault()];
                vbrdgList.Add(vbrdg == xListdouble.FirstOrDefault() ? xListdouble.LastOrDefault() : vbrdg);
            }

            var returnList = new List<Statistics>
                {
                    GetFullStatisticsFromList(ig30List, "I<sub>C(leak)</sub>", "А", 0, DividerProfile.WithDivider),
                    GetFullStatisticsFromList(vbrdgList, "V<sub>(br)</sub> (напряжение при Ig=100нА)", "В"),
                };
            return returnList;
        }

    }


}