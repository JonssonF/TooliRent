using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Application.Tools.DTOs;
using TooliRent.Domain.Entities;

namespace TooliRent.Application.Tools.Mapping
{
    public class ToolMappingProfile : Profile
    {
        public ToolMappingProfile() 
        {
            CreateMap<Tool, ToolListItemDto>()
                .ForCtorParam("Id", o => o.MapFrom(s => s.Id))
                .ForCtorParam("Name", o => o.MapFrom(s => s.Name))
                .ForCtorParam("CategoryName", o => o.MapFrom(s => s.Category.Name))
                .ForCtorParam("PricePerDay", o => o.MapFrom(s => s.PricePerDay))
                .ForCtorParam("Status", o => o.MapFrom(s => s.Status.ToString()));


            CreateMap<Tool, ToolDetailDto>()
                .ForCtorParam("Id", o => o.MapFrom(s => s.Id))
                .ForCtorParam("Name", o => o.MapFrom(s => s.Name))
                .ForCtorParam("Manufacturer", o => o.MapFrom(s => s.Manufacturer))
                .ForCtorParam("CategoryName", o => o.MapFrom(s => s.Category.Name))
                .ForCtorParam("Description", o => o.MapFrom(s => s.Description))
                .ForCtorParam("SerialNumber", o => o.MapFrom(s => s.SerialNumber))
                .ForCtorParam("PricePerDay", o => o.MapFrom(s => s.PricePerDay))
                .ForCtorParam("Status", o => o.MapFrom(s => s.Status.ToString()));

        }
    }
}
