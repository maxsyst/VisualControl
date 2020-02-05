using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VueExample.Exceptions;
using VueExample.Models.SRV6;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.ViewModels;

namespace VueExample.Providers.Srv6
{
    public class SpecificElementTypeService : ISpecificElementTypeService
    {
        private readonly ISpecificElementTypeProvider _specificElementProvider;
        public SpecificElementTypeService(ISpecificElementTypeProvider specificElementProvider)
        {
            _specificElementProvider = specificElementProvider;
        }
        public async Task<SpecificElementType> Create(SpecificElementTypeViewModel specificElementType)
        {
            return await _specificElementProvider.Create(specificElementType);
        }

        public async Task Delete(int id)
        {
            await GetById(id);
            await _specificElementProvider.Delete(id);
        }

        public async Task<IEnumerable<SpecificElementType>> GetByElementTypeId(int elementTypeId)
        {
            var specificElementTypeEnumerable = await _specificElementProvider.GetByElementTypeId(elementTypeId);
            if(!specificElementTypeEnumerable.Any())
                throw new RecordNotFoundException();
            return specificElementTypeEnumerable;
        }

        public async Task<SpecificElementType> GetById(int id)
        {
            return await _specificElementProvider.GetById(id);
        }

        public async Task<SpecificElementType> Update(SpecificElementTypeViewModel specificElementType)
        {
            return await _specificElementProvider.Update(specificElementType);
        }
    }
}