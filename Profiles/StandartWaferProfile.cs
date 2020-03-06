using AutoMapper;
using VueExample.Entities;
using VueExample.ViewModels;

namespace VueExample.Profiles
{
    public class StandartWaferProfile : Profile
    {
        public StandartWaferProfile()
        {
            CreateMap<CodeProductStandartWafer, StandartWaferViewModel>();
        }
    }
}