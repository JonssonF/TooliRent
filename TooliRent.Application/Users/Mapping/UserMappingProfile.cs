using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Application.Users.DTOs;
using TooliRent.Domain.Users;

namespace TooliRent.Application.Users.Mapping
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<UserRow, UserDto>()
                .ForCtorParam("Id",         opt => opt.MapFrom(src => src.Id))
                .ForCtorParam("Email",      opt => opt.MapFrom(src => src.Email))
                .ForCtorParam("FullName",   opt => opt.MapFrom(src => src.FullName))
                .ForCtorParam("Suspended", opt => opt.MapFrom(src => src.Suspended))
                .ForMember(d => d.Roles,         opt => opt.Ignore());
            //Roles is ignored because it is fetched through query in UsersController
        }
    }
}
