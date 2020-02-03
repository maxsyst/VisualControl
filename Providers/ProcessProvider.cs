using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VueExample.Contexts;
using VueExample.Exceptions;
using VueExample.Models;
using VueExample.Providers.Srv6.Interfaces;

namespace VueExample.Providers
{
    public class ProcessProvider : IProcessProvider
    {
        private readonly ICodeProductProvider _codeProductProvider;
        public ProcessProvider(ICodeProductProvider codeProductProvider)
        {
            _codeProductProvider = codeProductProvider;
        }
        public async Task<Process> GetProcessByCodeProductId(int codeProductId)
        {
            using (Srv6Context srv6Context = new Srv6Context())
            {
                return await srv6Context.CodeProducts.Where(x => x.IdCp == codeProductId).Join(
                    srv6Context.Processes,
                    o => o.ProcessId,
                    i => i.ProcessId,
                    (o,i) => i    
                ).FirstOrDefaultAsync() ?? throw new RecordNotFoundException();
            }
        }

        public async Task<List<Process>> GetAll() 
        {
            using (Srv6Context srv6Context = new Srv6Context())
            {
                return await srv6Context.Processes.ToListAsync();
            }   
        }

        public async Task<Process> GetByWaferId(string waferId)
        {
            var codeProduct = await _codeProductProvider.GetByWaferId(waferId) ?? throw new RecordNotFoundException();
            return await GetProcessByCodeProductId(codeProduct.IdCp) ?? throw new RecordNotFoundException();
        }
    }
}
