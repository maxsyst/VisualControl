using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VueExample.Contexts;
using VueExample.Entities;
using VueExample.Models.SRV6;
using VueExample.Providers.Srv6.Interfaces;

namespace VueExample.Providers
{
    public class GraphicProvider : IGraphicProvider
    {   
        private readonly Srv6Context _srv6Context;
        public GraphicProvider(Srv6Context srv6Context)
        {
            _srv6Context = srv6Context;
        }

        public async Task<Graphic> Create(Graphic graphic, int codeProductId)
        {
            _srv6Context.Graphics.Add(graphic);
            await _srv6Context.SaveChangesAsync();
            _srv6Context.CodeProductGraphic.Add(new CodeProductGraphic{CodeProductId = codeProductId, GraphicId = graphic.Id});
            await _srv6Context.SaveChangesAsync();
            return graphic;
        }

        public async Task<List<Graphic>> GetByCodeProduct(int codeProductId)
        {
              return await (from f in _srv6Context.CodeProductGraphic
                    join g in _srv6Context.Graphics on f.GraphicId equals g.Id
                    where f.CodeProductId == codeProductId
                    select g).ToListAsync();
        }

        public async Task<Graphic> GetByCodeProductAndName(int codeProductId, string name)
        {
              return await (from f in _srv6Context.CodeProductGraphic
                    join g in _srv6Context.Graphics on f.GraphicId equals g.Id
                    where g.Name == name && f.CodeProductId == codeProductId
                    select g).FirstOrDefaultAsync();
        }

        public async Task<Graphic> GetById(int graphicId)
        {
            return await _srv6Context.Graphics.FirstOrDefaultAsync(x => x.Id == graphicId);
        }
    }
}