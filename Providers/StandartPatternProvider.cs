using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VueExample.Contexts;
using VueExample.Entities;
using VueExample.Exceptions;
using VueExample.Providers.Abstract;

namespace VueExample.Providers
{
    public class StandartPatternProvider : IStandartPatternProvider
    {
        private readonly Srv6Context _srv6Context;
        public StandartPatternProvider(Srv6Context srv6Context)
        {
            _srv6Context = srv6Context;
        }
        public async Task<IList<StandartPatternEntity>> GetByDieTypeId(int dieTypeId)
        {
          return  await _srv6Context.StandartPatterns.Where(x => x.DieTypeId == dieTypeId).ToListAsync() ?? throw new RecordNotFoundException();
        }
    }
}