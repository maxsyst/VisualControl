using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VueExample.Contexts;
using VueExample.Models.SRV6;
using VueExample.Providers.Srv6.Interfaces;

namespace VueExample.Providers.Srv6
{
    public class GraphicService : ISRV6GraphicService
    {
        private readonly IAppCache _appCache;        
        private readonly IServiceProvider _serviceProvider;
        public GraphicService(IAppCache appCache, IServiceProvider serviceProvider)
        {
            _appCache = appCache;
            _serviceProvider = serviceProvider;
        }        
        
        public async Task<Graphic> GetByCodeProductAndName(int codeProductId, string name) 
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var srv6Context = scope.ServiceProvider.GetRequiredService<Srv6Context>();
                return await (from f in srv6Context.CodeProductGraphic
                    join g in srv6Context.Graphics on f.GraphicId equals g.Id
                    where g.Name == name && f.CodeProductId == codeProductId
                    select g).FirstOrDefaultAsync();
            }
        }
           

        public async Task<Graphic> GetById(int graphicId)
        {
            Func<Task<Graphic>> cachedService = async () => await this.GetByIdDb(graphicId);
            return await _appCache.GetOrAddAsync($"GID_{graphicId}", cachedService);
        }

        public async Task<Graphic> GetGraphicByKeyGraphicState(string keyGraphicState)
        {
            Func<Task<Graphic>> cachedService = async () => await this.GetGraphicByKeyGraphicStateDb(keyGraphicState);
            return await _appCache.GetOrAddAsync($"GKS_{keyGraphicState}", cachedService);
        }

        private async Task<Graphic> GetByIdDb(int graphicId) 
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var srv6Context = scope.ServiceProvider.GetRequiredService<Srv6Context>();
                return await srv6Context.Graphics.FirstOrDefaultAsync(x => x.Id == graphicId);
            }
        }

        private async Task<Graphic> GetGraphicByKeyGraphicStateDb(string keyGraphicState) 
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var srv6Context = scope.ServiceProvider.GetRequiredService<Srv6Context>();
                return await this.GetByIdDb(Convert.ToInt32(keyGraphicState.Split('_').FirstOrDefault()));
            }
        }
           
    }
}
