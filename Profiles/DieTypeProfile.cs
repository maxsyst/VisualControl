using AutoMapper;
using VueExample.Models.SRV6;
using VueExample.ViewModels;
using VueExample.ViewModels.DieType;

namespace VueExample.Profiles
{
    public class DieTypeProfile : Profile
    {
        public  DieTypeProfile()
        {
            CreateMap<DieType, DieTypeViewModel>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DieTypeId)).ReverseMap();
        }
    }
}