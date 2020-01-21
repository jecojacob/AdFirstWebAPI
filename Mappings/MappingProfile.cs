using AdviceFirstApi.Domain.Models;
using AdviceFirstApi.Domain.Models.Dto;
using AutoMapper;

namespace AdviceFirstApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AfUser, AfUserModelList>().ReverseMap();
            
        }
    }
}