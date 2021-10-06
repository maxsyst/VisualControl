using System.Collections.Generic;
using VueExample.Contexts;
using System.Linq;
using VueExample.Models.SRV6;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;
using VueExample.Entities;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.Exceptions;

namespace VueExample.Providers.Srv6
{
    public class MeasurementRecordingService : IMeasurementRecordingService
    {
        private readonly Srv6Context _srv6Context;
        public MeasurementRecordingService(Srv6Context srv6Context)
        {
            _srv6Context = srv6Context;
        }
        public async Task<MeasurementRecording> GetOrCreate(string name, int type, int bigMeasurementRecordingId, int? stageId = null) 
        {
            var measurementRecording = await GetByBmrIdAndName(bigMeasurementRecordingId, "оп." + name);
            if(measurementRecording is null)
            {
                var newMeasurementRecording = new MeasurementRecording{Name = "оп." + name, 
                                                                       MeasurementDateTime = DateTime.Now, Type = type, 
                                                                       BigMeasurementRecordingId = bigMeasurementRecordingId, 
                                                                       StageId = stageId};
                _srv6Context.MeasurementRecordings.Add(newMeasurementRecording);
                await _srv6Context.SaveChangesAsync();
                return newMeasurementRecording;
            }
            return measurementRecording;
        }

        public async Task<BigMeasurementRecording> GetOrAddBigMeasurement(string name, string waferId) 
        {
            var bigMeasurementRecording = await _srv6Context.BigMeasurementRecordings.FirstOrDefaultAsync(x => x.WaferId == waferId && x.Name == name);
            if(bigMeasurementRecording is null)
            {
                bigMeasurementRecording = new BigMeasurementRecording {Name = name, WaferId = waferId};
                _srv6Context.Add(bigMeasurementRecording);
                await _srv6Context.SaveChangesAsync();
            }
            return bigMeasurementRecording;
        }

        public async Task<FkMrP> GetOrCreateFkMrP(int measurementRecordingId, short parameterId, string waferId)
        {
            var fkmrp = await _srv6Context.FkMrPs.FirstOrDefaultAsync(x => x.MeasurementRecordingId == measurementRecordingId && x.Id247 == parameterId && x.WaferId == waferId);
            if(fkmrp is null)
            {
                fkmrp = new FkMrP{MeasurementRecordingId = measurementRecordingId, WaferId = waferId, Id247 = parameterId};
                _srv6Context.Add(fkmrp);
                await _srv6Context.SaveChangesAsync();
            }              
            return fkmrp;
        }
        public async Task<FkMrGraphic> AddOrGetFkMrGraphics(FkMrGraphic fkMrGraphic) 
        {
            var newFkMrGraphic = await _srv6Context.FkMrGraphics.FirstOrDefaultAsync(x => x.MeasurementRecordingId == fkMrGraphic.MeasurementRecordingId
                                                                                       && x.GraphicId == fkMrGraphic.GraphicId);
            if(newFkMrGraphic is null)
            {
                _srv6Context.FkMrGraphics.Add(fkMrGraphic);
                await _srv6Context.SaveChangesAsync();
            } 
            return newFkMrGraphic;              
        }

        public async Task<List<MeasurementRecording>> GetByWaferId(string waferId)
        {
            var measurementRecordingsList = await _srv6Context.FkMrPs
                                                  .Where(x => x.WaferId == waferId && x.Id247 == 247)
                                                  .AsNoTracking()
                                                  .Select(x => x.MeasurementRecording)
                                                  .ToListAsync();
            return measurementRecordingsList;
        }

        // public async Task<MeasurementRecording> GetByNameAndWaferId(string name, string waferId) 
        // {
        //     var mrList = await _srv6Context.FkMrPs.Where(x => x.WaferId == waferId).ToListAsync();
        //     return mrList.Select(measurementRecording => _srv6Context.MeasurementRecordings.FirstOrDefault(x => x.Id == measurementRecording.MeasurementRecordingId)).FirstOrDefault(mr => mr != null && mr.Name == name);                
        // }

        public async Task<MeasurementRecording> GetByNameAndWaferId(string name, string waferId) 
        {
            return await _srv6Context.FkMrPs.Where(f => f.WaferId == waferId)
                                            .Join(_srv6Context.MeasurementRecordings, 
                                            c => c.MeasurementRecordingId, 
                                            p => p.Id, 
                                            (c,p) => p).Where(x => x.Name == name).FirstOrDefaultAsync();            
        }

        public async Task<MeasurementRecording> GetByBmrIdAndName(int bmrId, string name) 
            => await _srv6Context.MeasurementRecordings.FirstOrDefaultAsync(x => x.Name == name && x.BigMeasurementRecordingId == bmrId);

        public Task<List<MeasurementRecording>> GetByWaferIdAndStageNameAndElementId(string waferId, string stageName, int elementId)
        {
            var waferIdSqlParameter = new SqlParameter("waferId", waferId);
            var stageNameSqlParameter = new SqlParameter("stageName", stageName);
            var elementIdSqlParameter = new SqlParameter("elementId", elementId);
            return _srv6Context.MeasurementRecordings.FromSqlInterpolated($"EXECUTE select_mr_by_stagename_waferid_elementid @waferId = {waferId}, @elementId = {elementId}, @stageName = {stageName}").ToListAsync();
        }

