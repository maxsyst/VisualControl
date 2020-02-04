using System.Linq;
using VueExample.Contexts;
using VueExample.Models;
using VueExample.Repository;

namespace VueExample.Providers
{
    public class WaferMapProvider : IWaferMapProvider
    {
        private readonly Srv6Context _srv6Context;
        public WaferMapProvider(Srv6Context srv6Context)
        {
            _srv6Context = srv6Context;
        }
        public WaferMap GetByWaferId(string waferId) => _srv6Context.WaferMaps.FirstOrDefault(x => x.WaferId == waferId);
    }
}
