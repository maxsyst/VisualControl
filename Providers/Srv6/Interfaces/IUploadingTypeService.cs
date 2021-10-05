using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models.SRV6.Uploader;
using VueExample.Models.SRV6.Uploader.Models;
using VueExample.ViewModels;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface IUploadingTypeService
    {
        Task<List<UploadingType>> GetAll();
        Task<List<Graphic4Type>> GetGraphicsByType(string type);
        Task<AvailableS2PGraphicsViewModel> GetAvailableS2PGraphics(string waferId);
    }
}