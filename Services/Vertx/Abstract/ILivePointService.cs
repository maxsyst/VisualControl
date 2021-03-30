using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models.Vertx;

namespace VueExample.Services.Vertx.Abstract
{
    public interface ILivePointService
    {
        Task<LivePoint> Create(LivePoint livePoint);
        Task<List<LivePoint>> GetNLastPoint(int n);
    }
}