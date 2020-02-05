using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models.SRV6;
using VueExample.ViewModels;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface ISpecificElementTypeService
    {
        Task<SpecificElementType> GetById(int id);
        Task<IEnumerable<SpecificElementType>> GetByElementTypeId(int elementTypeId);
        Task<SpecificElementType> Create(SpecificElementTypeViewModel specificElementType);
        Task<SpecificElementType> Update(SpecificElementTypeViewModel specificElementType);
        Task Delete(int id);
    }
}