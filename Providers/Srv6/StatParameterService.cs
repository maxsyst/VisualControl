using VueExample.Contexts;
using VueExample.Models.SRV6;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace VueExample.Providers.Srv6
{
    public class StatParameterService
    {
        private readonly IServiceProvider _services;
        public StatParameterService(IServiceProvider services)
        {
           _services = services;
        }
        public async Task<StatParameterForStage> GetByStatParameterIdAndStageId(int statisticParameterId, int? stageId) 
        {
            using (var scope = _services.CreateScope())
            {
                var srv6Context = scope.ServiceProvider.GetRequiredService<Srv6Context>();
                return await srv6Context.StatParametersForStage.FirstOrDefaultAsync(x => x.StageId == stageId && x.StatisticParameterId == statisticParameterId);
            }
           
        }
           
    }
}