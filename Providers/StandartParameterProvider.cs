using System.Collections;
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
    public class StandartParameterProvider : IStandartParameterProvider
    {
        private readonly Srv6Context _srv6Context;
        public StandartParameterProvider(Srv6Context srv6Context)
        {
            _srv6Context = srv6Context;
        }
        public async Task<StandartParameterEntity> Create(StandartParameterEntity standartParameterModel)
        {
            var standartParameter = standartParameterModel ?? throw new ValidationErrorException();
            _srv6Context.StandartParameters.Add(standartParameterModel);
            await _srv6Context.SaveChangesAsync();
            return standartParameter;
        }

        public async Task Delete(int standartParameterModelId)
        {
            var standartParameter = await _srv6Context.StandartParameters.FirstOrDefaultAsync(x => x.Id == standartParameterModelId) ?? throw new RecordNotFoundException();
            _srv6Context.Remove(standartParameter);
            await _srv6Context.SaveChangesAsync();
        }

        public async Task<IList<StandartParameterEntity>> GetAll()
        {
            var parametersList = await _srv6Context.StandartParameters.ToListAsync();
            if(parametersList.Any())
                return parametersList;
            else
                throw new RecordNotFoundException();
        }

        public async Task<StandartParameterEntity> GetById(int standartParameterModelId)
        {
            var standartParameter = await _srv6Context.StandartParameters.FirstOrDefaultAsync(x => x.Id == standartParameterModelId) ?? throw new RecordNotFoundException();
            return standartParameter;
        }

        public async Task<StandartParameterEntity> Update(StandartParameterEntity standartParameterModel)
        {
            var standartParameter = await _srv6Context.StandartParameters.FirstOrDefaultAsync(x => x.Id == standartParameterModel.Id) ?? throw new RecordNotFoundException();
            _srv6Context.Entry(standartParameter).CurrentValues.SetValues(standartParameterModel);
            await _srv6Context.SaveChangesAsync();
            return standartParameterModel;
        }
    }
}