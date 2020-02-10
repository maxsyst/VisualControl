using System.Threading.Tasks;
using VueExample.Contexts;
using VueExample.Entities;
using VueExample.Providers.Abstract;

namespace VueExample.Providers
{
    public class StandartParameterProvider : IStandartParameterProvider
    {
        private readonly Srv6Context _srv6Context;
        public StandartParameterProvider(Srv6Context srv6Context)
        {
            _srv6Context = srv6Context;
        }
        public Task<StandartParameterEntity> Create(StandartParameterEntity standartParameterModel)
        {
            throw new System.NotImplementedException();
        }

        public Task Delete(int standartParameterModelId)
        {
            throw new System.NotImplementedException();
        }

        public Task<StandartParameterEntity> GetById(int standartParameterModelId)
        {
            throw new System.NotImplementedException();
        }

        public Task<StandartParameterEntity> GetByProcess(int standartParameterModelId)
        {
            throw new System.NotImplementedException();
        }

        public Task<StandartParameterEntity> Update(StandartParameterEntity standartParameterModel)
        {
            throw new System.NotImplementedException();
        }
    }
}