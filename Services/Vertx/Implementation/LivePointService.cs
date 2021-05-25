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
        private readonly ApplicationContext _livePointContext;
        public LivePointService(ApplicationContext applicationContext)
        {
            _livePointContext = applicationContext;
        }
        public async Task<LivePoint> Create(LivePoint livePoint)
        {
            _livePointContext.Add(livePoint);
            await _livePointContext.SaveChangesAsync();
            return livePoint;
        }

        public async Task<List<LivePointResponseModel>> GetNLastPoint(int n)
        {
            var livePoints = new List<LivePointResponseModel>();
            var points = await _livePointContext.LivePoints.OrderByDescending(x => x.Id).Take(n).ToListAsync();
            var groupedPoints = points.GroupBy(m => new {m.MeasurementName, m.CharacteristicName})
                         .Select(group => group.First())
                         .ToList();
            var characteristics = await _livePointContext.Graphic.ToListAsync();
            foreach (var point in groupedPoints)
            {
                var unit = characteristics.FirstOrDefault(x => x.Specification == point.CharacteristicName)?.Unit;
                livePoints.Add(new LivePointResponseModel(point, unit));
            }
            return livePoints;
        }
    }
}