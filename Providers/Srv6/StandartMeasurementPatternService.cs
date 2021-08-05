using System.Threading.Tasks;
using AutoMapper;
using VueExample.Entities;
using VueExample.Models.SRV6;
using VueExample.Providers.Abstract;
using VueExample.Providers.Srv6.Interfaces;

namespace VueExample.Providers.Srv6
{
    public class StandartMeasurementPatternService : IStandartMeasurementPatternService
    {
        private readonly IMapper _mapper;
        private readonly IStandartMeasurementPatternProvider _standartMeasurementPatternProvider;
        public StandartMeasurementPatternService(IMapper mapper, IStandartMeasurementPatternProvider standartMeasurementPatternProvider)
        {
            _mapper = mapper;
            _standartMeasurementPatternProvider = standartMeasurementPatternProvider;
        }
        public async Task<StandartMeasurementPatternModel> Create(StandartMeasurementPatternModel standartMeasurementPatternModel)
            
            =>  _mapper.Map<StandartMeasurementPatternEntity, StandartMeasurementPatternModel>(await _standartMeasurementPatternProvider.Create(standartMeasurementPatternModel));

        public async Task Delete(int standartMeasurementPatternId)
        {
            await _standartMeasurementPatternProvider.Delete(standartMeasurementPatternId);
        }

        public async Task<StandartMeasurementPatternModel> GetByStageAndElementAndPattern(int stageId, int elementId, int patternId)
        {
            return _mapper.Map<StandartMeasurementPatternEntity, StandartMeasurementPatternModel>(await _standartMeasurementPatternProvider.GetByStageAndElementAndPattern(stageId, elementId, patternId));
        }
    }
}