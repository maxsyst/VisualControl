using AutoMapper;
using VueExample.Entities;
using VueExample.Models.SRV6;
using VueExample.ViewModels.StandartMeasurementPattern;

namespace VueExample.Profiles
{
    public class StandartMeasurementPatternProfile : Profile
    {
        public StandartMeasurementPatternProfile()
        {
            CreateMap<StandartMeasurementPatternEntity, StandartMeasurementPatternModel>().ReverseMap();
            CreateMap<StandartMeasurementPatternModel, StandartMeasurementPatternViewModel>().ReverseMap();
        }
    }
}