        public async Task<List<MeasurementRecording>> GetByWaferIdAndDieType(string waferId, string dieTypeName)
        {
            var dieType = await _srv6Context.DieTypes.FirstOrDefaultAsync(x => x.Name == dieTypeName);
            if(dieType is null)
            {
                return new List<MeasurementRecording>();
            }
            return await _srv6Context.MeasurementRecordings.FromSqlInterpolated($"EXECUTE select_all_mr_by_waferid_dietypeid @waferId = {waferId}, @dieTypeId = {dieType.DieTypeId}").ToListAsync();
        }

        public async Task<MeasurementRecording> UpdateStage(int measurementRecordingId, int stageId)
        {
            var measurementRecording = await _srv6Context.MeasurementRecordings.FirstOrDefaultAsync(x => x.Id == measurementRecordingId);
            measurementRecording.StageId = stageId;
            await _srv6Context.SaveChangesAsync();
            return measurementRecording;
        }

        public async Task<MeasurementRecording> GetById(int id) 
            => await _srv6Context.MeasurementRecordings.FirstOrDefaultAsync(x => x.Id == id);
        
        public async Task Delete(int measurementRecordingId)
        {
            var measurementRecording =  await _srv6Context.MeasurementRecordings.FirstOrDefaultAsync(x => x.Id == measurementRecordingId) 
                                        ?? throw new RecordNotFoundException();                
            var measurementRecordingSqlParameter = new SqlParameter("idmr", measurementRecording.Id);
            _srv6Context.Database.ExecuteSqlRaw("EXECUTE dbo.delete_full_measurement_recording @idmr", measurementRecordingSqlParameter);         
        }

        public async Task DeleteSpecificMeasurement(int measurementRecordingId, int graphicId)
        {
            var fkMrGraphicsCount = await _srv6Context.FkMrGraphics.CountAsync(x => x.MeasurementRecordingId == measurementRecordingId);
            if(fkMrGraphicsCount == 1) 
            {
                await Delete(measurementRecordingId);
            }                                 
            else 
            {
                if(fkMrGraphicsCount == 0) 
                {
                    throw new RecordNotFoundException();
                }
                await DeleteSpecificGraphic(measurementRecordingId, graphicId);
            }
        }

        public async Task DeleteSpecificMultiplyMeasurement(int measurementRecordingId, List<int> graphicIdList)
        {
            var fkMrGraphicsCount = await _srv6Context.FkMrGraphics.CountAsync(x => x.MeasurementRecordingId == measurementRecordingId);
            if(fkMrGraphicsCount == 0) 
            {
                throw new RecordNotFoundException();
            }
      
            if(fkMrGraphicsCount == graphicIdList.Count) 
            {
                await Delete(measurementRecordingId);
            }
            else 
            {
                foreach (var graphicId in graphicIdList)
                {
                    await DeleteSpecificGraphic(measurementRecordingId, graphicId);
                }
            }
            
        }

        public async Task<FkMrGraphic> GetFkMrGraphics(int? measurementRecordingId, int graphicId)
        {
            measurementRecordingId = measurementRecordingId ?? throw new RecordNotFoundException();
            return await _srv6Context.FkMrGraphics.FirstOrDefaultAsync(x => x.MeasurementRecordingId == measurementRecordingId
                                                                            && x.GraphicId == graphicId) ?? throw new RecordNotFoundException();
        }

        public async Task DeleteSet(IList<int> measurementRecordingIdList)
        {
            foreach (var measurementRecording in measurementRecordingIdList)
            {
               await Delete(measurementRecording);
            }
        }

        public async Task<MeasurementRecording> UpdateName(int measurementRecordingId, string newName)
        {
            if(newName.Contains("оп"))
                throw new ValidationErrorException();
            var measurementRecording = await _srv6Context.MeasurementRecordings.FirstOrDefaultAsync(x => x.Id == measurementRecordingId) ?? throw new RecordNotFoundException();
            measurementRecording.Name = $"оп.{newName}";
            await _srv6Context.SaveChangesAsync();
            return measurementRecording;
        }

        private async Task DeleteSpecificGraphic(int measurementRecordingId, int graphicId)
        {
            var graphicMeasurementRecording =  await _srv6Context.FkMrGraphics.FirstOrDefaultAsync(x => x.GraphicId == graphicId && x.MeasurementRecordingId == measurementRecordingId) 
                                                   ?? throw new RecordNotFoundException();                                              
            var valueList = await _srv6Context.DieGraphics.Where(x => x.MeasurementRecordingId == measurementRecordingId && x.GraphicId == graphicId).ToListAsync();                                                
            _srv6Context.DieGraphics.RemoveRange(valueList);
            _srv6Context.FkMrGraphics.Remove(graphicMeasurementRecording);
            await _srv6Context.SaveChangesAsync();      
        }

        public async Task Merge(int srcMeasurementRecordingId, int destMeasurementRecordingId)
        {
            var fkMrGraphicList = await _srv6Context.FkMrGraphics.Where(x => x.MeasurementRecordingId == srcMeasurementRecordingId).ToListAsync();
            foreach (var fkMr in fkMrGraphicList)
            {
                fkMr.MeasurementRecordingId = destMeasurementRecordingId;
            }
            var dieGraphicList = await _srv6Context.DieGraphics.Where(x => x.MeasurementRecordingId == srcMeasurementRecordingId).ToListAsync();
            foreach (var dieGraphics in dieGraphicList)
            {
                dieGraphics.MeasurementRecordingId = destMeasurementRecordingId;
            }
            await _srv6Context.SaveChangesAsync();
            await this.Delete(srcMeasurementRecordingId);
        }
    }
}