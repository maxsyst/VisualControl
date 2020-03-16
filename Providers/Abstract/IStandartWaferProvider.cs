using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Entities;
using VueExample.ViewModels;

namespace VueExample.Providers.Abstract
{
    public interface IStandartWaferProvider
    {
        Task<List<CodeProductStandartWafer>> GetByCodeProduct(int codeProductId);
        Task<List<CodeProductStandartWafer>> Create(List<CodeProductStandartWafer> standartWafers);
        Task Delete(int codeProductId);

    }
}