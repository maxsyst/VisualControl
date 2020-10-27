using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.ViewModels;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface IParcelProvider
    {
        Task<List<ParcelWithWafersViewModel>> GetByProcessId(int processId);
        Task<ParcelViewModel> GetById(int id);
        Task<ParcelViewModel> GetByWaferId(string waferId);
    }
}