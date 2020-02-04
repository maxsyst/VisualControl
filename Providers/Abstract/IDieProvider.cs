using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Entities;
using VueExample.Models;
using VueExample.Repository;

namespace VueExample.Providers
{
    public interface IDieProvider
    {
        List<Die> GetDiesByWaferId(string waferId);
        Task<Die> GetByWaferIdAndCode(string waferId, string code);
        Task<DieParameterOld> GetOrAddDieParameter(long dieId, int measurementRecordingId, int parameterId = 247, string value = "0");
        Task<Die> GetById(long dieId);
        Task<List<Die>> GetAll();
    }
}
