using AutoMapper;
using VueExample.Entities;
using VueExample.Models.SRV6;
using VueExample.ViewModels;

namespace VueExample.Profiles
{
    public class StandartParameterProfile : Profile
    {
        public StandartParameterProfile()
        {
            CreateMap<StandartParameterEntity, StandartParameterModel>().ReverseMap();
            CreateMap<StandartParameterModel, StandartParameterViewModel>().ReverseMap();
        }
    }
}