using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models.SRV6.Export;

namespace VueExample.StatisticsCore.Abstract
{
    public interface IExportProvider
    { 
        Task<List<Dictionary<string, string>>> Export(int idmr, string statNames, string delimeter, double k);
        Task<List<string>> GetStatisticsNameByMeasurementId(int measurementRecordingId, double k);
        Task PopulateKurbatovXLSByValues(KurbatovXLS kurbatovXLS);
        
    }
}