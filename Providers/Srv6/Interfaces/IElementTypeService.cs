using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models.SRV6;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface IElementTypeService
    {
        Task<ElementType> GetById(int id);
        Task<IEnumerable<ElementType>> GetAll();
        Task<ElementType> Create(string name);
        Task<ElementType> Update(ElementType elementType);
    }
}