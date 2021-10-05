using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models.SRV6;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface IGraphicProvider
    {
        Task<Graphic> Create(Graphic graphic, int codeProductId);
        Task<Graphic> GetById(int graphicId);
        Task<Graphic> GetByCodeProductAndName(int codeProductId, string name);
        Task<List<Graphic>> GetByCodeProduct(int codeProductId);
    }
}