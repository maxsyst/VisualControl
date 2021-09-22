using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models.SRV6;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface ISRV6GraphicService
    {
        Task<Graphic> Create(Graphic graphic, int codeProductId);
        Task<Graphic> CreateS2P(int codeProductId, string type);
        Task<Graphic> GetByCodeProductAndName(int codeProductId, string name);
        Task<Graphic> GetGraphicByKeyGraphicState(string keyGraphicState);
        Task<Graphic> GetById(int graphicId);
        Task<List<Graphic>> GetByCodeProduct(int codeProductId);
    }
}