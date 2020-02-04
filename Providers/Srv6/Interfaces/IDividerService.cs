using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models.SRV6;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface IDividerService
    {
        Task<List<Divider>> GetAll();
    }
}