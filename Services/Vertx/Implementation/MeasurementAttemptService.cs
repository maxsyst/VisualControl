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
    public class MeasurementAttemptService : IMeasurementAttemptService
    {
        private readonly IMongoCollection<MeasurementAttempt> _measurementAttemptCollection;

        public MeasurementAttemptService(IMongoClient mongoClient)
        {
            _measurementAttemptCollection = mongoClient.GetDatabase("vertx_excel").GetCollection<MeasurementAttempt>("measurement_attempts");
        }

        public async Task<MeasurementAttempt> CreateMeasurementAttempt(MeasurementAttempt measurementAttempt)
        {
            if (measurementAttempt.Name == "master")
            {
                throw new DuplicateException();
            }

            if (await GetByNameAndMdvId(measurementAttempt.Name, measurementAttempt.MdvId.ToString()) == null)
            {
                await _measurementAttemptCollection.InsertOneAsync(measurementAttempt);
                return measurementAttempt;
            }
            throw new DuplicateException();
        }

        public async Task<MeasurementAttempt> GetById(ObjectId id)
        {
            return await _measurementAttemptCollection
                .Find(Builders<MeasurementAttempt>.Filter.Where(x => x.Id == id))
                .FirstOrDefaultAsync();
        }

        public async Task<Dictionary<string, MeasurementAttempt>> GetAllLastMeasurementIds()
        {
            var dictionary = new Dictionary<string, MeasurementAttempt>();
            foreach (var measurementAttempt in _measurementAttemptCollection.AsQueryable())
            {
                var measurementId = await GetLastMeasurementId(measurementAttempt.Id);
                if (!string.IsNullOrEmpty(measurementId))
                {
                    dictionary.Add(measurementId, measurementAttempt);
                }
            }
            return dictionary;
        }


        public async Task<List<ObjectId>> GetAllMeasurementIds(ObjectId measurementAttemptId)
        {
            var measurementsList = new List<ObjectId>();
            var measurementAttempt = await GetById(measurementAttemptId);
            if (measurementAttempt is null) return measurementsList;
            return measurementAttempt.MeasurementsId.Select(x => new ObjectId(x)).ToList();
        }

        public async Task<MeasurementAttempt> GetByNameAndMdvId(string name, string mdvId)
        {
            return await _measurementAttemptCollection
                .Find(Builders<MeasurementAttempt>.Filter.Where(x => x.MdvId == new ObjectId(mdvId) && x.Name == name))
                .FirstOrDefaultAsync();
        }

        public async Task<List<MeasurementAttempt>> GetByMdvId(string mdvId)
        {
            return await _measurementAttemptCollection
                .Find(Builders<MeasurementAttempt>.Filter.Where(x => x.MdvId == new ObjectId(mdvId)))
                .ToListAsync();
        }

        public async Task<string> GetPenultMeasurementId(ObjectId measurementAttemptId)
        {
            var measurementAttempt = await GetById(measurementAttemptId);
            return measurementAttempt.MeasurementsId.ElementAt(measurementAttempt.MeasurementsId.Count - 2) ?? string.Empty;
        }

        public async Task<string> GetLastMeasurementId(ObjectId measurementAttemptId)
        {
            var measurementAttempt = await GetById(measurementAttemptId);
            return measurementAttempt.MeasurementsId.LastOrDefault() ?? string.Empty;
        }

        public async Task<bool> PushNewMeasurement(ObjectId measurementAttemptId, Measurement measurement)
        {
            var isRoot = false;
            var measurementAttempt = await GetById(measurementAttemptId);
            if (measurementAttempt.MeasurementsId.Count == 0)
            {
                await SetRootMeasurementId(measurementAttemptId, measurement);
                isRoot = true;
            }
            await _measurementAttemptCollection.UpdateOneAsync(Builders<MeasurementAttempt>.Filter.Where(x => x.Id == measurementAttemptId),
                Builders<MeasurementAttempt>.Update.Push(x => x.MeasurementsId, measurement.Id.ToString()),
                new UpdateOptions { IsUpsert = true });
            return isRoot;
        }

        public async Task<MeasurementAttempt> SetRootMeasurementId(ObjectId measurementAttemptId, Measurement measurement)
        {
            var filter = Builders<MeasurementAttempt>.Filter.Where(x => x.Id == measurementAttemptId);
            var updateRoot = Builders<MeasurementAttempt>.Update.Set(x => x.RootMeasurementId, measurement.Id.ToString());
            await _measurementAttemptCollection.FindOneAndUpdateAsync(filter, updateRoot,
                new FindOneAndUpdateOptions<MeasurementAttempt> { IsUpsert = true });
            return await GetById(measurementAttemptId);
        }

        public async Task<MeasurementAttempt> SetAttemptResult(ObjectId measurementAttemptId, MeasurementResult measurementResult)
        {
            var filter = Builders<MeasurementAttempt>.Filter.Where(x => x.Id == measurementAttemptId);
            var update = Builders<MeasurementAttempt>.Update.Set(x => x.MeasurementResult, measurementResult);
            await _measurementAttemptCollection.FindOneAndUpdateAsync(filter, update,
                new FindOneAndUpdateOptions<MeasurementAttempt> { IsUpsert = true });
            return await GetById(measurementAttemptId);
        }

        public async Task<MeasurementAttempt> GetMasterByMdvId(string mdvId)
        {
            return (await _measurementAttemptCollection
                .Find(Builders<MeasurementAttempt>.Filter.Where(x => x.MdvId == new ObjectId(mdvId)))
                .ToListAsync()).FirstOrDefault(x => x.Name == "master");
        }

        public async Task<bool> Delete(ObjectId measurementAttemptId)
        {
            var deleteResult =
                await _measurementAttemptCollection.DeleteOneAsync(
                    Builders<MeasurementAttempt>.Filter.Eq(x => x.Id, measurementAttemptId));
            return deleteResult.DeletedCount == 1;
        }
    }
}
