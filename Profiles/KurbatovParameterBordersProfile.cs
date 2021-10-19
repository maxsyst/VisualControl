using AutoMapper;
using VueExample.Entities;
using VueExample.Models.SRV6;
using VueExample.ViewModels.Kurbatov;

namespace VueExample.Profiles
{
    public class KurbatovParameterBordersProfile : Profile
    {
        public KurbatovParameterBordersProfile()
        {
            CreateMap<KurbatovParameterBordersEntity, KurbatovParameterBordersModel>().ReverseMap();
            CreateMap<KurbatovParameterBordersModel, KurbatovParameterBordersViewModel>().ReverseMap();
        }
    }
}