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
using TooliRent.Domain.Tools;

namespace TooliRent.Application.Bookings
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IToolReadRepository _toolRepo;

        public BookingService(IBookingRepository bookingRepo, IToolReadRepository toolRepo)
        {
            _bookingRepo = bookingRepo;
            _toolRepo = toolRepo;
        }

        private static BookingDetailsDto ToDetailsDto(Booking b) =>
            new BookingDetailsDto
            {
                Id = b.Id,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                Status = b.Status,
                TotalDays = (b.EndDate - b.StartDate).TotalDays,
                Items = b.Items.Select(i => new BookingItemDto(
                    i.ToolId,
                    i.Tool!.Name,
                    i.Tool.Category?.Name,
                    i.Tool.Status.ToString()
                )).ToList()
            };
        private static BookingListItemDto ToListItemDto(Booking b) =>
            new BookingListItemDto
            {
                Id = b.Id,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                Status = b.Status.ToString(),
                ToolCount = b.Items.Count,
                CanBeCancelled = DateTime.UtcNow < b.StartDate && b.Status != BookingStatus.Cancelled
            };


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

            var booking = new Booking
            {
                MemberId = memberId,
                StartDate = start,
                EndDate = end,
                Status = BookingStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                Items = requestedTools.Select(tid => new BookingItem
                {
                    ToolId = tid
                    
                }).ToList()
            };

            await _bookingRepo.AddAsync(booking, cancellationToken);
            await _bookingRepo.SaveChangesAsync(cancellationToken);

            await _bookingRepo.SetToolsStatusAsync(requestedTools, ToolStatus.AwaitingPickup, cancellationToken);

            var entity = await _bookingRepo.GetByIdForMemberAsync(booking.Id, memberId, cancellationToken);
            return (true, null, entity != null ? ToDetailsDto(entity) : null);
        }

        public async Task<List<BookingListItemDto>> GetMineAsync(string memberId, CancellationToken cancellationToken)
        {
            var entities = await _bookingRepo.GetForMemberAsync(memberId, cancellationToken);
            return entities.Select(ToListItemDto).ToList();
        }

        public async Task<BookingDetailsDto?> GetMineByIdAsync(int id, string memberId, CancellationToken cancellationToken)
        {
            var entity = await _bookingRepo.GetByIdForMemberAsync(id, memberId, cancellationToken);
            return entity != null ? ToDetailsDto(entity) : null;
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
            await _bookingRepo.SaveChangesAsync(cancellationToken);
            return (true, null);
        }
    }
}
