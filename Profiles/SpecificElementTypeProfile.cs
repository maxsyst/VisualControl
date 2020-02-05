using AutoMapper;
using VueExample.Models.SRV6;
using VueExample.ViewModels;

namespace VueExample.Profiles
{
    public class SpecificElementTypeProfile : Profile
    {
        public SpecificElementTypeProfile()
        {
            CreateMap<SpecificElementType, SpecificElementTypeViewModel>().ReverseMap();
        }
    }
}