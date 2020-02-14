using AutoMapper;
using VueExample.Entities;
using VueExample.Models.SRV6;
using VueExample.ViewModels;

namespace VueExample.Profiles
{
    public class StandartPatternProfile : Profile
    {
        public StandartPatternProfile()
        {
            CreateMap<StandartPatternEntity, StandartPattern>().ReverseMap();
            CreateMap<StandartPattern, StandartPatternViewModel>().ReverseMap();
        }
    }
}