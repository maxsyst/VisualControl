using System.Threading.Tasks;
using VueExample.Models;

namespace VueExample.Providers
{
    public interface IWaferMapProvider
    {
        Task<WaferMap> GetByWaferId(string waferId);
    }
}
