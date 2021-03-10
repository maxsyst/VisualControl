using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using VueExample.Models.Vertx;

namespace VueExample.Services.Vertx.Abstract
{
    public interface IMeasurementService
    {
        Task<Measurement> Create(Measurement measurement, ObjectId measurementAttemptId);
        Task<Measurement> GetById(ObjectId id);
        Task<Measurement> GetByName(string name);
        Task<List<Characteristic>> GetAllCharacteristics(ObjectId id);
        Task<List<Measurement>> GetByMeasurementAttemptId(ObjectId measurementAttemptId);
        Task<Measurement> FullUpdate(Measurement measurement);
        Task<Measurement> SetFinishDate(ObjectId measurementId);
        Task<LastUpdate> SetLastUpdate(ObjectId measurementId, LastUpdate lastUpdate);
        Task<bool> Delete(ObjectId id);
    }
}
