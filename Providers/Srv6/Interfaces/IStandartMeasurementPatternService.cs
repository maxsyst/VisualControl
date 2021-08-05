using System.Threading.Tasks;
using VueExample.Models.SRV6;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface IStandartMeasurementPatternService
    {

        Task<StandartMeasurementPatternModel> Create(StandartMeasurementPatternModel standartMeasurementPatternModel);
        Task<StandartMeasurementPatternModel> GetByStageAndElementAndPattern(int stageId, int elementId, int patternId);
        Task Delete(int standartMeasurementPatternId);
    }
}