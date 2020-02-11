using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using VueExample.Entities;
using VueExample.Models.SRV6;
using VueExample.Providers.Abstract;
using VueExample.Providers.Srv6.Interfaces;

namespace VueExample.Providers.Srv6
{
    public class StandartParameterService : IStandartParameterService
    {
        private readonly IStandartParameterProvider _standartParameterProvider;
        private readonly IMapper _mapper;
        public StandartParameterService(IStandartParameterProvider standartParameterProvider, IMapper mapper)
        {
            _standartParameterProvider = standartParameterProvider;
            _mapper = mapper;
        }

        public async Task<StandartParameterModel> Create(StandartParameterModel standartParameterModel) 
            => _mapper.Map<StandartParameterEntity, StandartParameterModel>(await _standartParameterProvider.Create(_mapper.Map<StandartParameterModel, StandartParameterEntity>(standartParameterModel)));

        public async Task Delete(int standartParameterModelId) 
            => await _standartParameterProvider.Delete(standartParameterModelId);

        public async Task<IList<StandartParameterModel>> GetAll()
        {
           return _mapper.Map<IList<StandartParameterEntity>, IList<StandartParameterModel>>(await _standartParameterProvider.GetAll());
        }

        public async Task<StandartParameterModel> GetById(int standartParameterModelId) 
            => _mapper.Map<StandartParameterEntity, StandartParameterModel>(await _standartParameterProvider.GetById(standartParameterModelId));

        public async Task<StandartParameterModel> Update(StandartParameterModel standartParameterModel)
            => _mapper.Map<StandartParameterEntity, StandartParameterModel>(await _standartParameterProvider.Update(_mapper.Map<StandartParameterModel, StandartParameterEntity>(standartParameterModel)));
    }
}