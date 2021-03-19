using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using VueExample.Exceptions;
using VueExample.Models.Vertx;
using VueExample.Services.Vertx.Abstract;

namespace VueExample.Services.Vertx.Implementation
{
    public class MeasurementService : IMeasurementService
    {
        private readonly IMongoCollection<Measurement> _measurementCollection;
        private readonly IMeasurementAttemptService _measurementAttemptService;

        public MeasurementService(IMongoDatabase mongoDatabase, IMeasurementAttemptService measurementAttemptService)
        {
            _measurementCollection = mongoDatabase.GetCollection<Measurement>("measurements");
            _measurementAttemptService = measurementAttemptService;
        }

        public async Task<Measurement> Create(Measurement measurement, ObjectId measurementAttemptId)
        {
            if (await _measurementAttemptService.GetById(measurementAttemptId) == null)
            {
                throw new RecordNotFoundException("MeasurementAttempt not found");
            }
            if (await GetByName(measurement.Name) != null) throw new DuplicateException("Measurement already exists");
            await _measurementCollection.InsertOneAsync(measurement);
            var isRoot = await _measurementAttemptService.PushNewMeasurement(measurementAttemptId, measurement);
            if (!isRoot)
            {
                var finishedMeasurement = await _measurementAttemptService.GetPenultMeasurementId(measurementAttemptId);
                await SetFinishDate(new ObjectId(finishedMeasurement));
            }
            return measurement;
        }

        public async Task<Measurement> GetById(ObjectId id)
        {
            return await _measurementCollection.Find(Builders<Measurement>.Filter.Where(x => x.Id == id))
                .FirstOrDefaultAsync();
        }

        public async Task<Measurement> GetByName(string name)
        {
            return await _measurementCollection.Find(Builders<Measurement>.Filter.Where(x => x.Name == name))
                .FirstOrDefaultAsync();
        }

        public async Task<List<Characteristic>> GetAllCharacteristics(ObjectId id)
        {
            var measurement = await GetById(id);
            return measurement is null ? new List<Characteristic>() : measurement.MeasurementSetPlusUnits.Select(x => x.Characteristic).ToList();
        }

        public async Task<LastUpdate> SetLastUpdate(ObjectId measurementId, LastUpdate lastUpdate)
        {
            var measurement = await GetById(measurementId);
            var filter = Builders<Measurement>.Filter.Where(x => x.Id == measurementId);
            var updateLastUpdate = Builders<Measurement>.Update.Set(x => x.LastUpdate, lastUpdate);
            if (measurement.LastUpdate != null)
            {
                var updateDuration = updateLastUpdate.Set(x => x.DurationSeconds,
                   (int)(lastUpdate.Date - measurement.CreationDate).TotalSeconds);
                await _measurementCollection.FindOneAndUpdateAsync(filter, updateDuration,
                    new FindOneAndUpdateOptions<Measurement> { IsUpsert = true });
            }
            else
            {
                var updateLastUpdateAndCreation = updateLastUpdate.Set(x => x.CreationDate, lastUpdate.Date);
                await _measurementCollection.FindOneAndUpdateAsync(filter, updateLastUpdateAndCreation,
                    new FindOneAndUpdateOptions<Measurement> { IsUpsert = true });
            }
            return lastUpdate;
        }

        public async Task<Measurement> FullUpdate(Measurement measurement)
        {
            await _measurementCollection.ReplaceOneAsync(
                x => x.Id == measurement.Id,
                measurement);
            return measurement;
        }

        public async Task<Measurement> SetFinishDate(ObjectId measurementId)
        {
            var measurement = await GetById(measurementId);
            if (measurement?.LastUpdate == null) return measurement;
            var filter = Builders<Measurement>.Filter.Where(x => x.Id == measurementId);
            var update = Builders<Measurement>.Update.Set(x => x.FinishDate, measurement.LastUpdate.Date);
            await _measurementCollection.FindOneAndUpdateAsync(filter, update,
                new FindOneAndUpdateOptions<Measurement> { IsUpsert = true });
            return measurement;
        }

        public async Task<List<Measurement>> GetByMeasurementAttemptId(ObjectId measurementAttemptId)
        {
            return await _measurementCollection.Find(Builders<Measurement>.Filter.Where(x => x.MeasurementAttemptId == measurementAttemptId))
                .ToListAsync();
        }


        public async Task<bool> Delete(ObjectId id)
        {
            var deleteResult =
                await _measurementCollection.DeleteOneAsync(
                    Builders<Measurement>.Filter.Eq(x => x.Id, id));
            return deleteResult.DeletedCount == 1;
        }
    }
}
