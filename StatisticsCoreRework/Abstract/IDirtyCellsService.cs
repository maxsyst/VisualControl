using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.StatisticsCoreRework.Models;

namespace VueExample.StatisticsCoreRework.Abstract
{
    public interface IDirtyCellsService
    {
        Task<Dictionary<string, DirtyCellsShort>> GetDirtyCellsShortsByKeyGraphicState(int measurementRecordingId, string keyGraphicState, List<DirtyCellsProfile> dirtyCellsProfiles);
        Task<MeasurementRecordingDirtyCellsSnapshot> GetDirtyCellsInitialSnapShotByMeasurementRecordingId(int measurementRecordingId);
        
    }
}