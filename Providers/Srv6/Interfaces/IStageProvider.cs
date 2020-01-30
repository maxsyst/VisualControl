using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models;
using VueExample.ViewModels;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface IStageProvider
    {
        Task<Stage> GetById(int id);
        Task<Stage> GetByMeasurementRecordingId(int measurementRecordingId);
        Task<List<Stage>> GetAll();
        Task<List<Stage>> GetStagesByProcessId(int processId);
        Task<List<Stage>> GetStagesByWaferId(string waferId);
        Task<Stage> Create(string name, int processId);
        Task<Stage> Create(Stage stage);
        Task<Stage> Update(Stage stage);
        Task Delete(int stageId);
    }
}