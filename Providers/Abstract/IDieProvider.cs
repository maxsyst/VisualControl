using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Entities;
using VueExample.Models;

namespace VueExample.Providers
{
    public interface IDieProvider
    {
        Task<List<Die>> GetDiesByWaferId(string waferId);
        Task<Die> GetByWaferIdAndCode(string waferId, string code);
        Task<DieParameterOld> GetOrAddDieParameter(long dieId, int measurementRecordingId, short parameterId = 247, string value = "0");
        Task<List<DieParameterOld>> GetOrAddDieParameters(List<long> dieIdList, int measurementRecordingId, short parameterId = 247, string value = "0");
        Task<Die> GetById(long dieId);
        Task<List<Die>> GetAll();
    }
}
