using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Entities;
using VueExample.Models.SRV6;

namespace VueExample.Providers.Abstract
{
    public interface IStandartPatternProvider
    {
        Task<StandartPatternEntity> Create(StandartPattern standartPattern);
        Task<IList<StandartPatternEntity>> GetByDieTypeId(int dieTypeId);
        Task<StandartPatternEntity> GetByName(string name);
        Task<StandartPatternEntity> GetById(int patternId);
        Task Delete(int id);
    }
}