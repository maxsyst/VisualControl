using System.Threading.Tasks;
using AutoMapper;
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

        public Task<StandartParameterModel> Create(StandartParameterModel standartParameterModel)
        {
            throw new System.NotImplementedException();
        }

        public Task Delete(int standartParameterModelId)
        {
            throw new System.NotImplementedException();
        }

        public Task<StandartParameterModel> GetById(int standartParameterModelId)
        {
            throw new System.NotImplementedException();
        }

        public Task<StandartParameterModel> GetByProcess(int standartParameterModelId)
        {
            throw new System.NotImplementedException();
        }

        public Task<StandartParameterModel> Update(StandartParameterModel standartParameterModel)
        {
            throw new System.NotImplementedException();
        }
    }
}