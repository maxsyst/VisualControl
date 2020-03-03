using System.Threading.Tasks;
using VueExample.Entities;
using VueExample.Models.SRV6;

namespace VueExample.Providers.Abstract
{
    public interface IStandartMeasurementPatternProvider
    {
        Task<StandartMeasurementPatternEntity> Create(StandartMeasurementPatternModel standartMeasurementPatternModel);
        Task Delete(int standartMeasurementPatternId);
    }
}