using AutoMapper;
using VueExample.Entities;
using VueExample.Models.SRV6;

namespace VueExample.Profiles
{
    public class StandartMeasurementPatternProfile : Profile
    {
        public StandartMeasurementPatternProfile()
        {
            CreateMap<StandartMeasurementPatternEntity, StandartMeasurementPatternModel>().ReverseMap();
        }
    }
}