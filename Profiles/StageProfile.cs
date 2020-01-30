using AutoMapper;
using VueExample.Models;
using VueExample.ViewModels;

namespace VueExample.Profiles
{
    public class StageProfile : Profile
    {
        public StageProfile()
        {
            CreateMap<Stage, StageViewModel>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.StageId))
                                              .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.StageName))
                                              .ReverseMap();
        }
    }
}