using AutoMapper;
using MongoDB.Bson;
using VueExample.Models.Vertx;
using VueExample.ViewModels.Vertx.InputModels;
using VueExample.ViewModels.Vertx.ResponseModels;

namespace VueExample.Profiles
{
    public class MeasurementAttemptProfile : Profile
    {
        public MeasurementAttemptProfile()
        {
            CreateMap<MeasurementAttemptInputModel, MeasurementAttempt>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(src => new ObjectId(src.Id)))
                .ForMember(dest => dest.MdvId,
                    opt => opt.MapFrom(src => new ObjectId(src.MdvId)));
            CreateMap<MeasurementAttempt, MeasurementAttemptResponseModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.MdvId,
                    opt => opt.MapFrom(src => src.MdvId.ToString()));
        }
    }
}