using AutoMapper;
using VueExample.Models.SRV6;
using VueExample.ViewModels;

namespace VueExample.Profiles
{
    public class ParcelProfile : Profile
    {
        public ParcelProfile()
        {
            CreateMap<ParcelViewModel, Parcel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name))
            .ReverseMap();
        }
    }
}