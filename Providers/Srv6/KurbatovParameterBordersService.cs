using System.Threading.Tasks;
using AutoMapper;
using VueExample.Entities;
using VueExample.Models.SRV6;
using VueExample.Providers.Abstract;
using VueExample.Providers.Srv6.Interfaces;

namespace VueExample.Providers.Srv6
{
    public class KurbatovParameterBordersService : IKurbatovParameterBordersService
    {
        private readonly IMapper _mapper;
        private readonly IKurbatovParameterBordersProvider _kurbatovParameterBordersProvider;
        public KurbatovParameterBordersService(IMapper mapper, IKurbatovParameterBordersProvider kurbatovBordersProvider)
        {
            _mapper = mapper;
            _kurbatovParameterBordersProvider = kurbatovBordersProvider;
        }
        public async Task<KurbatovParameterBordersModel> Create(KurbatovParameterBordersModel kurbatovBordersModel)
            
            => _mapper.Map<KurbatovParameterBordersEntity, KurbatovParameterBordersModel>(await _kurbatovParameterBordersProvider.Create(kurbatovBordersModel));

        public async Task<KurbatovParameterBordersModel> Update(KurbatovParameterBordersModel kurbatovBordersModel)
           
           => _mapper.Map<KurbatovParameterBordersEntity, KurbatovParameterBordersModel>(await _kurbatovParameterBordersProvider.Update(kurbatovBordersModel));
    }
}