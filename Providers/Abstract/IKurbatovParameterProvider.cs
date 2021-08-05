using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Entities;

namespace VueExample.Providers.Abstract
{
    public interface IKurbatovParameterProvider
    {
        Task<KurbatovParameterEntity> Create(int? bordersId, int standartParameterId, int standartMeasurementPatternId);
        Task<List<KurbatovParameterEntity>> GetBySmp(int standartMeasurementPatternId);
    }
}