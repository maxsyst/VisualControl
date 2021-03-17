using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using VueExample.Exceptions;
using VueExample.Models.Vertx;
using VueExample.Services.Vertx.Abstract;
using VueExample.ViewModels.Vertx.ResponseModels;

namespace VueExample.Services.Vertx.Implementation
{
    public class PointService : IPointService
    {
        private readonly IMongoCollection<MeasurementSet> _measurementSetCollection;
        private readonly IMeasurementAttemptService _measurementAttemptService;
        private readonly IMeasurementService _measurementService;
        private readonly IMeasurementSetPlusUnitService _measurementSetPlusUnitService;
        private readonly IMeasurementSetService _measurementSetService;


        public PointService(IMongoDatabase mongoDatabase,
            IMeasurementAttemptService measurementAttemptService,
            IMeasurementService measurementService,
            IMeasurementSetPlusUnitService measurementSetPlusUnitService,
            IMeasurementSetService measurementSetService)
        {
            _measurementAttemptService = measurementAttemptService;
            _measurementService = measurementService;
            _measurementSetPlusUnitService = measurementSetPlusUnitService;
            _measurementSetService = measurementSetService;
            _measurementSetCollection = mongoDatabase.GetCollection<MeasurementSet>("measurement_sets");
        }

        public async Task<Point> Create(double value, Characteristic characteristic, string measurementName, bool isNewSet, DateTime creationDate)
        {
            var measurement = await _measurementService.GetByName(measurementName);
            if (measurement == null)
            {
                throw new RecordNotFoundException();
            }

            var measurementSetPlusUnit =
                await _measurementSetPlusUnitService.GetByCharacteristicNameAndMeasurementId(characteristic.Name,
                    measurement.Id) ?? await _measurementSetPlusUnitService.Create(characteristic, 60, measurement.Id, creationDate);
            if (double.IsNaN(value))
            {
                return new Point();
            }

            var measurementSet = await _measurementSetService.GetLastMeasurementSet(measurement.Id,
                measurementSetPlusUnit.GeneratedId);
            var isNecessary =
                _measurementSetPlusUnitService.IsNecessaryToCreateNewMeasurementSet(isNewSet, measurementSet);
            if (isNecessary)
            {
                measurementSet = await _measurementSetService.Create(measurement.Id, measurementSetPlusUnit.GeneratedId);
            }

            var lastUpdate = new LastUpdate(value, Convert.ToDateTime(creationDate));
            await _measurementSetPlusUnitService.ChangeLastUpdate(characteristic.Name, lastUpdate, measurement.Id);
            var point = new Point(value, measurementSetPlusUnit.CreationDate, lastUpdate.Date);
            await _measurementSetCollection.UpdateOneAsync(Builders<MeasurementSet>.Filter.Where(x => x.Id == measurementSet.Id),
                Builders<MeasurementSet>.Update.Push(x => x.Points, point),
                new UpdateOptions { IsUpsert = true });
            return point;
        }

        public async Task<MeasurementResponseModelWithPoints> GetByMeasurement(ObjectId measurementId,
            string characteristicName)
        {
            var measurement = await _measurementService.GetById(measurementId);
            var measurementResponseModelWithPoints = new MeasurementResponseModelWithPoints
            {
                Id = measurement.Id.ToString(),
                Name = measurement.Name,
                CreationDate = measurement.CreationDate,
                Points = new List<Point>()
            };
            var measurementSetPlusUnit =
                measurement.MeasurementSetPlusUnits.FirstOrDefault(x => x.Characteristic.Name == characteristicName);
            if (measurementSetPlusUnit is null) return new MeasurementResponseModelWithPoints();
            foreach (var measurementSetId in measurementSetPlusUnit.MeasurementSetIds)
            {
                var measurementSet = await _measurementSetService.GetById(new ObjectId(measurementSetId));
                measurementResponseModelWithPoints.Points.AddRange(measurementSet.Points);
            }

            return measurementResponseModelWithPoints;
        }

        public async Task<Dictionary<string, MeasurementResponseModelWithPoints>> GetByMeasurement(
            ObjectId measurementId, string characteristicName, int siftedK, bool withoutBadPoints)
        {
            var pointsList = new List<Point>();
            var measurement = await _measurementService.GetById(measurementId);
            var measurementSetPlusUnit =
                measurement.MeasurementSetPlusUnits.FirstOrDefault(x => x.Characteristic.Name == characteristicName);
            if (measurementSetPlusUnit is null)
                return new Dictionary<string, MeasurementResponseModelWithPoints>
                    {{measurementId.ToString(), new MeasurementResponseModelWithPoints()}};
            foreach (var measurementSetId in measurementSetPlusUnit.MeasurementSetIds)
            {
                var measurementSet = await _measurementSetService.GetById(new ObjectId(measurementSetId));
                pointsList.AddRange(measurementSet.Points);
            }

            return new Dictionary<string, MeasurementResponseModelWithPoints>
            {
                {
                    measurementId.ToString(),
                    new MeasurementResponseModelWithPoints
                    {
                        Id = measurement.Id.ToString(),
                        Name = measurement.Name,
                        CreationDate = measurement.CreationDate,
                        Points = withoutBadPoints
                            ? RemoveBadPoints(SiftPoints(pointsList, siftedK))
                            : SiftPoints(pointsList, siftedK)
                    }
                }
            };
        }

