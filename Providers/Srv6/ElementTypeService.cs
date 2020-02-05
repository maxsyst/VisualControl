using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models.SRV6;
using VueExample.Providers.Abstract;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.Exceptions;

namespace VueExample.Providers.Srv6
{
    public class ElementTypeService : IElementTypeService
    {
        private readonly IElementTypeProvider _elementTypeProvider;
        public ElementTypeService(IElementTypeProvider elementTypeProvider)
        {
            _elementTypeProvider = elementTypeProvider;
        }

        public async Task<ElementType> Create(string name)
        {
            if(String.IsNullOrEmpty(name))
                throw new ValidationErrorException();
            var elementType = await _elementTypeProvider.Create(name);
            return elementType;
        }

        public async Task<IEnumerable<ElementType>> GetAll()
        {
            var elementTypeEnumerable = await _elementTypeProvider.GetAll();
            if(!elementTypeEnumerable.Any())
                throw new RecordNotFoundException();
            return elementTypeEnumerable;
        }

        public async Task<ElementType> GetById(int id) => await _elementTypeProvider.GetById(id) ?? throw new RecordNotFoundException();
        public async Task<ElementType> Update(ElementType elementType) => await _elementTypeProvider.Update(elementType);
    }
}