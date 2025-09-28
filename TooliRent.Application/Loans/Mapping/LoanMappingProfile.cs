using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Application.Loans.Responses;
using TooliRent.Domain.Entities;

namespace TooliRent.Application.Loans.Mapping
{
    public class LoanMappingProfile : Profile
    {
        public LoanMappingProfile()
        {
            // Loan -> LoanResponse 
            CreateMap<Loan, LoanResponse>()
                .ForCtorParam("BookingId", opt => opt.MapFrom((src, ctx) => (int)ctx.Items["BookingId"]))
                .ForCtorParam("LoanId", opt => opt.MapFrom(src => src.Id))
                .ForCtorParam("ToolIds", opt => opt.MapFrom(src => src.Items.Select(i => i.ToolId).ToList()))
                .ForCtorParam("OldBookingStatus", opt => opt.MapFrom((src, ctx) => (string)ctx.Items["OldBookingStatus"]))
                .ForCtorParam("NewBookingStatus", opt => opt.MapFrom((src, ctx) => (string)ctx.Items["NewBookingStatus"]))
                .ForCtorParam("TimeStamp", opt => opt.MapFrom((src, ctx) => (DateTime)ctx.Items["TimeStamp"]));

            // Loan -> ActiveLoanResponse 
            CreateMap<Loan, ActiveLoanResponse>()
                .ForCtorParam("LoanId", opt => opt.MapFrom(s => s.Id))
                .ForCtorParam("BookingId", opt => opt.MapFrom(s => s.BookingId))
                .ForCtorParam("PickUpAt", opt => opt.MapFrom(s => s.PickUpAt))
                .ForCtorParam("DueAt", opt => opt.MapFrom(s => s.DueAt))
                .ForCtorParam("IsOverDue", opt => opt.MapFrom(s => s.IsOverDue))
                .ForCtorParam("ToolIds", opt => opt.MapFrom(s => s.Items.Select(i => i.ToolId).ToList()));
        }
    }
}
