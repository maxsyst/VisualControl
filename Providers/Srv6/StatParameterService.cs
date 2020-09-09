using VueExample.Contexts;
using VueExample.Models.SRV6;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using VueExample.Providers.Srv6.Interfaces;

namespace VueExample.Providers.Srv6
{
    public class StatParameterService : IStatParameterService
    {
        private readonly Srv6Context _srv6Context;
        public StatParameterService(Srv6Context srv6Context)
        {
           _srv6Context = srv6Context;
        }

        public async Task<List<StatParameterForStage>> GetByStageId(int? stageId) 
        {
            if(stageId == null)
            {
                return new List<StatParameterForStage>();
            }
            else
            {
                return await _srv6Context.StatParametersForStage.Where(x => x.StageId == stageId).ToListAsync();
            }
        }

        public async Task<StatParameterForStage> GetByStatParameterIdAndStageId(int statisticParameterId, int? stageId) 
        {
            return await _srv6Context.StatParametersForStage.FirstOrDefaultAsync(x => x.StageId == stageId && x.StatisticParameterId == statisticParameterId);            
        }
           
    }
}