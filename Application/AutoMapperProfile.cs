using Application.DTOS;

using AutoMapper;

using Domain.Entities;

namespace Application;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {

        CreateMap<User, UserStatusDto>();
    }
}

