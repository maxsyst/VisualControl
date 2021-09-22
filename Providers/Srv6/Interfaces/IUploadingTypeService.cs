using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models.SRV6.Uploader;
using VueExample.ViewModels;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface IUploadingTypeService
    {
        Task<List<UploadingType>> GetAll();
        Task<AvailableS2PGraphicsViewModel> GetAvailableS2PGraphics(string waferId);
    }
}