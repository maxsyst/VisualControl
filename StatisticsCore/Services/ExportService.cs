using System.Globalization;
using System.Linq;
using System;
using System.Collections.Generic;
using LazyCache;
using VueExample.Models.SRV6;
using VueExample.StatisticsCore.Abstract;
using VueExample.Providers;
using VueExample.Models.SRV6.Export;
using System.Threading.Tasks;
using VueExample.Providers.Srv6.Interfaces;
using System.Collections.Concurrent;

namespace VueExample.StatisticsCore.Services
{
    public class ExportService : IExportProvider
    {
        private readonly IAppCache _appCache;        
        private readonly IDieValueService _dieValueService;
        private readonly IStatParameterService _statParameterService;
        private readonly IStageProvider _stageProvider;
        private readonly IStatisticService _statisticService;
        private readonly IDieProvider _dieProvider;
        public ExportService(IAppCache appCache, IStatisticService statisticService, IStageProvider stageProvider, IDieValueService dieValueService, IDieProvider dieProvider, ISRV6GraphicService graphicService, IStatParameterService statParameterService)
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

        public async Task<List<string>> GetStatisticsNameByMeasurementId(int measurementRecordingId, double k)
        {
            var statNamesList = new List<string>();
            string measurementRecordingIdAsKey = Convert.ToString(measurementRecordingId);
            var stageId = (await _stageProvider.GetByMeasurementRecordingId(measurementRecordingId)).StageId;
            var dieValuesDictionary = await _dieValueService.GetDieValuesByMeasurementRecording(measurementRecordingId);
            var statDictionary =  await _statisticService.GetSingleParameterStatisticByDieValues(new ConcurrentDictionary<string, List<DieValue>>(dieValuesDictionary), measurementRecordingId, stageId, 1.0, k);
            statNamesList.AddRange(from stat in statDictionary
                                   from singleParameterStatistic in stat.Value
                                   select singleParameterStatistic.Name);
            return statNamesList;
        }

        

        private async Task<List<AtomicDieValue>> GetDieValues(KurbatovParameter kurbatovParameter, double k = 1.5)
        {
            string measurementRecordingIdAsKey = Convert.ToString(kurbatovParameter.MeasurementRecordingId);
            var stageId = (await _stageProvider.GetByMeasurementRecordingId(kurbatovParameter.MeasurementRecordingId)).StageId;
            var dieValuesDictionary = await _dieValueService.GetDieValuesByMeasurementRecording(kurbatovParameter.MeasurementRecordingId);
            var statDictionary = await _statisticService.GetSingleParameterStatisticByDieValues(new ConcurrentDictionary<string, List<DieValue>>(dieValuesDictionary), kurbatovParameter.MeasurementRecordingId, stageId, 1.0, k);
            foreach (var stat in statDictionary) 
            {
                foreach (var singleParameterStatistic in stat.Value) 
                {
                    if(kurbatovParameter.ParameterNameStat == singleParameterStatistic.Name)
                    {
                        for (int i = 0; i < singleParameterStatistic.dieList.Count; i++)
                        {
                            
                            long? die = (long?)singleParameterStatistic.dieList[i];
                            var value = singleParameterStatistic.valueList[i] / kurbatovParameter.Divider;
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
            if((value >= lower || Double.IsNaN(lower)) && (value <= upper || Double.IsNaN(upper)))
            {
                return "Годен";
            }
            else
            {
                return "Брак";
            }
        }

        public async Task<List<Dictionary<string, string>>> Export(int measurementRecordingId, string statNames, string delimeter, double k = 1.5)
        {
            string measurementRecordingIdAsKey = Convert.ToString(measurementRecordingId);
            var statNamesList = statNames.Split(delimeter).ToList();
            var exportList = new List<Dictionary<string, string>>();
            var stageId = (await _stageProvider.GetByMeasurementRecordingId(measurementRecordingId)).StageId;
            var dieValuesDictionary = await _dieValueService.GetDieValuesByMeasurementRecording(measurementRecordingId);
            var statDictionary = await _statisticService.GetSingleParameterStatisticByDieValues(new ConcurrentDictionary<string, List<DieValue>>(dieValuesDictionary), measurementRecordingId, stageId, 1.0, k);
            foreach (var stat in statDictionary) 
            {
                foreach (var singleParameterStatistic in stat.Value) 
                {
                    if(statNamesList.Contains(singleParameterStatistic.Name))
                    {
                        
                        var d = new Dictionary<string, string>();
                        var dieList = new List<int>();
                        d["name"] = singleParameterStatistic.Name;
                      
                        for (int i = 0; i < singleParameterStatistic.dieList.Count; i++)
                        {
                            long? die = (long?)singleParameterStatistic.dieList[i];
                            dieList.Add(Convert.ToInt32((await _dieProvider.GetById((long)die)).Code));
                        }
                        for (int i = 0; i < dieList.Count; i++)
                        {
                            d["#" + Convert.ToString(i+1)] = Convert.ToString(singleParameterStatistic.valueList[i], CultureInfo.InvariantCulture);
                        }
                        
                        exportList.Add(d);                    
                    }
                 
                }
            }
            return exportList;
        }

    
    }
}