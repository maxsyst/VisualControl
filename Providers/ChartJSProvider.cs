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

        public async Task<AbstractChart> GetLinearFromDieValues(Dictionary <long?, DieValue> dieValuesDictionary, List<long?> dieIdList, double divider, string keyGraphicState) 
        {
            var datasets = new ConcurrentDictionary<long, Dataset>();
            var currentDieValues = dieIdList.Select(dieId => dieValuesDictionary[dieId]).ToList();
            Graphic graphic = await _graphicService.GetGraphicByKeyGraphicState(keyGraphicState);
            Parallel.ForEach (currentDieValues, (dieValue) => 
            {
                var dataset = new LinearDataset();
                dataset.BorderColor = _colorService.GetHexColorByDieId(dieValue.DieId);
                dataset.DieId = (long)dieValue.DieId;
                for (int i = 0; i < dieValue.YList.Count; i++)
                {
                    dataset.Data.Add(double.Parse(dieValue.YList[i], CultureInfo.InvariantCulture) / divider);
                }
                datasets.TryAdd(dataset.DieId, dataset);
            });
            if(divider - 1.0 > 1E6)
            {
                graphic.OrdinateUnit =  $"{graphic.OrdinateUnit}/мм"; 
            }
            var labelsList = new List<string>();
            var chart = new LinearChart (labelsList, 
                                         datasets.ToDictionary(kv => kv.Key, kv => kv.Value), 
                                         new ChartModels.ChartJs.Options.XAxis($"{graphic.Absciss}({graphic.AbscissUnit})", true), 
                                         new ChartModels.ChartJs.Options.YAxis($"{graphic.Ordinate}({graphic.OrdinateUnit})", true));
            labelsList.AddRange(currentDieValues.FirstOrDefault().XList);
            return chart;
        }

        public async Task<AbstractChart> GetHistogramFromDieValues(List<DieValue> dieValuesList, List<long?> dieIdList, double divider, string keyGraphicState) 
        {
            var labelsList = new List<string>();
            var datasetList = new Dictionary<long, Dataset>();
            Graphic graphic = await _graphicService.GetGraphicByKeyGraphicState(keyGraphicState);
            if(divider - 1.0 > 1E6)
            {
                graphic.OrdinateUnit =  $"{graphic.OrdinateUnit}/мм"; 
            }
            var chart = new BarChart(labelsList, 
                                     datasetList, 
                                     new ChartModels.ChartJs.Options.XAxis($"Номер кристалла", true), 
                                     new ChartModels.ChartJs.Options.YAxis($"{graphic.Ordinate}({graphic.OrdinateUnit})", true));
            var dataset = new BarDataset();
            var dataDictionary = new Dictionary<string, SingleBarDataset>();
            foreach (var dieValue in dieValuesList.Where(x => dieIdList.Contains(x.DieId)).ToList()) 
            {
                var die = await _dieProvider.GetById((long) dieValue.DieId);
                dataDictionary.Add(Convert.ToString(die.Code), new SingleBarDataset(die.DieId, "#3D5AFE", double.Parse(dieValue.YList[0], CultureInfo.InvariantCulture) / divider));
            }
            foreach (var kv in dataDictionary.OrderBy(x => Convert.ToInt32(x.Key.Split('-')[0])))
            {
                labelsList.Add(kv.Key);
                dataset.Data.Add(kv.Value.DieId, kv.Value.Value);
                dataset.BackgroundColor.Add(kv.Value.BackgroundColor);
            }
            datasetList.Add(1, dataset);
            return chart;
        }
    }
}