using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VueExample.Contexts;
using VueExample.Models.SRV6;
using VueExample.Providers.Srv6.Interfaces;

namespace VueExample.Providers.Srv6
{
    public class ElementTypeService : IElementTypeService
    {
        private readonly Srv6Context _srv6Context;
        public ElementTypeService(Srv6Context srv6Context)
        {
            _srv6Context = srv6Context;
        }
        public async Task<IList<ElementType>> GetAll() => await _srv6Context.ElementTypes.ToListAsync();
    }
}