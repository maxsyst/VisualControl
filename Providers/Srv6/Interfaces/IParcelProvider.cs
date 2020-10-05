using System.Threading.Tasks;
using VueExample.Models.SRV6;
using VueExample.ViewModels;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface IParcelProvider
    {
        Task<ParcelViewModel> GetById(int id);
        Task<ParcelViewModel> GetByWaferId(string waferId);
    }
}