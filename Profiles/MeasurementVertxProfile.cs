﻿using AutoMapper;
using MongoDB.Bson;
using VueExample.Models.Vertx;
using VueExample.ViewModels.Vertx.InputModels;
using VueExample.ViewModels.Vertx.ResponseModels;

namespace VueExample.Profiles
{
    public class MeasurementVertxProfile : Profile
    {
        public MeasurementVertxProfile()
        {
            CreateMap<Measurement, MeasurementResponseModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.MeasurementAttemptId,
                    opt => opt.MapFrom(src => src.MeasurementAttemptId.ToString()));
            CreateMap<MeasurementResponseModel, Measurement>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => new ObjectId(src.Id)))
                .ForMember(dest => dest.MeasurementAttemptId,
                    opt => opt.MapFrom(src => new ObjectId(src.MeasurementAttemptId)));
            CreateMap<MeasurementInputModel, Measurement>()
                .ForMember(dest => dest.MeasurementAttemptId,
                    opt => opt.MapFrom(src => new ObjectId(src.MeasurementAttemptId)));
        }
    }
}