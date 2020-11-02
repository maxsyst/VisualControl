using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<StandartMeasurementPatternEntity>> CreateFull(List<StandartMeasurementPatternEntity> smpList)
        {
            _srv6Context.StandartMeasurementPatterns.AddRange(smpList);
            await _srv6Context.SaveChangesAsync();
            return smpList;
        }

        public async Task Delete(int standartMeasurementPatternId)
        {
            var standartMeasurementPattern = await _srv6Context.StandartMeasurementPatterns.FindAsync(standartMeasurementPatternId) ?? throw new RecordNotFoundException();
            _srv6Context.Remove(standartMeasurementPattern);
            await _srv6Context.SaveChangesAsync();
        }

        public async Task<List<StandartMeasurementPatternEntity>> GetFullList(int patternId)
        {
            return await _srv6Context.StandartMeasurementPatterns.Where(x => x.PatternId == patternId).Include(x => x.KurbatovParameters).ThenInclude(k => k.StandartParameterEntity)
                                                                                                      .Include(x => x.KurbatovParameters).ThenInclude(k => k.KurbatovParameterBordersEntity).ToListAsync();
        }

        public async Task<List<StandartMeasurementPatternEntity>> UpdateFull(List<StandartMeasurementPatternEntity> smpList)
        {
            var smpOldIdList = await _srv6Context.StandartMeasurementPatterns.Where(x => x.PatternId == smpList.First().PatternId).Select(x => x.Id).ToListAsync();
            foreach (var smp in smpList)
            {
                if(smp.Id == 0)
                {
                    _srv6Context.StandartMeasurementPatterns.Add(smp);
                }
                else 
                {
                    _srv6Context.StandartMeasurementPatterns.Update(smp);
                    smpOldIdList.Remove(smp.Id);
                }
            }
            _srv6Context.StandartMeasurementPatterns.RemoveRange(_srv6Context.StandartMeasurementPatterns.Where(x => smpOldIdList.Contains(x.Id)));
            await _srv6Context.SaveChangesAsync();
            return smpList;
        }
    }
}