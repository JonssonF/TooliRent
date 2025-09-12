using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Application.Bookings.DTOs;
using TooliRent.Domain.Entities;

namespace TooliRent.Application.Bookings.Mapping
{
    public class BookingMappingProfile : Profile
    {
        public BookingMappingProfile()
        {
            CreateMap<Booking, BookingDetailsDto>()
                .ForMember(d => d.Items, opt => opt.MapFrom(src =>
                    src.Items.Select(i => new BookingItemDto(
                        i.ToolId,
                        i.Tool!.Name,
                        i.Tool.Category != null ? i.Tool.Category.Name : null,
                        i.Tool.Status.ToString()
                        )).ToList()
                    ));
        }
    }
}
