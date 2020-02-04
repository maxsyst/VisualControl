using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VueExample.Contexts;
using VueExample.Models;
using VueExample.Providers.Srv6.Interfaces;

namespace VueExample.Providers
{
    public class WaferProvider : IWaferProvider
    {
        private readonly Srv6Context _srv6Context;
        public WaferProvider(Srv6Context srv6Context)
        {
            _srv6Context = srv6Context;
        }
        public async Task<List<Wafer>> GetWafers() => await _srv6Context.Wafers.ToListAsync();
        public async Task<Wafer> GetByWaferId(string waferId) => await _srv6Context.Wafers.FirstOrDefaultAsync(x => x.WaferId == waferId);
    }
}
