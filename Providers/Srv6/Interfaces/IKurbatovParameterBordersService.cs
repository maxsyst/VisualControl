using System.Threading.Tasks;
using VueExample.Models.SRV6;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface IKurbatovParameterBordersService
    {
        Task<KurbatovParameterBordersModel> Create(KurbatovParameterBordersModel kurbatovBordersModel);
        Task<KurbatovParameterBordersModel> Update(KurbatovParameterBordersModel kurbatovBordersModel);
    }
}