using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models.SRV6;
using VueExample.StatisticsCoreRework.Models;

namespace VueExample.StatisticsCoreRework.Abstract
{
    public interface IStatisticService
    {
        Task<Dictionary<string, Dictionary<string, SingleParameterStatisticValues>>> GetSingleParameterStatisticByMeasurementRecording(int measurementRecordingId); 
        Task<Dictionary<string, SingleParameterStatisticCalculated>> GetCalculatedStatisticByMeasurementRecordingGraphicStateAndDies(int measurementRecordingId, 
                                                                                                    string keyGraphicState, double divider, List<long> dieIdList);
    }
}