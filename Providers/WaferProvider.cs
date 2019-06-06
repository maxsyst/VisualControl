using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VueExample.Contexts;
using VueExample.Models;
using VueExample.Repository;

namespace VueExample.Providers
{
    public class WaferProvider
    {
        public List<Wafer> GetWafers()
        {
            using (Srv6Context srv6Context = new Srv6Context())
            {
                return srv6Context.Wafers.ToList();
            }
        }

        public Wafer GetByWaferId(string waferId)
        {
            using (Srv6Context srv6Context = new Srv6Context())
            {
                return srv6Context.Wafers.FirstOrDefault(x => x.WaferId == waferId);
            }
        }

       
    }
}
