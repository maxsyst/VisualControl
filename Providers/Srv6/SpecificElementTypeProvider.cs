using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VueExample.Contexts;
using VueExample.Exceptions;
using VueExample.Models.SRV6;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.ViewModels;

namespace VueExample.Providers.Srv6
{
    public class SpecificElementTypeProvider : ISpecificElementTypeProvider
    {
        private readonly Srv6Context _srv6Context;
        public SpecificElementTypeProvider(Srv6Context srv6Context)
        {
            _srv6Context = srv6Context;
        }
        public async Task<SpecificElementType> Create(SpecificElementTypeViewModel specificElementTypeViewModel)
        {
            var specificElementType = new SpecificElementType{Name = specificElementTypeViewModel.Name,
                                                              ElementTypeId = specificElementTypeViewModel.ElementTypeId,
                                                              Specification = specificElementTypeViewModel.Specification};
            _srv6Context.SpecificElementTypes.Add(specificElementType);
            await _srv6Context.SaveChangesAsync();
            return specificElementType;                                       
        }

        public async Task<SpecificElementType> Update(SpecificElementTypeViewModel specificElementTypeViewModel)
        {
            var specificElementType = await _srv6Context.SpecificElementTypes.FirstOrDefaultAsync(x => x.Id == specificElementTypeViewModel.Id) ?? throw new RecordNotFoundException();
            specificElementType.Name = specificElementTypeViewModel.Name;
            specificElementType.ElementTypeId = specificElementTypeViewModel.ElementTypeId;
            specificElementType.Specification = specificElementType.Specification;
            _srv6Context.SpecificElementTypes.Update(specificElementType);
            await _srv6Context.SaveChangesAsync();
            return specificElementType;
        }

        public async Task Delete(int id)
        {
            var specificElementType = await _srv6Context.SpecificElementTypes.FirstOrDefaultAsync(x => x.Id == id) ?? throw new RecordNotFoundException();
            _srv6Context.SpecificElementTypes.Remove(specificElementType);
            await _srv6Context.SaveChangesAsync();
        }

        public async Task<IEnumerable<SpecificElementType>> GetByElementTypeId(int elementTypeId) 
            => await _srv6Context.SpecificElementTypes.Where(x => x.ElementTypeId == elementTypeId).ToListAsync();

        public async Task<SpecificElementType> GetById(int id) 
            => await _srv6Context.SpecificElementTypes.FirstOrDefaultAsync(x => x.Id == id) ?? throw new RecordNotFoundException();

    }
}