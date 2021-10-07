using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Entities;
using VueExample.Models.SRV6;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface IMeasurementRecordingService
    {
        Task<List<MeasurementRecording>> GetByWaferId(string waferId);
        Task<List<MeasurementRecording>> GetByWaferIdAndDieType(string waferId, string dieTypeName);
        Task<MeasurementRecording> GetOrCreate(string name, int type, int bmrId, int? stageId);
        Task Merge(int srcMeasurementRecordingId, int destMeasurementRecordingId);
        Task Delete(int measurementRecordingId);
        Task DeleteSet(IList<int> measurementRecordingIdList);
        Task DeleteSpecificMultiplyMeasurement(int measurementRecordingId, List<int> graphicIdList);
        Task DeleteSpecificMeasurement(int measurementRecordingId, int graphicId);
        Task<MeasurementRecording> UpdateName(int measurementRecordingId, string newName);
        Task<BigMeasurementRecording> GetOrAddBigMeasurement(string name, string waferId);
        Task<FkMrP> GetOrCreateFkMrP(int measurementRecordingId, short parameterId, string waferId);
        Task<FkMrGraphic> GetFkMrGraphics(int? measurementRecordingId, int graphicId);
        Task<FkMrGraphic> AddOrGetFkMrGraphic(FkMrGraphic fkMrGraphic);
        Task<List<FkMrGraphic>> AddOrGetFkMrGraphics(List<FkMrGraphic> fkMrGraphics);
        Task<List<MeasurementRecording>> GetByWaferIdAndStageNameAndElementId(string waferId, string stageName, int elementId);
        Task<MeasurementRecording> GetByNameAndWaferId(string name, string waferId);
        Task<MeasurementRecording> GetByBmrIdAndName(int bmrId, string name);
        Task<MeasurementRecording> UpdateStage(int measurementRecordingId, int stageId);
        Task<MeasurementRecording> GetById(int id);
    }
}