using System.Globalization;
using System.Linq;
using System;
using System.Collections.Generic;
using LazyCache;
using VueExample.Models.SRV6;
using VueExample.Providers;
using VueExample.Models.SRV6.Export;
using System.Threading.Tasks;
using VueExample.Providers.Srv6.Interfaces;
using System.Collections.Concurrent;
using VueExample.StatisticsCoreRework.Abstract;

namespace VueExample.StatisticsCoreRework.Services
{
    public class ExportService : IExportProvider
    {
        private readonly IAppCache _appCache;
        private readonly IDieValueService _dieValueService;
        private readonly IStatParameterService _statParameterService;
        private readonly IStageProvider _stageProvider;
        private readonly  VueExample.StatisticsCoreRework.Services.StatisticService _statisticService;
        private readonly IDieProvider _dieProvider;
        public ExportService(IAppCache appCache,  VueExample.StatisticsCoreRework.Services.StatisticService statisticService, IStageProvider stageProvider, IDieValueService dieValueService, IDieProvider dieProvider, ISRV6GraphicService graphicService, IStatParameterService statParameterService)
        {
            _dieValueService = dieValueService;
            _dieProvider = dieProvider;
            _statParameterService = statParameterService;
            _appCache = appCache;
            _stageProvider = stageProvider;
            _statisticService = statisticService;
        }
        public async Task PopulateKurbatovXLSByValues(KurbatovXLS kurbatovXLS)
        {
            List<KurbatovParameter> list = kurbatovXLS.kpList.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                KurbatovParameter kurbatovParameter = list[i];
                kurbatovXLS.kpList[i].advList.AddRange((await GetDieValues(kurbatovParameter)).ToList());
            }
            kurbatovXLS.FindDirty();
        }

        public async Task<List<string>> GetStatisticsNameByMeasurementId(int measurementRecordingId)
        {
            var statNamesList = new List<string>();
            string measurementRecordingIdAsKey = Convert.ToString(measurementRecordingId);
            var dieValuesDictionary = await _dieValueService.GetDieValuesByMeasurementRecording(measurementRecordingId);
            var statDictionary = await _statisticService.GetSingleParameterStatisticByDieValues(new ConcurrentDictionary<string, List<DieValue>>(dieValuesDictionary), measurementRecordingId);
            statNamesList.AddRange(from stat in statDictionary
                                   from singleParameterStatistic in stat.Value
                                   select singleParameterStatistic.Value.StatisticName);
            return statNamesList;
        }

        private async Task<List<AtomicDieValue>> GetDieValues(KurbatovParameter kurbatovParameter)
        {
            string measurementRecordingIdAsKey = Convert.ToString(kurbatovParameter.MeasurementRecordingId);
            var stageId = (await _stageProvider.GetByMeasurementRecordingId(kurbatovParameter.MeasurementRecordingId)).StageId;
            var dieValuesDictionary = await _dieValueService.GetDieValuesByMeasurementRecording(kurbatovParameter.MeasurementRecordingId);
            var statDictionary = await _statisticService.GetSingleParameterStatisticByDieValues(new ConcurrentDictionary<string, List<DieValue>>(dieValuesDictionary), kurbatovParameter.MeasurementRecordingId);
            foreach (var stat in statDictionary)
            {
                foreach (var singleParameterStatistic in stat.Value)
                {
                    if (kurbatovParameter.ParameterNameStat == singleParameterStatistic.Value.StatisticName)
                    {
                        for (int i = 0; i < singleParameterStatistic.Value.DieStatDictionary.Count; i++)
                        {
                            long? die = (long?)singleParameterStatistic.Value.DieStatDictionary.Keys.ElementAt(i);
                            var value = Convert.ToDouble(singleParameterStatistic.Value.DieStatDictionary.Values.ElementAt(i), CultureInfo.InvariantCulture) / kurbatovParameter.Divider;
                            kurbatovParameter.advList.Add
                            (
                                new AtomicDieValue
                                {
                                    DieCode = (await _dieProvider.GetById((long)die)).Code,
                                    Value = value,
                                    Status = GetStatus(kurbatovParameter.Lower, kurbatovParameter.Upper, value)
                                }
                            );
                            kurbatovParameter.advList = kurbatovParameter.advList.OrderBy(_ => Convert.ToInt32(_.DieCode)).ToList();
                        }
                    }
                }
            }
            return kurbatovParameter.advList;
        }

        private string GetStatus(double lower, double upper, double value)
        {
            if ((value >= lower || Double.IsNaN(lower)) && (value <= upper || Double.IsNaN(upper)))
            {
                return "Годен";
            }
            else
            {
                return "Брак";
            }
        }

        public async Task<List<Dictionary<string, string>>> Export(int measurementRecordingId, string statNames, string delimeter)
        {
            string measurementRecordingIdAsKey = Convert.ToString(measurementRecordingId);
            var statNamesList = statNames.Split(delimeter).ToList();
            var exportList = new List<Dictionary<string, string>>();
            var stageId = (await _stageProvider.GetByMeasurementRecordingId(measurementRecordingId)).StageId;
            var dieValuesDictionary = await _dieValueService.GetDieValuesByMeasurementRecording(measurementRecordingId);
            var statDictionary = await _statisticService.GetSingleParameterStatisticByDieValues(new ConcurrentDictionary<string, List<DieValue>>(dieValuesDictionary), measurementRecordingId);
            foreach (var stat in statDictionary)
            {
                foreach (var singleParameterStatistic in stat.Value)
                {
                    if (statNamesList.Contains(singleParameterStatistic.Value.StatisticName))
                    {
                        var d = new Dictionary<string, string>();
                        var dieList = new List<int>();
                        d["name"] = singleParameterStatistic.Value.StatisticName;
                        for (int i = 0; i < singleParameterStatistic.Value.DieStatDictionary.Count; i++)
                        {
                            long? die = (long?)singleParameterStatistic.Value.DieStatDictionary.Keys.ElementAt(i);
                            dieList.Add(Convert.ToInt32((await _dieProvider.GetById((long)die)).Code));
                            d["#" + Convert.ToString(i + 1)] = Convert.ToString(singleParameterStatistic.Value.DieStatDictionary.Values.ElementAt(i), CultureInfo.InvariantCulture);
                        }
                        exportList.Add(d);
                    }
                }
            }
            return exportList;
        }
    }
}