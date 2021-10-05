using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Entities;
using VueExample.Models.SRV6;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface IKurbatovParameterService
    {
        Task<KurbatovParameterEntity> Create(int? bordersId, int standartParameterId, int standartMeasurementPatternId);
        Task<List<KurbatovParameterModel>> GetBySmp(int standartMeasurementPatternId);
    }
}