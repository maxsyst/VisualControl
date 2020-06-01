using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VueExample.Contexts;
using VueExample.Models;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.ResponseObjects;

namespace VueExample.Providers
{
    public class WaferProvider : IWaferProvider
    {
        private readonly Srv6Context _srv6Context;
        public WaferProvider(Srv6Context srv6Context)
        {
            _srv6Context = srv6Context;
        }
        public async Task<List<Wafer>> GetWafers() => await _srv6Context.Wafers.ToListAsync();
        public async Task<Wafer> GetByWaferId(string waferId) => await _srv6Context.Wafers.FirstOrDefaultAsync(x => x.WaferId == waferId);

        public async Task<List<PWafer>> GetPWafer()
        {
           return await _srv6Context.PWaferQuery.FromSql("SELECT A.Wafer_id as WaferId, A.Cp_Name as CodeProductName, A.Process_Name as ProcessName, A.Date_measurement as MeasurementDate , A.StartTime as StartTime, A.Name as MeasurementRecordingName FROM(SELECT DISTINCT TOP (100) PERCENT Code_product.Cp_Name, ProcessList.Process_Name, GrowWafer.StartTime, Measurement_Recording.Date_measurement, Measurement_Recording.Name, GrowWafer.Wafer_id, ROW_NUMBER() OVER (PARTITION BY FK_MR_P.Wafer_id ORDER BY Date_measurement DESC) AS RowID FROM GrowWafer INNER JOIN FK_MR_P ON GrowWafer.Wafer_id = FK_MR_P.Wafer_id INNER JOIN Measurement_Recording ON FK_MR_P.id_mr = Measurement_Recording.id_mr INNER JOIN Code_product ON GrowWafer.id_cp = Code_product.id_cp INNER JOIN ProcessList ON ProcessList.id_processlist = Code_product.id_processlist) AS A WHERE A.RowID = 1 and  A.Name is not null ORDER BY A.Date_measurement DESC").ToListAsync();
        }
    }
}
