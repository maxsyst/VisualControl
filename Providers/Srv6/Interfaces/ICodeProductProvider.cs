using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface ICodeProductProvider
    {
        Task<List<CodeProduct>> GetAll();
        Task<CodeProduct> GetByWaferId(string waferId);
        Task<CodeProduct> GetByName(string name);
        Task<IList<CodeProduct>> GetByProcessId(int processId);
        Task<List<CodeProduct>> GetCodeProductsByDieType(int dieTypeId);
    }
}