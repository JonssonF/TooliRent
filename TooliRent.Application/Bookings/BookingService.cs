using AutoMapper;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Application.Bookings.DTOs;
using TooliRent.Domain.Entities;
using TooliRent.Domain.Enums;
using TooliRent.Domain.Interfaces;

namespace TooliRent.Application.Bookings
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IToolReadRepository _toolRepo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public BookingService(IBookingRepository bookingRepo, IToolReadRepository toolRepo, IUnitOfWork uow, IMapper mapper)
        {
            _bookingRepo = bookingRepo;
            _toolRepo = toolRepo;
            _uow = uow;
            _mapper = mapper;
        }
        
        public async Task<(bool Ok, string? Error, BookingDetailsDto? Data)> CreateAsync(BookingCreateRequest request, string memberId, CancellationToken cancellationToken)
        {
            var requestedTools = request.ToolIds.Distinct().ToList();
           
            var count = await _toolRepo.CountExistingAsync(requestedTools, cancellationToken);
            if (count != requestedTools.Count)
            {
                return (false, "One or more tools do not exist.", null);
            }

            var start = DateTime.SpecifyKind(request.StartDate.Date, DateTimeKind.Utc);
            var end = DateTime.SpecifyKind(request.EndDate.Date, DateTimeKind.Utc);
            if (await _bookingRepo.AnyOverlappingAsync(requestedTools, start, end, cancellationToken))
            {
                return (false, "Date conflict: Atleast one of your requested tools is already booked between the dates.", null);
            }

            var booking = _mapper.Map<Booking>(request);
            booking.MemberId = memberId;

            await _bookingRepo.AddAsync(booking, cancellationToken);
            await _bookingRepo.SetToolsStatusAsync(requestedTools, ToolStatus.AwaitingPickup, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);


            var entity = await _bookingRepo.GetByIdForMemberAsync(booking.Id, memberId, cancellationToken);
            return (true, null, entity != null ? _mapper.Map<BookingDetailsDto>(entity) : null);
        }

        public async Task<List<BookingListItemDto>> GetMineAsync(string memberId, CancellationToken cancellationToken)
        {
            var entities = await _bookingRepo.GetForMemberAsync(memberId, cancellationToken);
            return _mapper.Map<List<BookingListItemDto>>(entities);
        }

        public async Task<BookingDetailsDto?> GetMineByIdAsync(int id, string memberId, CancellationToken cancellationToken)
        {
            var entity = await _bookingRepo.GetByIdForMemberAsync(id, memberId, cancellationToken);
            return entity != null ? _mapper.Map<BookingDetailsDto>(entity) : null;
        }
        public async Task<(bool Ok, string? Error)> CancelAsync(int id, string memberId, CancellationToken cancellationToken)
        {
            var booking = await _bookingRepo.GetAggregateForUpdateForMemberAsync(id, memberId, cancellationToken);
            if (booking == null)
            {
                return (false, "Booking not found.");
            }
            if (booking.Status == BookingStatus.Cancelled)
            {
                return (false, "Booking is already cancelled.");
            }
            if (DateTime.UtcNow >= booking.StartDate)
            {
                return (false, "Cannot cancel a booking that has already started or passed.");
            }
            booking.Status = BookingStatus.Cancelled;
            await _uow.SaveChangesAsync(cancellationToken);
            return (true, null);
        }
    }
}
