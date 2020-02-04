using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VueExample.Contexts;
using VueExample.Entities;
using VueExample.Models;

namespace VueExample.Providers
{
    public class DieProvider : IDieProvider
    {
        private readonly Srv6Context _srv6Context;
        public DieProvider(Srv6Context srv6Context)
        {
            _srv6Context = srv6Context;
        }
        public async Task<DieParameterOld> GetOrAddDieParameter(long dieId, int measurementRecordingId, int parameterId = 247, string value = "0")
        {
            var dieParameter = await  _srv6Context.DiesParameterOld.FirstOrDefaultAsync(x => x.MeasurementRecordingId == measurementRecordingId && x.DieId == dieId && x.Id247 == parameterId);
            if(dieParameter is null)
            {
                dieParameter = new DieParameterOld{DieId = dieId, MeasurementRecordingId = measurementRecordingId, Id247 = parameterId, Value = value};
                _srv6Context.DiesParameterOld.Add(dieParameter);
                await _srv6Context.SaveChangesAsync();
            }
            return dieParameter;
        }

        public async Task<Die> GetByWaferIdAndCode(string waferId, string code) 
            => await _srv6Context.Dies.FirstOrDefaultAsync(x => x.WaferId == waferId && x.Code == code);

        public List<Die> GetDiesByWaferId(string waferId) 
            => _srv6Context.Dies.Where(x => x.WaferId == waferId).ToList();

        public async Task<Die> GetById(long dieId) => await _srv6Context.Dies.FirstOrDefaultAsync(x => x.DieId == dieId);
        public async Task<List<Die>> GetAll() => await _srv6Context.Dies.ToListAsync();
    }
}
