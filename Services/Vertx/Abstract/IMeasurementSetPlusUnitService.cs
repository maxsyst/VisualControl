using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using VueExample.Models.Vertx;

namespace VueExample.Services.Vertx.Abstract
{
    public interface IMeasurementSetPlusUnitService
    {
        Task<MeasurementSetPlusUnit> Create(Characteristic characteristic, int quantTime, ObjectId measurementId, DateTime creationDate);

        Task<MeasurementSetPlusUnit> GetById(string generatedId, ObjectId measurementId);

        Task<LastUpdate> ChangeLastUpdate(string characteristicName, LastUpdate lastUpdate, ObjectId measurementId);
        Task<bool> ChangeCharacteristicUnit(string characteristicName, string characteristicUnit, ObjectId measurementId);

        Task<MeasurementSetPlusUnit> GetByCharacteristicNameAndMeasurementId(string characteristicName,
            ObjectId measurementId);
        bool IsNecessaryToCreateNewMeasurementSet(bool isNewSet, MeasurementSet measurementSet);
        Task<string> Delete(string characteristicName, ObjectId measurementId);
    }
}
