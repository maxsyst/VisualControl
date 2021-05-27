using System.Globalization;
using System;
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
        private readonly IMeasurementService _measurementService;
        public LivePointService(ApplicationContext applicationContext, IMeasurementService measurementService)
        {
            _livePointContext = applicationContext;
            _measurementService = measurementService;
        }

        protected string GetFormat(double number)
        {
            if(double.Equals(number, 0.0)) {
                return "0";
            }
            if (Math.Abs(number) < 1E-22 || Math.Abs(number) > 1E22)
            {
                return String.Empty;
            }
            if ((Math.Abs(number) >= 10000 || Math.Abs(number) < 1E-2) && Math.Abs(number - 0) > 1E-20)
            {
                return number.ToString("0.00E0", CultureInfo.InvariantCulture);
            }
            return number.ToString("0.000", CultureInfo.InvariantCulture);
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
            var measurements = new List<Measurement>();
            var measurementNamesList = groupedPoints.Select(x => x.MeasurementName).ToList();
            var tasks = measurementNamesList.Select(async (x) => {
                measurements.Add(await _measurementService.GetByName(x));
            });
            await Task.WhenAll(tasks);          
            foreach (var point in groupedPoints)
            {
                var unit = characteristics.FirstOrDefault(x => x.Specification == point.CharacteristicName)?.Unit;
                if(point.CharacteristicName == "Pout") {
                    var currentMeasurement = measurements.FirstOrDefault(x => x.Name == point.MeasurementName);
                    if(currentMeasurement != null) {
                        var currentPointId = groupedPoints.FirstOrDefault(x => x.MeasurementName == currentMeasurement.Name && x.CharacteristicName == "Id");
                        if(currentPointId != null && Convert.ToDouble(currentPointId.Value, CultureInfo.InvariantCulture) > 0) {
                            var PoutWatts = Math.Pow(10, Convert.ToDouble(point.Value, CultureInfo.InvariantCulture) / 10) / 1000; 
                            var drEffPoint = new LivePointResponseModel{
                                Id = Convert.ToInt64(Math.Pow(2, 32)) - point.Id,
                                Value = Convert.ToString(PoutWatts / (Convert.ToDouble(currentMeasurement.Vpower, CultureInfo.InvariantCulture) * Convert.ToDouble(currentPointId.Value, CultureInfo.InvariantCulture)) * 100, CultureInfo.InvariantCulture),
                                CharacteristicName = "Dr.Eff",
                                CharacteristicUnit = "%",
                                Date = point.Date,
                                MeasurementName = point.MeasurementName
                            };
                            livePoints.Add(drEffPoint);
                        }
                    }
                }
                livePoints.Add(new LivePointResponseModel(point, unit));
            }
            livePoints.ForEach(x => x.Value = GetFormat(Convert.ToDouble(x.Value, CultureInfo.InvariantCulture)));
            return livePoints;
        }
    }
}