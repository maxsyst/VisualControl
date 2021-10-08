using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using VueExample.Parsing.Models;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.Services.Abstract;
using VueExample.ViewModels;

namespace VueExample.Services
{
    public class Graphic4Service : IGraphic4Service
    {
        private readonly IMongoCollection<Graphic4ViewModel> _graphic4Collection;
        private readonly IMeasurementRecordingService _measurementRecordingService;
        public Graphic4Service(IMongoClient mongoClient, IMeasurementRecordingService measurementRecordingService)
        {
            _graphic4Collection = mongoClient.GetDatabase("srv6_graphic4").GetCollection<Graphic4ViewModel>("Graphic4");
            _measurementRecordingService = measurementRecordingService;
        }
        public async Task<Graphic4ViewModel> CreateGraphic4(List<Graphic4ParseResult> graphic4ParseResultList, int measurementRecordingId, string waferId)
        {
            var graphicViewModel = new Graphic4ViewModel();
            graphicViewModel.WaferId = waferId;
            graphicViewModel.MeasurementRecordingId = measurementRecordingId;
            graphicViewModel.GraphicData = graphic4ParseResultList.ToList();
            await _graphic4Collection.InsertOneAsync(graphicViewModel);
            return graphicViewModel;
        }

        public async Task<bool> DeleteGraphic4(int measurementRecordingId)
        {
            var deleteResult = await _graphic4Collection.DeleteOneAsync(Builders<Graphic4ViewModel>.Filter.Eq(x => x.MeasurementRecordingId, measurementRecordingId));
            await _measurementRecordingService.Delete(measurementRecordingId);
            return deleteResult.DeletedCount == 1;
        }

        public async Task<Graphic4ViewModel> GetGraphic4ByMeasurementRecordingId(int measurementRecordingId)
        {
            return await _graphic4Collection.Find(Builders<Graphic4ViewModel>.Filter.Eq(x => x.MeasurementRecordingId, measurementRecordingId)).FirstOrDefaultAsync();
        }
    }
}