using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Entities;
using VueExample.Models.SRV6;

namespace VueExample.Providers.Abstract
{
    public interface IStandartMeasurementPatternProvider
    {
        Task<StandartMeasurementPatternEntity> Create(StandartMeasurementPatternModel standartMeasurementPatternModel);
        Task<List<StandartMeasurementPatternEntity>> CreateFull(List<StandartMeasurementPatternEntity> smpList);
        Task Delete(int standartMeasurementPatternId);
    }
}