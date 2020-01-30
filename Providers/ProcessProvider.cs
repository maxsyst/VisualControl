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
        public async Task<Process> GetProcessByCodeProductId(int codeProductId)
        {
            using (Srv6Context srv6Context = new Srv6Context())
            {
                return await srv6Context.CodeProducts.Join(
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
    }
}
