using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VueExample.Contexts;
using VueExample.Models;

namespace VueExample.Providers
{
    public class BasicGraphicProvider : IGraphicProvider
    {
        public Graphic GetGraphicById(int id)
        {
            using (ApplicationContext appContext = new ApplicationContext())
            {
                return appContext.Graphic.FirstOrDefault(x => x.GraphicId == id);
            }
        }
    }
}
