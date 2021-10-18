using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models.SRV6.Export;

namespace VueExample.StatisticsCoreRework.Abstract
{
    public interface IExportProvider
    {
        Task<List<Dictionary<string, string>>> Export(int idmr, string statNames, string delimeter);
        Task<List<string>> GetStatisticsNameByMeasurementId(int measurementRecordingId);
        Task PopulateKurbatovXLSByValues(KurbatovXLS kurbatovXLS);
    }
}