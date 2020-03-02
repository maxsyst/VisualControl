using System.Threading.Tasks;
using VueExample.Entities;
using VueExample.Models.SRV6;

namespace VueExample.Providers.Abstract
{
    public interface IKurbatovParameterBordersProvider
    {
        Task<KurbatovParameterBordersEntity> Create(KurbatovParameterBordersModel kurbatovBordersModel);
        Task<KurbatovParameterBordersEntity> Update(KurbatovParameterBordersModel kurbatovBordersModel);
    }
}