using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models.Vertx;

namespace VueExample.Services.Vertx.Abstract
{
    public interface IAggregationService
    {
        Task<List<LastMeasurementMdv>> GetNLastMeasurementAttemptsWithMdv(int n);
    }
}
