using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Parsing.Models;
using VueExample.ViewModels;

namespace VueExample.Services.Abstract
{
    public interface IGraphic4Service
    {
        Task<Graphic4ViewModel> GetGraphic4ByMeasurementRecordingId(int measurementRecordingId);
        Task<Graphic4ViewModel> CreateGraphic4(List<Graphic4ParseResult> graphic4ParseResultList, int measurementRecordingId, string waferId);
        Task<bool> DeleteGraphic4(int measurementRecordingId);
    }
}