using AutoMapper;
using VueExample.Entities;
using VueExample.Models.SRV6;

namespace VueExample.Profiles
{
    public class KurbatovParameterBordersProfile : Profile
    {
        public KurbatovParameterBordersProfile()
        {
            CreateMap<KurbatovParameterBordersEntity, KurbatovParameterBordersModel>().ReverseMap();
        }
    }
}