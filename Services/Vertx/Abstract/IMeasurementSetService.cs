using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using VueExample.Models.Vertx;

namespace VueExample.Services.Vertx.Abstract
{
    public interface IMeasurementSetService
    {
        Task<MeasurementSet> Create(ObjectId measurementId, string measurementSetPlusUnitId);
        Task UpdatePoints(ObjectId id, List<Point> pointsList);
        Task<MeasurementSet> GetById(ObjectId id);
        Task<MeasurementSet> GetLastMeasurementSet(ObjectId measurementId, string measurementSetPlusUnitId);
        Task<IList<string>> GetAllByMeasurementIdsSetPlusUnitId(ObjectId measurementId, string measurementSetPlusUnitId);
    }
}
