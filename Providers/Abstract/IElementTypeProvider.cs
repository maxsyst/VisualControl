using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models.SRV6;

namespace VueExample.Providers.Abstract
{
    public interface IElementTypeProvider
    {
        Task<ElementType> GetById(int id);
        Task<IEnumerable<ElementType>> GetAll();
        Task<ElementType> Create(string name);
        Task<ElementType> Update(ElementType elementType);
    }
}