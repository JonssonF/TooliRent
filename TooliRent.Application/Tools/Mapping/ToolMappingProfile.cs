using AutoMapper;
using TooliRent.Application.Tools.DTOs;
using TooliRent.Domain.Entities;
using TooliRent.Domain.Tools;

namespace TooliRent.Application.Tools.Mapping
{
    public class ToolMappingProfile : Profile
    {
        public ToolMappingProfile() 
        {
            CreateMap<ToolListRow, ToolListItemDto>();
            CreateMap<ToolDetailRow, ToolDetailDto>();

            CreateMap<Tool, ToolListItemDto>()
                .ForCtorParam("Id", o => o.MapFrom(s => s.Id))
                .ForCtorParam("Name", o => o.MapFrom(s => s.Name))
                .ForCtorParam("CategoryName", o => o.MapFrom(s => s.Category != null ? s.Category.Name : null))
                .ForCtorParam("Status", o => o.MapFrom(s => s.Status))
                .ForCtorParam("PricePerDay", o => o.MapFrom(s => s.PricePerDay));

            CreateMap<Tool, ToolDetailDto>()
                .ForCtorParam("Id", o => o.MapFrom(s => s.Id))
                .ForCtorParam("Name", o => o.MapFrom(s => s.Name))
                .ForCtorParam("Description", o => o.MapFrom(s => s.Description))
                .ForCtorParam("Manufacturer", o => o.MapFrom(s => s.Manufacturer))
                .ForCtorParam("CategoryName", o => o.MapFrom(s => s.Category != null ? s.Category.Name : null))
                .ForCtorParam("Status", o => o.MapFrom(s => s.Status))
                .ForCtorParam("LastMaintenanceDate", o => o.MapFrom(_ => (DateTime?)null))
                .ForCtorParam("NextMaintenanceDate", o => o.MapFrom(_ => (DateTime?)null))
                .ForCtorParam("PricePerDay", o => o.MapFrom(s => s.PricePerDay))
                .ForCtorParam("TotalLoans", o => o.MapFrom(_ => 0));

        }
    }
}
