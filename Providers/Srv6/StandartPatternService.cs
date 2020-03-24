using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using VueExample.Entities;
using VueExample.Exceptions;
using VueExample.Models.SRV6;
using VueExample.Providers.Abstract;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.ViewModels;

namespace VueExample.Providers.Srv6
{
    public class StandartPatternService : IStandartPatternService
    {
        private readonly IMapper _mapper;
        private readonly IStandartPatternProvider _standartPatternProvider;
        private readonly IStandartMeasurementPatternProvider _standartMeasurementPatternProvider;
        public StandartPatternService(IStandartPatternProvider standartPatternProvider, IStandartMeasurementPatternProvider standartMeasurementPatternProvider, IMapper mapper)
        {
            _standartPatternProvider = standartPatternProvider;
            _standartMeasurementPatternProvider = standartMeasurementPatternProvider;
            _mapper = mapper;
        }

        public async Task<StandartPattern> Create(StandartPatternViewModel standartPattern)
        {
            var standartPatternEntity = await _standartPatternProvider.Create(_mapper.Map<StandartPatternViewModel, StandartPattern>(standartPattern));
            return _mapper.Map<StandartPatternEntity, StandartPattern>(standartPatternEntity);
        }

        public async Task<StandartPattern> CreateFull(StandartMeasurementPatternFullViewModel standartMeasurementPatternFull)
        {
            if(!(await _standartPatternProvider.GetByName(standartMeasurementPatternFull.StandartPattern.Name)).IsNullObject)
                throw new ValidationErrorException();
            var smpList = new List<StandartMeasurementPatternEntity>();
            var standartPattern = new StandartPatternEntity{Name = standartMeasurementPatternFull.StandartPattern.Name, DieTypeId = standartMeasurementPatternFull.StandartPattern.DieTypeId};
            foreach(var smp in standartMeasurementPatternFull.standartMeasurementPatternList)
            {
                var smpFull = new StandartMeasurementPatternEntity{ ElementId = smp.ElementId, 
                                                                    StageId = smp.StageId, 
                                                                    DividerId = smp.DividerId, 
                                                                    PatternId = smp.PatternId, 
                                                                    Name = smp.Name};
                smpFull.StandartPattern = standartPattern;
                smpFull.KurbatovParameters = new List<KurbatovParameterEntity>();
                foreach (var kp in smp.kpList)
                {
                    if (String.IsNullOrEmpty(kp.KurbatovParameterBorders.Lower) && String.IsNullOrEmpty(kp.KurbatovParameterBorders.Upper))
                    {
                        smpFull.KurbatovParameters.Add(new KurbatovParameterEntity{StandartParameterId = kp.StandartParameter.Id});
                    }
                    else
                    {
                        smpFull.KurbatovParameters.Add(new KurbatovParameterEntity{KurbatovParameterBordersEntity = new KurbatovParameterBordersEntity{Upper = kp.KurbatovParameterBorders.Upper, Lower = kp.KurbatovParameterBorders.Lower}, StandartParameterId = kp.StandartParameter.Id});
                    }
                }
                smpList.Add(smpFull);
            }
            await _standartMeasurementPatternProvider.CreateFull(smpList);
            return _mapper.Map<StandartPatternEntity, StandartPattern>(standartPattern);
        }

