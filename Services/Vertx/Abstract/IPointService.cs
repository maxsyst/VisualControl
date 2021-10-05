using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using VueExample.Models.Vertx;
using VueExample.ViewModels.Vertx.ResponseModels;

namespace VueExample.Services.Vertx.Abstract
{
    public interface IPointService
    {
        Task<Point> Create(double value, Characteristic characteristic, string measurementName, bool isNewSet, DateTime creationDate);
        Task<MeasurementResponseModelWithPoints> GetByMeasurement(ObjectId measurementId, string characteristicName);
        Task<Dictionary<string, MeasurementResponseModelWithPoints>> GetByMeasurement(ObjectId measurementId, string characteristicName, int siftedK, bool withoutBadPoints);
        Task<Dictionary<string, MeasurementResponseModelWithPoints>> GetByMeasurementFirstSeconds(ObjectId measurementId, string characteristicName, int seconds);
        Task<Dictionary<string, MeasurementResponseModelWithPoints>> GetByMeasurementAttemptDuration(
            ObjectId measurementAttemptId, string characteristicName, int siftedK, bool withoutBadPoints);
        Task<Dictionary<string, MeasurementResponseModelWithPoints>> GetByMeasurementAttemptTrueDate(ObjectId measurementAttemptId, string characteristicName, int siftedK, bool withoutBadPoints);
        Task<Dictionary<string, MeasurementResponseModelWithPoints>> GetByMeasurementAttemptFirstSeconds(ObjectId measurementAttemptId, string characteristicName, int seconds);
    }
}
