using System.Threading.Tasks;
using VueExample.Entities;
using VueExample.Models.SRV6;
using VueExample.ResponseObjects;
using VueExample.ViewModels;

namespace VueExample.Providers.Abstract
{
    public interface IShortLinkProvider
    {
        Task<AfterDbManipulationObject<ShortLinkInfoViewModel>> GetElementExportDetails(string shortLink);
        string Obfuscate(string str);
        string Deobfuscate(string str);
        Task<ShortLinkEntity> Create(string fullLink);
        Task<ShortLink> GetByGeneratedId(string generatedId);
    }
}