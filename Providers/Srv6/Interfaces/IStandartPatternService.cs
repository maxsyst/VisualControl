using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models.SRV6;
using VueExample.ViewModels;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface IStandartPatternService
    {
        Task<IList<StandartPattern>> GetByDieTypeId(int dieTypeId);
        Task<StandartPattern> GetByName(string name);
        Task<StandartPattern> Create(StandartPatternViewModel standartPattern);  
        Task<StandartPattern> CreateFull(StandartMeasurementPatternFullViewModel standartMeasurementPatternFull);
        Task<StandartMeasurementPatternFullViewModel> GetFull(int patternId);
        Task Delete(int id);
    }
}