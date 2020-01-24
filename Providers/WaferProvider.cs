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
        public async Task<List<Wafer>> GetWafers()
        {
            using (Srv6Context srv6Context = new Srv6Context())
            {
                return await srv6Context.Wafers.ToListAsync();
            }
        }

        public async Task<Wafer> GetByWaferId(string waferId)
        {
            using (Srv6Context srv6Context = new Srv6Context())
            {
                return await srv6Context.Wafers.FirstOrDefaultAsync(x => x.WaferId == waferId);
            }
        }

       
    }
}
