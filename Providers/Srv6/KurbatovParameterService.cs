using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using VueExample.Entities;
using VueExample.Models.SRV6;
using VueExample.Providers.Abstract;
using VueExample.Providers.Srv6.Interfaces;

namespace VueExample.Providers.Srv6
{
    public class KurbatovParameterService : IKurbatovParameterService
    {
        private readonly IMapper _mapper;
        private readonly IKurbatovParameterProvider _kurbatovParameterProvider;
        public KurbatovParameterService(IMapper mapper, IKurbatovParameterProvider kurbatovParameterProvider)
        {
            _mapper = mapper;
            _kurbatovParameterProvider = kurbatovParameterProvider;
        }
        public async Task<KurbatovParameterEntity> Create(int? bordersId, int standartParameterId, int standartMeasurementPatternId)
        {
            return await _kurbatovParameterProvider.Create(bordersId, standartParameterId, standartMeasurementPatternId);
        }

        public async Task<List<KurbatovParameterModel>> GetBySmp(int standartMeasurementPatternId)
        {
            var kpList = await _kurbatovParameterProvider.GetBySmp(standartMeasurementPatternId);
            return kpList.Select(x => new KurbatovParameterModel {  Id = x.Id, 
                                                                    SmpId = x.SmpId, 
                                                                    StandartParameter = _mapper.Map<StandartParameterModel>(x.StandartParameterEntity), 
                                                                    KurbatovParameterBorders = _mapper.Map<KurbatovParameterBordersModel>(x.KurbatovParameterBordersEntity)})
                         .ToList();
        }
    }
}