using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VueExample.Models;

namespace VueExample.Providers
{
    public interface IWaferMapProvider
    {
        WaferMap GetByWaferId(string waferId);
    }
}