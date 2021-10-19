using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models.SRV6;
using VueExample.ViewModels;
using VueExample.ViewModels.DieType;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface IDieTypeProvider
    {
        Task<List<DieType>> GetAll();
        Task<DieType> GetByName(string name);
        Task<List<DieType>> GetByWaferId(string waferId);
        Task<DieTypeUpdatingViewModel> GetCodeProductsAndElements(int id);
        Task<DieType> Create(DieTypeUpdatingViewModel dieTypeViewModel);
        Task<DieType> Update(DieTypeViewModel dieTypeViewModel);
        Task<List<DieType>> GetByCodeProductId(int codeProductId);
        Task<Tuple<CodeProductViewModel, string>> UpdateCodeProductsMap(int dieTypeId, int codeProductId);

    }
}