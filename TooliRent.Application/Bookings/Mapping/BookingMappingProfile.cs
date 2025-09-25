using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Application.Bookings.DTOs;
using TooliRent.Domain.Entities;
using TooliRent.Domain.Enums;

namespace TooliRent.Application.Bookings.Mapping
{
    public class BookingMappingProfile : Profile
    {
        // Mapping profile for converting Booking entities to BookingDetailsDto objects.
        public BookingMappingProfile()
        {
            // Item
            CreateMap<BookingItem, BookingItemDto>()
                .ForCtorParam("ToolId", o => o.MapFrom(s => s.ToolId))
                .ForCtorParam("ToolName", o => o.MapFrom(s => s.Tool!.Name))
                .ForCtorParam("CategoryName", o => o.MapFrom(s => s.Tool!.Category != null ? s.Tool!.Category!.Name : null))
                .ForCtorParam("Status", o => o.MapFrom(s => s.Tool!.Status.ToString()));

            // Details
            CreateMap<Booking, BookingDetailsDto>()
                .ForMember(d => d.TotalDays, o => o.MapFrom(s => (s.EndDate - s.StartDate).TotalDays))
                .ForMember(d => d.Items, o => o.MapFrom(s => s.Items));

            // ListItem
            CreateMap<Booking, BookingListItemDto>()
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.ToolCount, o => o.MapFrom(s => s.Items.Count))
                .ForMember(d => d.CanBeCancelled, o => o.MapFrom(s => DateTime.UtcNow < s.StartDate && s.Status != BookingStatus.Cancelled));
        }
    }
}
