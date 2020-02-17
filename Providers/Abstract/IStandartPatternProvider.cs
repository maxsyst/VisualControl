using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Entities;

namespace VueExample.Providers.Abstract
{
    public interface IStandartPatternProvider
    {
        Task<IList<StandartPatternEntity>> GetByDieTypeId(int dieTypeId);
        Task Delete(int id);
    }
}