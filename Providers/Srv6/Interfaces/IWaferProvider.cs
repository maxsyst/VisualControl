using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface IWaferProvider
    {
        Task<List<Wafer>> GetWafers();
        Task<Wafer> GetByWaferId(string waferId);

    }
}