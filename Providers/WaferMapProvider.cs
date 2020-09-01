using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VueExample.Contexts;
using VueExample.Models;

namespace VueExample.Providers
{
    public class WaferMapProvider : IWaferMapProvider
    {
        private readonly Srv6Context _srv6Context;
        public WaferMapProvider(Srv6Context srv6Context)
        {
            _srv6Context = srv6Context;
        }
        public async Task<WaferMap> GetByWaferId(string waferId) 
            => await _srv6Context.WaferMaps.FirstOrDefaultAsync(x => x.WaferId == waferId);
    }
}
