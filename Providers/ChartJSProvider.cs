using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VueExample.ChartModels.ChartJs;
using VueExample.ChartModels.ChartJs.Bar;
using VueExample.ChartModels.ChartJs.Linear;
using VueExample.Color;
using VueExample.Models.SRV6;
using VueExample.Providers.Srv6.Interfaces;

namespace VueExample.Providers
{
    public class ChartJSProvider : IChartJSProvider 
    {
        private readonly IDieProvider _dieProvider;
        private readonly IColorService _colorService;
        private readonly ISRV6GraphicService _graphicService;

        public ChartJSProvider (IDieProvider dieProvider, ISRV6GraphicService graphicService, IColorService colorService) 
        {
            _dieProvider = dieProvider;
            _graphicService = graphicService;
            _colorService = colorService;
        }

        public async Task<AbstractChart> GetLinearFromDieValues(List<DieValue> dieValuesList, List<long?> dieIdList, double divider, string keyGraphicState) 
        {
            var datasetList = new ConcurrentBag<Dataset>();
            var currentDieValues = dieValuesList.Where(x => dieIdList.Contains(x.DieId)).ToList();
            Graphic graphic = await _graphicService.GetGraphicByKeyGraphicState(keyGraphicState);
            Parallel.ForEach (currentDieValues, (dieValue) => 
            {
                var dataset = new LinearDataset();
                dataset.BorderColor = _colorService.GetHexColorByDieId(dieValue.DieId);
                dataset.DieId = dieValue.DieId;
                for (int i = 0; i < dieValue.XList.Count; i++) 
                {
                    if (i < dieValue.YList.Count) 
                    {
                        dataset.Data.Add(double.Parse (dieValue.YList[i], CultureInfo.InvariantCulture) / divider);
                    }
                }
                datasetList.Add(dataset);
            });
            var labelsList = new List<string> ();
            var chart = new LinearChart (labelsList, 
                                         datasetList, 
                                         new ChartModels.ChartJs.Options.XAxis($"{graphic.Absciss}({graphic.AbscissUnit})", true), 
                                         new ChartModels.ChartJs.Options.YAxis($"{graphic.Ordinate}({graphic.OrdinateUnit})", true));
            labelsList.AddRange(dieValuesList.FirstOrDefault().XList);
            return chart;
        }

        public async Task<AbstractChart> GetHistogramFromDieValues (List<DieValue> dieValuesList, List<long?> dieIdList, double divider, string keyGraphicState) 
        {
            var labelsList = new List<string>();
            var datasetList = new List<Dataset>();
            Graphic graphic = await _graphicService.GetGraphicByKeyGraphicState(keyGraphicState);
            var chart = new BarChart(labelsList, 
                                     datasetList, 
                                     new ChartModels.ChartJs.Options.XAxis($"Номер кристалла", true), 
                                     new ChartModels.ChartJs.Options.YAxis($"{graphic.Ordinate}({graphic.OrdinateUnit})", true));
            var dataset = new BarDataset();
            foreach (var dieValue in dieValuesList.Where(x => dieIdList.Contains(x.DieId)).ToList()) 
            {
                var die = await _dieProvider.GetById((long) dieValue.DieId);
                labelsList.Add(Convert.ToString(die.Code));
                dataset.BackgroundColor.Add("#3D5AFE");
                dataset.DieIdList.Add(die.DieId);
                dataset.Data.Add(double.Parse(dieValue.YList[0], CultureInfo.InvariantCulture) / divider);
            }
            datasetList.Add(dataset);
            return chart;
        }
    }
}