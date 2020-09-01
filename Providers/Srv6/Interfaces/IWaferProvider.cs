using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models;
using VueExample.ResponseObjects;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface IWaferProvider
    {
        Task<List<Wafer>> GetWafers();
        Task<List<PWafer>> GetPWafer();
        Task<Wafer> GetByWaferId(string waferId);

    }
}