        public async Task<Dictionary<string, MeasurementResponseModelWithPoints>> GetByMeasurementFirstSeconds(
            ObjectId measurementId, string characteristicName, int seconds)
        {
            var pointsList = new List<Point>();
            var measurement = await _measurementService.GetById(measurementId);
            var measurementSetPlusUnit =
                measurement.MeasurementSetPlusUnits.FirstOrDefault(x => x.Characteristic.Name == characteristicName);
            if (measurementSetPlusUnit is null)
                return new Dictionary<string, MeasurementResponseModelWithPoints>
                    {{measurementId.ToString(), new MeasurementResponseModelWithPoints()}};
            foreach (var measurementSetId in measurementSetPlusUnit.MeasurementSetIds)
            {
                var measurementSet = await _measurementSetService.GetById(new ObjectId(measurementSetId));
                pointsList.AddRange(measurementSet.Points);
            }

            return new Dictionary<string, MeasurementResponseModelWithPoints>
            {
                {
                    measurementId.ToString(), new MeasurementResponseModelWithPoints
                    {
                        Id = measurement.Id.ToString(),
                        Name = measurement.Name,
                        CreationDate = measurement.CreationDate,
                        Points = pointsList.Where(x => x.FromStartDate.TotalSeconds < seconds).ToList()
                    }
                }
            };
        }

        public async Task<Dictionary<string, MeasurementResponseModelWithPoints>> GetByMeasurementAttemptDuration(
            ObjectId measurementAttemptId, string characteristicName, int siftedK, bool withoutBadPoints)
        {
            var measurementIdsList = await _measurementAttemptService.GetAllMeasurementIds(measurementAttemptId);
            var pointsDictionary = new Dictionary<string, MeasurementResponseModelWithPoints>();
            var currentDuration = TimeSpan.Zero;
            ;
            foreach (var measurementId in measurementIdsList.Select(x => x.ToString()).ToList())
            {
                var measurementResponseModelWithPoints =
                    await GetByMeasurement(new ObjectId(measurementId), characteristicName);
                var pointsList = measurementResponseModelWithPoints.Points.ToList();
                var nextDuration = pointsList.Last().FromStartDate;
                foreach (var point in pointsList)
                {
                    point.FromStartDate = point.FromStartDate + currentDuration;
                }

                measurementResponseModelWithPoints.Points = pointsList.ToList();
                measurementResponseModelWithPoints.Points = withoutBadPoints
                    ? RemoveBadPoints(SiftPoints(measurementResponseModelWithPoints.Points, siftedK))
                    : SiftPoints(measurementResponseModelWithPoints.Points, siftedK);
                pointsDictionary.Add(measurementId, measurementResponseModelWithPoints);
                currentDuration = currentDuration + nextDuration;
            }

            return pointsDictionary;
        }

        public async Task<Dictionary<string, MeasurementResponseModelWithPoints>> GetByMeasurementAttemptTrueDate(
            ObjectId measurementAttemptId, string characteristicName, int siftedK, bool withoutBadPoints)
        {
            var measurementIdsList = await _measurementAttemptService.GetAllMeasurementIds(measurementAttemptId);
            var pointsDictionary = new Dictionary<string, MeasurementResponseModelWithPoints>();
            foreach (var measurementId in measurementIdsList.Select(x => x.ToString()).ToList())
            {
                var measurementResponseModelWithPoints =
                    await GetByMeasurement(new ObjectId(measurementId), characteristicName);
                measurementResponseModelWithPoints.Points = withoutBadPoints
                    ? RemoveBadPoints(SiftPoints(measurementResponseModelWithPoints.Points, siftedK))
                    : SiftPoints(measurementResponseModelWithPoints.Points, siftedK);
                pointsDictionary.Add(measurementId, measurementResponseModelWithPoints);
            }

            return pointsDictionary;
        }

        public async Task<Dictionary<string, MeasurementResponseModelWithPoints>> GetByMeasurementAttemptFirstSeconds(
            ObjectId measurementAttemptId, string characteristicName, int seconds)
        {
            var measurementIdsList = await _measurementAttemptService.GetAllMeasurementIds(measurementAttemptId);
            var pointsDictionary = new Dictionary<string, MeasurementResponseModelWithPoints>();
            foreach (var measurementId in measurementIdsList.Select(x => x.ToString()).ToList())
            {
                var measurementResponseModelWithPoints =
                    await GetByMeasurement(new ObjectId(measurementId), characteristicName);
                measurementResponseModelWithPoints.Points = measurementResponseModelWithPoints.Points
                    .Where(x => x.FromStartDate.TotalSeconds < seconds).ToList();
                pointsDictionary.Add(measurementId, measurementResponseModelWithPoints);
            }

            return pointsDictionary;
        }

        private List<Point> SiftPoints(List<Point> points, int siftedK) => siftedK == 0 ? points.ToList() : points.Where((p, index) => index % (points.Count / siftedK) == 0).ToList();

        private List<Point> RemoveBadPoints(List<Point> points)
        {
            if (points.Count < 6)
            {
                return points.ToList();
            }

            var quartile1Double = MathNet.Numerics.Statistics.Statistics.LowerQuartile(points.Select(x => x.Value));
            var quartile3Double = MathNet.Numerics.Statistics.Statistics.UpperQuartile(points.Select(x => x.Value));
            var iqr = MathNet.Numerics.Statistics.Statistics.InterquartileRange(points.Select(x => x.Value));
            return points.Where(p => p.Value > quartile1Double - 1.5 * iqr && p.Value < quartile3Double + 1.5 * iqr)
                .ToList();
        }
    }
}
