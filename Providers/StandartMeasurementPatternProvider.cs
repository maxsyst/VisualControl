using System.Threading.Tasks;
using VueExample.Contexts;
using VueExample.Entities;
using VueExample.Exceptions;
using VueExample.Models.SRV6;
using VueExample.Providers.Abstract;

namespace VueExample.Providers
{
    public class StandartMeasurementPatternProvider : IStandartMeasurementPatternProvider
    {
        private readonly Srv6Context _srv6Context;
        public StandartMeasurementPatternProvider(Srv6Context srv6Context)
        {
            _srv6Context = srv6Context;
        }
        public async Task<StandartMeasurementPatternEntity> Create(StandartMeasurementPatternModel standartMeasurementPatternModel)
        {
            var standartMeasurementPattern = new StandartMeasurementPatternEntity{  ElementId = standartMeasurementPatternModel.ElementId, 
                                                                                    StageId = standartMeasurementPatternModel.StageId, 
                                                                                    DividerId = standartMeasurementPatternModel.DividerId, 
                                                                                    PatternId = standartMeasurementPatternModel.PatternId, 
                                                                                    Name = standartMeasurementPatternModel.Name};
            _srv6Context.StandartMeasurementPatterns.Add(standartMeasurementPattern);
            await _srv6Context.SaveChangesAsync();
            return standartMeasurementPattern;
        }

        public async Task Delete(int standartMeasurementPatternId)
        {
            var standartMeasurementPattern = _srv6Context.StandartMeasurementPatterns.FindAsync(standartMeasurementPatternId) ?? throw new RecordNotFoundException();
            _srv6Context.Remove(standartMeasurementPattern);
            await _srv6Context.SaveChangesAsync();
        }
    }
}