using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VueExample.Contexts;
using VueExample.Models.Vertx;
using VueExample.Services.Vertx.Abstract;

namespace VueExample.Services.Vertx.Implementation
{
    public class LivePointService : ILivePointService
    {
        private readonly LivePointContext _livePointContext;
        public LivePointService(LivePointContext livePointContext)
        {
            _livePointContext = livePointContext;
        }
        public async Task<LivePoint> Create(LivePoint livePoint)
        {
            _livePointContext.Add(livePoint);
            await _livePointContext.SaveChangesAsync();
            return livePoint;
        }

        public async Task<List<LivePoint>> GetNLastPoint(int n)
        {
            return await _livePointContext.LivePoints.OrderBy(x => x.Id).TakeLast(n).ToListAsync();
        }
    }
}