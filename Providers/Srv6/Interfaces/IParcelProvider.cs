using System.Threading.Tasks;
using VueExample.Models.SRV6;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface IParcelProvider
    {
        Task<Parcel> GetById(int id);
        Task<Parcel> GetByWaferId(string waferId);
    }
}