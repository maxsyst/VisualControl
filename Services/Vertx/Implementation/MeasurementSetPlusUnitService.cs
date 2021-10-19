using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using VueExample.Exceptions;
using VueExample.Models.Vertx;
using VueExample.Services.Vertx.Abstract;

namespace VueExample.Services.Vertx.Implementation
{
    public class MeasurementSetPlusUnitService : IMeasurementSetPlusUnitService
    {
        private readonly IMongoCollection<Measurement> _measurementCollection;
        private readonly IMeasurementService _measurementService;

        public MeasurementSetPlusUnitService(IMongoClient mongoClient, IMeasurementService measurementService)
        {
            _measurementCollection = mongoClient.GetDatabase("vertx_excel").GetCollection<Measurement>("measurements");
            _measurementService = measurementService;
        }
        public async Task<MeasurementSetPlusUnit> Create(Characteristic characteristic, int quantTime, ObjectId measurementId, DateTime creationDate)
        {
            var measurementSetPlusUnit = new MeasurementSetPlusUnit(characteristic, quantTime, creationDate);
            if (await GetByCharacteristicNameAndMeasurementId(characteristic.Name, measurementId) != null)
                throw new DuplicateException();
            await _measurementCollection.UpdateOneAsync(Builders<Measurement>.Filter.Where(x => x.Id == measurementId),
                Builders<Measurement>.Update.Push(x => x.MeasurementSetPlusUnits, measurementSetPlusUnit),
                new UpdateOptions { IsUpsert = true });
            return measurementSetPlusUnit;
        }
        public async Task<LastUpdate> ChangeLastUpdate(string characteristicName, LastUpdate lastUpdate,
            ObjectId measurementId)
        {
            var measurementSetPlusUnit =
                await GetByCharacteristicNameAndMeasurementId(characteristicName, measurementId);
            var filter = Builders<Measurement>.Filter.And(
                Builders<Measurement>.Filter.Where(x => x.Id == measurementId),
                Builders<Measurement>.Filter.ElemMatch(x => x.MeasurementSetPlusUnits,
                    c => c.GeneratedId == measurementSetPlusUnit.GeneratedId));
            var update = Builders<Measurement>.Update.Set(x => x.MeasurementSetPlusUnits[-1].LastUpdate, lastUpdate);
            await _measurementCollection.FindOneAndUpdateAsync(filter, update,
                new FindOneAndUpdateOptions<Measurement> { IsUpsert = true });
            await _measurementService.SetLastUpdate(measurementId, lastUpdate);
            return lastUpdate;
        }
        public async Task<bool> ChangeCharacteristicUnit(string characteristicName, string characteristicUnit, ObjectId measurementId)
        {
           var measurementSetPlusUnit =
                await GetByCharacteristicNameAndMeasurementId(characteristicName, measurementId);
            if(measurementSetPlusUnit == null) {
                return false;
            }
            var filter = Builders<Measurement>.Filter.And(
                Builders<Measurement>.Filter.Where(x => x.Id == measurementId),
                Builders<Measurement>.Filter.ElemMatch(x => x.MeasurementSetPlusUnits,
                    c => c.GeneratedId == measurementSetPlusUnit.GeneratedId));
             var update = Builders<Measurement>.Update.Set(x => x.MeasurementSetPlusUnits[-1].Characteristic, new Characteristic(characteristicName, characteristicUnit));
            await _measurementCollection.FindOneAndUpdateAsync(filter, update,
                new FindOneAndUpdateOptions<Measurement> { IsUpsert = true });
            return true;
        }
        public async Task<MeasurementSetPlusUnit> GetById(string generatedId, ObjectId measurementId)
        {
            var measurement = await _measurementCollection
                .Find(Builders<Measurement>.Filter.Eq(x => x.Id, measurementId))
                .FirstOrDefaultAsync();
            return measurement.MeasurementSetPlusUnits.FirstOrDefault(x => x.GeneratedId == generatedId);
        }
        public async Task<MeasurementSetPlusUnit> GetByCharacteristicNameAndMeasurementId(string characteristicName,
            ObjectId measurementId)
        {
            var measurement = await _measurementCollection
                .Find(Builders<Measurement>.Filter.Eq(x => x.Id, measurementId))
                .FirstOrDefaultAsync();
            return measurement.MeasurementSetPlusUnits.FirstOrDefault(x => x.Characteristic.Name == characteristicName);
        }
        public bool IsNecessaryToCreateNewMeasurementSet(bool isNewSet, MeasurementSet measurementSet)
        {
            return measurementSet == null || isNewSet || measurementSet.Points.Count >= 1000;
        }
        public async Task<string> Delete(string characteristicName, ObjectId measurementId)
        {
            var measurementSetPlusUnit =
                await GetByCharacteristicNameAndMeasurementId(characteristicName, measurementId);
            await _measurementCollection.UpdateOneAsync(Builders<Measurement>.Filter.Where(x => x.Id == measurementId),
                Builders<Measurement>.Update.PullFilter(x => x.MeasurementSetPlusUnits,
                    Builders<MeasurementSetPlusUnit>.Filter.Where(m =>
                        m.GeneratedId == measurementSetPlusUnit.GeneratedId)), new UpdateOptions { IsUpsert = true });
            return measurementSetPlusUnit.GeneratedId;
        }
    }
}
