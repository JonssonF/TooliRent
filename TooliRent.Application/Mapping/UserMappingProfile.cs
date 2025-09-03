using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Application.Users;
using TooliRent.Infrastructure.Identity;

namespace TooliRent.Application.Mapping
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<AppUser, UserDto>()
                .ForCtorParameter("Id", opt => opt.MapFrom(src => src.Id))
                .ForCtorParameter("Email", opt => opt.MapFrom(src => src.Email))
                .ForCtorParameter("FullName", opt => opt.MapFrom(src => src.FullName))
                .ForCtorParameter("Roles", opt => opt.Ignore());

            //Roles is ignored because it is fetched through LINQ in AppDbContext
        }
    }
}
