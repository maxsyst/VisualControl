using System.Threading.Tasks;
using VueExample.Entities;

namespace VueExample.Providers.Abstract
{
    public interface IKurbatovParameterProvider
    {
        Task<KurbatovParameterEntity> Create(int? bordersId, int standartParameterId, int standartMeasurementPatternId);
    }
}