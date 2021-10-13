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
        private readonly IMongoCollection<Graphic4UploadingViewModel> _graphic4Collection;
        private readonly IMeasurementRecordingService _measurementRecordingService;
        private readonly IDieWithCodeService _dieWithCodeService;
        public Graphic4Service(IMongoClient mongoClient, IMeasurementRecordingService measurementRecordingService, IDieWithCodeService dieWithCodeService)
        {
            _graphic4Collection = mongoClient.GetDatabase("srv6_graphic4").GetCollection<Graphic4UploadingViewModel>("Graphic4");
            _measurementRecordingService = measurementRecordingService;
            _dieWithCodeService = dieWithCodeService;
        }
        public async Task<Graphic4UploadingViewModel> CreateGraphic4(List<Graphic4ParseResult> graphic4ParseResultList, int measurementRecordingId, string waferId)
        {
            var graphicViewModel = new Graphic4UploadingViewModel();
            graphicViewModel.WaferId = waferId;
            graphicViewModel.MeasurementRecordingId = measurementRecordingId;
            var graphic4ParseResultViewModelList = new List<Graphic4ParseResultViewModel>();
            foreach (var graphic4ParseResult in graphic4ParseResultList)
            {
                graphic4ParseResultViewModelList.Add(new Graphic4ParseResultViewModel(graphic4ParseResult, await _dieWithCodeService.CreateDieWithCodes(graphic4ParseResult.DieWithCodesList)));
            }
            graphicViewModel.GraphicData = graphic4ParseResultViewModelList;
            await _graphic4Collection.InsertOneAsync(graphicViewModel);
            return graphicViewModel;
        }

        public async Task<bool> DeleteGraphic4(int measurementRecordingId)
        {
            var dieWithCodeIds = (await _graphic4Collection.Find(Builders<Graphic4UploadingViewModel>.Filter.Eq(x => x.MeasurementRecordingId, measurementRecordingId)).FirstOrDefaultAsync()).GraphicData.SelectMany(x => x.DieWithCodesList).ToList();
            var dieWithCodeDeleteResult = await _dieWithCodeService.DeleteDieWithCodes(dieWithCodeIds);
            var deleteResult = await _graphic4Collection.DeleteOneAsync(Builders<Graphic4UploadingViewModel>.Filter.Eq(x => x.MeasurementRecordingId, measurementRecordingId));
            await _measurementRecordingService.Delete(measurementRecordingId);
            return deleteResult.DeletedCount == 1;
        }

        public async Task<Graphic4ViewModel> GetGraphic4ByMeasurementRecordingId(int measurementRecordingId)
        {
            var graphic4UploadingViewModel = await _graphic4Collection.Find(Builders<Graphic4UploadingViewModel>.Filter.Eq(x => x.MeasurementRecordingId, measurementRecordingId)).FirstOrDefaultAsync();
            var graphic4ViewModel = new Graphic4ViewModel();
            graphic4ViewModel.MeasurementRecordingId = graphic4UploadingViewModel.MeasurementRecordingId;
            graphic4ViewModel.WaferId = graphic4UploadingViewModel.WaferId;
            graphic4ViewModel.GraphicData = (await Task.WhenAll(graphic4UploadingViewModel.GraphicData.Select(async x => new Graphic4ParseResult{
                Graphic = x.Graphic,
                States = x.States.ToList(),
                DieWithCodesList = (await Task.WhenAll(x.DieWithCodesList.Select(async d => await _dieWithCodeService.GetById(d)))).ToList()
            }))).ToList();
            return graphic4ViewModel;
        }
    }
}