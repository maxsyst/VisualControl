using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using VueExample.Entities;
using VueExample.Models.SRV6;
using VueExample.Providers.Abstract;
using VueExample.Providers.Srv6.Interfaces;

namespace VueExample.Providers.Srv6
{
    public class StandartPatternService : IStandartPatternService
    {
        private readonly IMapper _mapper;
        private readonly IStandartPatternProvider _standartPatternProvider;
        public StandartPatternService(IStandartPatternProvider standartPatternProvider, IMapper mapper)
        {
            _standartPatternProvider = standartPatternProvider;
            _mapper = mapper;
        }

        public async Task Delete(int id)
        {
            await _standartPatternProvider.Delete(id);
        }

        public async Task<IList<StandartPattern>> GetByDieTypeId(int dieTypeId)
            =>  _mapper.Map<IList<StandartPatternEntity>, IList<StandartPattern>>(await _standartPatternProvider.GetByDieTypeId(dieTypeId));
    }
}