using System.Threading.Tasks;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface INormalizeService
    {
        Task NormalizeHistogram(int idmr, double mean, double stddev);
        Task CreateNewNormalizeHistogram(int idmr, int graphicId, string waferId, double mean, double stddev);
    }
}