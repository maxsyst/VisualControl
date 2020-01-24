using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VueExample.Contexts;
using VueExample.Models;
using VueExample.Providers.Srv6.Interfaces;

namespace VueExample.Providers
{
    public class CodeProductProvider : ICodeProductProvider
    {
        public async Task<CodeProduct> GetByWaferId(string waferId)
        {
            using (var srv6Context = new Srv6Context())
            {
                var wafer = srv6Context.Wafers.FirstOrDefault(x => x.WaferId == waferId);
                return wafer != null ? await srv6Context.CodeProducts.FindAsync(wafer.CodeProductId) : null;
            }
        }

        public async Task<CodeProduct> GetByName(string name) 
        {
            using (var srv6Context = new Srv6Context())
            {
                return await srv6Context.CodeProducts.FirstOrDefaultAsync(x => x.CodeProductName == name);
            }
        }

        public async Task<IList<CodeProduct>> GetByProcessId(int processId) 
        {
            using (var srv6Context = new Srv6Context())
            {
                return await srv6Context.CodeProducts.Where(x => x.ProcessId == processId).ToListAsync();
            }
        }

        public async Task<List<CodeProduct>> GetCodeProductsByDieType(int dieTypeId) 
        {
            using (var srv6Context = new Srv6Context())
            {
                return await srv6Context.DieTypes.Join(srv6Context.DieTypeCodeProducts, c => c.DieTypeId, p => p.DieTypeId, (c,p) => p.CodeProduct).ToListAsync();
            }
        }

        public async Task<List<CodeProduct>> GetAll()
        {
            using (var srv6Context = new Srv6Context())
            {
                return await srv6Context.CodeProducts.ToListAsync();
            }
        }
    }
}
