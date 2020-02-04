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
        private readonly Srv6Context _srv6Context;
        public CodeProductProvider(Srv6Context srv6Context)
        {
            _srv6Context = srv6Context;
        }

        public async Task<CodeProduct> GetByWaferId(string waferId)
        {
            var wafer = _srv6Context.Wafers.FirstOrDefault(x => x.WaferId == waferId);
            return wafer != null ? await _srv6Context.CodeProducts.FindAsync(wafer.CodeProductId) : null;
        }

        public async Task<CodeProduct> GetByName(string name) 
            => await _srv6Context.CodeProducts.FirstOrDefaultAsync(x => x.CodeProductName == name);

        public async Task<IList<CodeProduct>> GetByProcessId(int processId) 
            => await _srv6Context.CodeProducts.Where(x => x.ProcessId == processId).ToListAsync();

        public async Task<List<CodeProduct>> GetCodeProductsByDieType(int dieTypeId) 
            => await _srv6Context.DieTypes.Join(_srv6Context.DieTypeCodeProducts, c => c.DieTypeId, p => p.DieTypeId, (c,p) => p.CodeProduct).ToListAsync();

        public async Task<List<CodeProduct>> GetAll() 
            => await _srv6Context.CodeProducts.ToListAsync();
    }
}
