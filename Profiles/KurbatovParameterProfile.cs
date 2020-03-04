using AutoMapper;
using VueExample.Entities;
using VueExample.Models.SRV6;

namespace VueExample.Profiles
{
    public class KurbatovParameterProfile : Profile
    {
        public KurbatovParameterProfile()
        {
            CreateMap<KurbatovParameterEntity, KurbatovParameterModel>().ReverseMap();
        }
    }
}