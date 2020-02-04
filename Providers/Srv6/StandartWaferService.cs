using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VueExample.Contexts;
using VueExample.Providers.Srv6.Interfaces;

namespace VueExample.Providers.Srv6
{
    public class StandartWaferService : IStandartWaferService
    {
        private readonly Srv6Context _srv6Context;
        public StandartWaferService(Srv6Context srv6Context)
        {
            _srv6Context = srv6Context;
        }
        
        public async Task<string> GetCodeFromStandartWafer(string code, string map) 
            =>  (await _srv6Context.CodeProductStandartWafers
                        .FirstOrDefaultAsync(x => x.Id == _srv6Context.MapStandartWafers.FirstOrDefault(m => m.NewCode == code && m.MapName == map).Idfk)).Code;
    }
}