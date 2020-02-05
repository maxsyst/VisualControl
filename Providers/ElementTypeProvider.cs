using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VueExample.Contexts;
using VueExample.Exceptions;
using VueExample.Models.SRV6;
using VueExample.Providers.Abstract;

namespace VueExample.Providers
{
    public class ElementTypeProvider : IElementTypeProvider
    {
        private readonly Srv6Context _srv6Context;
        public ElementTypeProvider(Srv6Context srv6Context)
        {
            _srv6Context = srv6Context;
        }
        public async Task<ElementType> Create(string name)
        {
            var elementType = new ElementType{Name = name};
            _srv6Context.ElementTypes.Add(elementType);
            await _srv6Context.SaveChangesAsync();
            return elementType;
        }

        public async Task<IEnumerable<ElementType>> GetAll() => await _srv6Context.ElementTypes.ToListAsync();
        public async Task<ElementType> GetById(int id) => await _srv6Context.ElementTypes.FirstOrDefaultAsync(x => x.Id == id) ?? throw new RecordNotFoundException();
        public async Task<ElementType> Update(ElementType elementType)
        {
            var elementTypeUpdate = await _srv6Context.ElementTypes.FirstOrDefaultAsync(x => x.Id == elementType.Id) ?? throw new RecordNotFoundException();
            elementTypeUpdate.Name = elementType.Name;
            _srv6Context.Update(elementTypeUpdate);
            await _srv6Context.SaveChangesAsync();
            return elementTypeUpdate;
        }
    }
}