using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models.SRV6;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface IStatParameterService
    {
        Task<List<StatParameterForStage>> GetByStageId(int? stageId);
        Task<List<StatisticParameter>> GetAllParametersByStageId(int? stageId); 
        Task<StatParameterForStage> GetByStatParameterIdAndStageId(int statisticParameterId, int? stageId);
    }
}