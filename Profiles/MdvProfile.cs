using AutoMapper;
using MongoDB.Bson;
using VueExample.Models.Vertx;
using VueExample.ViewModels.Vertx.ResponseModels;

namespace VueExample.Profiles
{
    public class MdvProfile : Profile
    {
        public MdvProfile()
        {
            CreateMap<Mdv, MdvResponseModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom<string>(src => src.Id.ToString()));
            CreateMap<MdvResponseModel, Mdv>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => new ObjectId(src.Id)));
        }
    }
}