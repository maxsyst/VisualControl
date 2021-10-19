using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using VueExample.Models.Vertx;
using VueExample.Services.Vertx.Abstract;

namespace VueExample.Services.Vertx.Implementation
{
    public class MeasurementSetService : IMeasurementSetService
    {
        private readonly IMongoCollection<Measurement> _measurementCollection;
        private readonly IMongoCollection<MeasurementSet> _measurementSetsCollection;
        private readonly IMeasurementSetPlusUnitService _measurementSetPlusUnitService;
        public MeasurementSetService(IMongoClient mongoClient, IMeasurementSetPlusUnitService measurementSetPlusUnitService)
        {
            _measurementSetsCollection = mongoClient.GetDatabase("vertx_excel").GetCollection<MeasurementSet>("measurement_sets");
            _measurementCollection = mongoClient.GetDatabase("vertx_excel").GetCollection<Measurement>("measurements");
            _measurementSetPlusUnitService = measurementSetPlusUnitService;
        }
        public async Task<MeasurementSet> Create(ObjectId measurementId, string measurementSetPlusUnitId)
        {
            var measurementSet = new MeasurementSet(measurementSetPlusUnitId);
            await _measurementSetsCollection.InsertOneAsync(measurementSet);
            var filter = Builders<Measurement>.Filter.And(
                Builders<Measurement>.Filter.Where(x => x.Id == measurementId),
                Builders<Measurement>.Filter.ElemMatch(x => x.MeasurementSetPlusUnits,
                    c => c.GeneratedId == measurementSetPlusUnitId));
            var update =
                Builders<Measurement>.Update.Push(x => x.MeasurementSetPlusUnits[-1].MeasurementSetIds, measurementSet.Id.ToString());
            await _measurementCollection.FindOneAndUpdateAsync(filter, update);
            return measurementSet;
        }
        public async Task UpdatePoints(ObjectId id, List<Point> pointsList)
        {
            await _measurementSetsCollection.UpdateOneAsync(Builders<MeasurementSet>.Filter.Where(x => x.Id == id),
                Builders<MeasurementSet>.Update.PushEach(x => x.Points, pointsList),
                new UpdateOptions { IsUpsert = true });

        }
        public async Task<MeasurementSet> GetById(ObjectId id)
        {
            return await _measurementSetsCollection
                .Find(Builders<MeasurementSet>.Filter.Where(x => x.Id == id))
                .FirstOrDefaultAsync();
        }
        public async Task<MeasurementSet> GetLastMeasurementSet(ObjectId measurementId, string measurementSetPlusUnitId)
        {
            var measurementSetsIds = await GetAllByMeasurementIdsSetPlusUnitId(measurementId, measurementSetPlusUnitId);
            if (measurementSetsIds.Count > 0)
            {
                return await GetById(new ObjectId(measurementSetsIds.Last()));
            }
            return null;
        }
        public async Task<IList<string>> GetAllByMeasurementIdsSetPlusUnitId(ObjectId measurementId, string measurementSetPlusUnitId)
        {
            var measurement = await _measurementCollection
                .Find(Builders<Measurement>.Filter.Where(x => x.Id == measurementId))
                .FirstOrDefaultAsync();
            var measurementSetPlusUnits =
                measurement.MeasurementSetPlusUnits.FirstOrDefault(x => x.GeneratedId == measurementSetPlusUnitId);
            return measurementSetPlusUnits != null ? measurementSetPlusUnits.MeasurementSetIds : new List<string>();
        }
    }
}
