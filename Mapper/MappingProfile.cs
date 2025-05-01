using AutoMapper;
using EmailScheduler.Models.Dtos;
using EmailScheduler.Models.Entities;

namespace EmailScheduler.Mapper
{

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Emails, EmailsDto>()
                .ReverseMap();
        }
    }
}