using AutoMapper;
using VueExample.ViewModels.Vertx.ResponseModels;

namespace Vertx.AutoMapperProfiles
{
    public class PointVertxProfile : Profile
    {
        public PointVertxProfile()
        {
            CreateMap<VueExample.Models.Vertx.Point, PointResponseModel>().ReverseMap();
        }
    }
}