        public async Task<StandartPattern> Update(StandartMeasurementPatternFullViewModel standartMeasurementPatternFull)
        {
            var standartPattern = await _standartPatternProvider.GetById(standartMeasurementPatternFull.StandartPattern.Id);
            if(standartPattern.IsNullObject)
                throw new RecordNotFoundException();
            var smpList = new List<StandartMeasurementPatternEntity>();
            foreach(var smp in standartMeasurementPatternFull.standartMeasurementPatternList)
            {
                var smpFull = new StandartMeasurementPatternEntity{ Id = smp.Id,
                                                                    ElementId = smp.ElementId, 
                                                                    StageId = smp.StageId, 
                                                                    DividerId = smp.DividerId, 
                                                                    PatternId = smp.PatternId, 
                                                                    Name = smp.Name};
                smpFull.KurbatovParameters = new List<KurbatovParameterEntity>();
                foreach (var kp in smp.kpList)
                {
                    if (String.IsNullOrEmpty(kp.KurbatovParameterBorders.Lower) && String.IsNullOrEmpty(kp.KurbatovParameterBorders.Upper))
                    {
                        smpFull.KurbatovParameters.Add(new KurbatovParameterEntity{Id = kp.Id, StandartParameterId = kp.StandartParameter.Id});
                    }
                    else
                    {
                        smpFull.KurbatovParameters.Add(new KurbatovParameterEntity{Id = kp.Id, KurbatovParameterBordersEntity = new KurbatovParameterBordersEntity{Id = (int)kp.KurbatovParameterBorders.Id, Upper = kp.KurbatovParameterBorders.Upper, Lower = kp.KurbatovParameterBorders.Lower}, StandartParameterId = kp.StandartParameter.Id});
                    }
                }
                smpList.Add(smpFull);
            }
            await _standartMeasurementPatternProvider.UpdateFull(smpList);
            return _mapper.Map<StandartPatternEntity, StandartPattern>(standartPattern);
        }

        public async Task Delete(int id)
        {
            await _standartPatternProvider.Delete(id);
        }

        public async Task<IList<StandartPattern>> GetByDieTypeId(int dieTypeId)
            =>  _mapper.Map<IList<StandartPatternEntity>, IList<StandartPattern>>(await _standartPatternProvider.GetByDieTypeId(dieTypeId));

        public async Task<StandartPattern> GetByName(string name)
        {
            var standartPatternEntity = await _standartPatternProvider.GetByName(name);
            if(standartPatternEntity.IsNullObject)
            {
                throw new RecordNotFoundException();
            }
            return _mapper.Map<StandartPatternEntity, StandartPattern>(standartPatternEntity);
        }

        public async Task<StandartMeasurementPatternFullViewModel> GetFull(int patternId)
        {
            var standartPatternFull = new StandartMeasurementPatternFullViewModel();
            var standartPattern = await _standartPatternProvider.GetById(patternId);
            if(standartPattern.IsNullObject)
                throw new RecordNotFoundException();
            standartPatternFull.StandartPattern = _mapper.Map<StandartPattern, StandartPatternViewModel>(_mapper.Map<StandartPatternEntity, StandartPattern>(standartPattern));
            var smpList = await _standartMeasurementPatternProvider.GetFullList(patternId);   
            if(smpList.Count == 0)
                throw new CollectionIsEmptyException();
            foreach (var smp in smpList)
            {
                var smpVm = _mapper.Map<StandartMeasurementPatternModel, StandartMeasurementPatternViewModel>(_mapper.Map<StandartMeasurementPatternEntity, StandartMeasurementPatternModel>(smp));
                foreach (var kp in smp.KurbatovParameters)
                {
                    var kpVm = new KurbatovParameterViewModel();
                    kpVm.Id = kp.Id;
                    kpVm.KurbatovParameterBorders = new KurbatovParameterBordersViewModel{Id = kp.KurbatovParameterBordersEntity?.Id, 
                                                                                          Lower = kp.KurbatovParameterBordersEntity?.Lower, 
                                                                                          Upper = kp.KurbatovParameterBordersEntity?.Upper};
                    kpVm.StandartParameter = _mapper.Map<StandartParameterModel, StandartParameterViewModel>(_mapper.Map<StandartParameterEntity, StandartParameterModel>(kp.StandartParameterEntity));                
                    smpVm.kpList.Add(kpVm);
                }
                standartPatternFull.standartMeasurementPatternList.Add(smpVm);
            }
            return standartPatternFull;
        }
    }
}