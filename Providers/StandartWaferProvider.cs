using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VueExample.Contexts;
using VueExample.Entities;
using VueExample.Providers.Abstract;

namespace VueExample.Providers
{
    public class StandartWaferProvider : IStandartWaferProvider
    {
        private readonly Srv6Context _srv6Context;
        public StandartWaferProvider(Srv6Context srv6Context)
        {
            _srv6Context = srv6Context;
        }

        public async Task<List<CodeProductStandartWafer>> Create(List<CodeProductStandartWafer> standartWafers)
        {
            _srv6Context.CodeProductStandartWafers.AddRange(standartWafers);
            await _srv6Context.SaveChangesAsync();
            return standartWafers;
        }

        public async Task Delete(int codeProductId)
        {
            _srv6Context.CodeProductStandartWafers.RemoveRange(_srv6Context.CodeProductStandartWafers.Where(x => x.CodeProductId == codeProductId));
            await _srv6Context.SaveChangesAsync()
        }

        public async Task<List<CodeProductStandartWafer>> GetByCodeProduct(int codeProductId)
        {
            return await _srv6Context.CodeProductStandartWafers.Where(x => x.CodeProductId == codeProductId).ToListAsync();
        }
    }
}