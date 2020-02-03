using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface IProcessProvider
    {
        Task<Process> GetProcessByCodeProductId(int codeProductId);
        Task<Process> GetByWaferId(string waferId);
        Task<List<Process>> GetAll(); 
    }
}