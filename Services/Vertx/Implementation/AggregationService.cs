using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MongoDB.Bson;
using VueExample.Models.Vertx;
using VueExample.Services.Vertx.Abstract;
using VueExample.ViewModels.Vertx.ResponseModels;

namespace VueExample.Services.Vertx.Implementation
{
    public class AggregationService : IAggregationService
    {
        private readonly IMdvService _mdvService;
        private readonly IMeasurementAttemptService _measurementAttemptService;
        private readonly IMeasurementService _measurementService;
        private readonly IMapper _mapper;

        public AggregationService(IMapper mapper, IMdvService mdvService, IMeasurementAttemptService measurementAttemptService,
            IMeasurementService measurementService)
        {
            _mapper = mapper;
            _mdvService = mdvService;
            _measurementAttemptService = measurementAttemptService;
            _measurementService = measurementService;
        }

        public async Task<List<LastMeasurementMdv>> GetNLastMeasurementAttemptsWithMdv(int n)
        {
            return await GetMdv(await GetLastUpdates(n));
        }

        private async Task<List<LastMeasurementMdv>> GetLastUpdates(int n = -1)
        {
            var measurementAttemptWithLastUpdates = new List<LastMeasurementMdv>();
            var lastMeasurementIds = await _measurementAttemptService.GetAllLastMeasurementIds();
            foreach (var (measurementId, measurementAttempt) in lastMeasurementIds)
            {
                var measurement = await _measurementService.GetById(new ObjectId(measurementId));
                measurementAttemptWithLastUpdates.Add(
                    new LastMeasurementMdv(_mapper.Map<MeasurementAttemptResponseModel>(measurementAttempt), measurement));
            }

            return n == -1
                ? measurementAttemptWithLastUpdates.OrderByDescending(x => x.Measurement.LastUpdate.Date).ToList()
                : measurementAttemptWithLastUpdates.OrderByDescending(x => x.Measurement.LastUpdate.Date).Take(n).ToList();
        }

        private async Task<List<LastMeasurementMdv>> GetMdv(
            List<LastMeasurementMdv> measurementAttemptWithLastUpdates)
        {
            foreach (var measurementAttemptWithLastUpdate in measurementAttemptWithLastUpdates)
            {
                var mdv = await _mdvService.GetById(new ObjectId(measurementAttemptWithLastUpdate.MeasurementAttempt.MdvId));
                measurementAttemptWithLastUpdate.Mdv = mdv;
            }

            return measurementAttemptWithLastUpdates;
        }
    }
}
