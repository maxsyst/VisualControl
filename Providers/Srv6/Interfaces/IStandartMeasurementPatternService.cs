using System.Threading.Tasks;
using VueExample.Models.SRV6;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface IStandartMeasurementPatternService
    {
        Task<StandartMeasurementPatternModel> Create(StandartMeasurementPatternModel standartMeasurementPatternModel);
        Task Delete(int standartMeasurementPatternId);
    }
}