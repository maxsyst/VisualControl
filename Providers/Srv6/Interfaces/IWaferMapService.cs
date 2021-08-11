using System.Threading.Tasks;
using VueExample.Models;
using VueExample.ViewModels;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface IWaferMapService
    {
        Task<FormedMapViewModel> GetFormedMap(WaferMapFieldViewModel waferMapFieldViewModel);
    }
}