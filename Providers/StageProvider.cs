using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VueExample.Contexts;
using VueExample.Exceptions;
using VueExample.Models;
using VueExample.Providers.Srv6.Interfaces;

namespace VueExample.Providers
{
    public class StageProvider : IStageProvider
    {
        private readonly Srv6Context _srv6Context;
        public StageProvider(Srv6Context srv6Context)
        {
            _srv6Context = srv6Context;
        }
        public async Task<Stage> Create(string name, int processId) 
        {
            var newStage = new Stage{StageName = name, ProcessId = processId};
            _srv6Context.Stages.Add(newStage);
            await _srv6Context.SaveChangesAsync();
            return newStage;
        }

        public async Task<Stage> Create(Stage stage)
        {
            if(await _srv6Context.Stages.AnyAsync(x => x.ProcessId == stage.ProcessId && x.StageName == stage.StageName) || String.IsNullOrEmpty(stage.StageName))
                throw new ValidationErrorException();
            _srv6Context.Stages.Add(stage);
            await _srv6Context.SaveChangesAsync();
            return stage;
        }

        public async Task Delete(int stageId)
        {
            if(await _srv6Context.MeasurementRecordings.AnyAsync(x => x.StageId == stageId))
                throw new ValidationErrorException();
            var stage = await _srv6Context.Stages.FirstOrDefaultAsync(x => x.StageId == stageId) ?? throw new RecordNotFoundException();
            _srv6Context.Remove(stage);
            await _srv6Context.SaveChangesAsync();
        }

        public async Task<List<Stage>> GetAll() => await _srv6Context.Stages.ToListAsync();

        public async Task<Stage> GetById(int stageId) => await _srv6Context.Stages.FirstOrDefaultAsync(x => x.StageId == stageId) ?? throw new RecordNotFoundException();

        public async Task<Stage> GetByMeasurementRecordingId(int measurementRecordingId)
        {
            var stageId = (await _srv6Context.MeasurementRecordings.FirstOrDefaultAsync(x => x.Id == measurementRecordingId))?.StageId;
            return stageId is null ? new Stage{IsNullObject = true} : await GetById((int)stageId);
        }

        public async Task<List<Stage>> GetStagesByProcessId(int processId)
        {
            var stageList = await _srv6Context.Stages.Where(x => x.ProcessId == processId && x.CodeProductId == null).ToListAsync();
            return stageList.Count == 0 ? throw new RecordNotFoundException() : stageList;
        }
        
        public async Task<List<Stage>> GetStagesByWaferId(string waferId)
        {
            var stageList = await (from stage in _srv6Context.Stages 
                            join process in _srv6Context.Processes on stage.ProcessId equals process.ProcessId
                            join codeProduct in _srv6Context.CodeProducts on process.ProcessId equals codeProduct.ProcessId
                            join wafer in _srv6Context.Wafers on codeProduct.IdCp equals wafer.CodeProductId
                            where wafer.WaferId == waferId
                            select stage).ToListAsync();
            return stageList.Count == 0 ? throw new RecordNotFoundException() : stageList; 
        }

        public async Task<Stage> Update(Stage stage)
        {
            var stageUpdate = await _srv6Context.Stages.FirstOrDefaultAsync(x => x.StageId == stage.StageId) ?? throw new RecordNotFoundException();
            _srv6Context.Entry(stageUpdate).CurrentValues.SetValues(stage);
            await _srv6Context.SaveChangesAsync();
            return stage;
        }
    }
}
