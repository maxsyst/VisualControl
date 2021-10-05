using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using VueExample.Models.Vertx;

namespace VueExample.Services.Vertx.Abstract
{
    public interface IMeasurementAttemptService
    {
        Task<MeasurementAttempt> CreateMeasurementAttempt(MeasurementAttempt measurementAttempt);
        Task<MeasurementAttempt> GetById(ObjectId id);
        Task<Dictionary<string, MeasurementAttempt>> GetAllLastMeasurementIds();
        Task<List<ObjectId>> GetAllMeasurementIds(ObjectId measurementAttemptId);
        Task<string> GetPenultMeasurementId(ObjectId measurementAttemptId);
        Task<string> GetLastMeasurementId(ObjectId measurementAttemptId);
        Task<MeasurementAttempt> GetByNameAndMdvId(string name, string mdvId);
        Task<List<MeasurementAttempt>> GetByMdvId(string mdvId);
        Task<MeasurementAttempt> GetMasterByMdvId(string mdvId);
        Task<MeasurementAttempt> SetAttemptResult(ObjectId measurementAttemptId, MeasurementResult measurementResult);
        Task<bool> PushNewMeasurement(ObjectId measurementAttemptId, Measurement measurement);
        Task<bool> Delete(ObjectId measurementAttemptId);
    }
}
