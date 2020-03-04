using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using VueExample.Entities;
using VueExample.Extensions;
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
        private readonly IKurbatovParameterProvider _kurbatovParameterProvider;
        private readonly IKurbatovParameterBordersProvider _kurbatovParameterBordersProvider;
        public StandartPatternService(IStandartPatternProvider standartPatternProvider, IStandartMeasurementPatternProvider standartMeasurementPatternProvider, IKurbatovParameterProvider kurbatovParameterProvider,  IKurbatovParameterBordersProvider kurbatovParameterBordersProvider, IMapper mapper)
        {
            _standartPatternProvider = standartPatternProvider;
            _kurbatovParameterProvider = kurbatovParameterProvider;
            _kurbatovParameterBordersProvider = kurbatovParameterBordersProvider;
            _standartMeasurementPatternProvider = standartMeasurementPatternProvider;
            _mapper = mapper;
        }

        public async Task<StandartPattern> Create(StandartPatternViewModel standartPattern)
        {
            var standartPatternEntity = await _standartPatternProvider.Create(_mapper.Map<StandartPatternViewModel, StandartPattern>(standartPattern));
            return _mapper.Map<StandartPatternEntity, StandartPattern>(standartPatternEntity);
        }

        public async Task CreateFull(StandartMeasurementPatternFullViewModel standartMeasurementPatternFull)
        {
            var standartPatternId = (await Create(standartMeasurementPatternFull.StandartPattern)).Id;
            await standartMeasurementPatternFull.standartMeasurementPatternList.ForEachAsync(async smp => {
                smp.PatternId = standartPatternId;
                var smpId = (await _standartMeasurementPatternProvider.Create(_mapper.Map<StandartMeasurementPatternViewModel, StandartMeasurementPatternModel>(smp))).Id;
                await smp.kpList.ForEachAsync(async kp => {
                    var bordersId = (await _kurbatovParameterBordersProvider.Create(_mapper.Map<KurbatovParameterBordersViewModel, KurbatovParameterBordersModel>(kp.KurbatovParameterBorders))).Id;
                    await _kurbatovParameterProvider.Create(bordersId, kp.StandartParameter.Id, smpId);
                });
            });
        }


        public async Task Delete(int id)
        {
            await _standartPatternProvider.Delete(id);
        }

        public async Task<IList<StandartPattern>> GetByDieTypeId(int dieTypeId)
            =>  _mapper.Map<IList<StandartPatternEntity>, IList<StandartPattern>>(await _standartPatternProvider.GetByDieTypeId(dieTypeId));
    }
}