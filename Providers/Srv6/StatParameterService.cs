using VueExample.Contexts;
using System.Linq;
using VueExample.Models.SRV6;

namespace VueExample.Providers.Srv6
{
    public class StatParameterService
    {
        private readonly Srv6Context _srv6Context;
        public StatParameterService(Srv6Context srv6Context)
        {
            _srv6Context = srv6Context;
        }
        public StatParameterForStage GetByStatParameterIdAndStageId(int statisticParameterId, int? stageId) 
            => _srv6Context.StatParametersForStage.FirstOrDefault(x => x.StageId == stageId && x.StatisticParameterId == statisticParameterId);
    }
}