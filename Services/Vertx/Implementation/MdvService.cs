using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using VueExample.Models.Vertx;
using VueExample.Services.Vertx.Abstract;

namespace VueExample.Services.Vertx.Implementation
{
    public class MdvService : IMdvService
    {
        private readonly IMongoCollection<Mdv> _mdvCollection;
        private readonly IMongoCollection<MeasurementAttempt> _measurementAttemptCollection;

        public MdvService(IMongoDatabase mongoDatabase)
        {
            _mdvCollection = mongoDatabase.GetCollection<Mdv>("mdv");
            _measurementAttemptCollection = mongoDatabase.GetCollection<MeasurementAttempt>("measurement_attempts");
        }

        public async Task<Mdv> CreateMdv(Mdv mdv)
        {
            if(await GetByWaferAndCode(mdv.WaferId, mdv.Code) == null)
            {
                await _mdvCollection.InsertOneAsync(mdv);
                var measurementAttempt = new MeasurementAttempt
                {
                    MdvId = mdv.Id,
                    MeasurementsId = new List<string>(),
                    Name = "master",
                    RootMeasurementId = string.Empty
                };
                await _measurementAttemptCollection.InsertOneAsync(measurementAttempt);
                return mdv;
            }
            else 
            {
                return null;
            }
        }

        public async Task<bool> Delete(ObjectId id)
        {
            var deleteResult = await _mdvCollection.DeleteOneAsync(Builders<Mdv>.Filter.Eq(x => x.Id, id));
            await _measurementAttemptCollection.DeleteManyAsync(Builders<MeasurementAttempt>.Filter.Eq(x => x.MdvId, id));
            return deleteResult.DeletedCount == 1;
        }

        public async Task<List<Mdv>> GetByWafer(string waferId)
        {
            return await _mdvCollection.Find(Builders<Mdv>.Filter.Eq(x => x.WaferId, waferId)).ToListAsync();
        }

        public async Task<Mdv> GetById(ObjectId id)
        {
            return await _mdvCollection.Find(Builders<Mdv>.Filter.Eq(x => x.Id, id))
                .FirstOrDefaultAsync();
        }

        public async Task<Mdv> GetByWaferAndCode(string waferId, string code)
        {
            return await _mdvCollection.Find(Builders<Mdv>.Filter.Where(x => x.Code == code && x.WaferId == waferId))
                .FirstOrDefaultAsync();
        }
    }
}