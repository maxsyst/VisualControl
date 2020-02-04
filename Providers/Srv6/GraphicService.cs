using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VueExample.Contexts;
using VueExample.Models.SRV6;
using VueExample.Providers.Srv6.Interfaces;

namespace VueExample.Providers.Srv6
{
    public class GraphicService : ISRV6GraphicService
    {
        private readonly Srv6Context _srv6Context;
        public GraphicService(Srv6Context srv6Context)
        {
            _srv6Context = srv6Context;
        }
        public Graphic GetById(int graphicId) => _srv6Context.Graphics.FirstOrDefault(x => x.Id == graphicId);
        
        public async Task<Graphic> GetByCodeProductAndName(int codeProductId, string name)
        {
            return  await (from f in _srv6Context.CodeProductGraphic
                    join g in _srv6Context.Graphics on f.GraphicId equals g.Id
                    where g.Name == name && f.CodeProductId == codeProductId
                    select g).FirstOrDefaultAsync();
        }        
    }
}
