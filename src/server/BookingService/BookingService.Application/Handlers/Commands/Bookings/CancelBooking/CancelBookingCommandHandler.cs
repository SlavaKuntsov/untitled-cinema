using BookingService.Application.DTOs;
using BookingService.Application.Interfaces.Seats;
using BookingService.Domain.Enums;
using BookingService.Domain.Interfaces.Repositories;
using BookingService.Domain.Models;
using Domain.Exceptions;
using Extensions.Enums;
using MapsterMapper;
using MediatR;

namespace BookingService.Application.Handlers.Commands.Bookings.CancelBooking;

public class CancelBookingCommandHandler(
	IBookingsRepository bookingsRepository,
	ISeatsService seatsService,
	IMapper mapper) : IRequestHandler<CancelBookingCommand, BookingModel>
{
	public async Task<BookingModel> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
	{
		var existBooking = await bookingsRepository.GetOneAsync(
			b => b.Id == request.Id
				&& b.Status == BookingStatus.Reserved.GetDescription(),
			cancellationToken)
							?? throw new NotFoundException($"Booking with id '{request.Id}' doesn't exists");

		if (existBooking.Status == BookingStatus.Cancelled.GetDescription())
			throw new InvalidOperationException($"Booking with id '{existBooking.Id}' already cancelled.");

		await bookingsRepository.UpdateStatusAsync(
			request.Id,
			BookingStatus.Cancelled.GetDescription(),
			cancellationToken);

		var booking = await bookingsRepository.GetOneAsync(
			b => b.Id == request.Id,
			cancellationToken: cancellationToken);

		existBooking.Status = BookingStatus.Cancelled.GetDescription();
		
		foreach (var seat in booking.Seats)
		{
			var updatedSeatsDto = new UpdatedSeatDTO(
				booking.SessionId,
				new SeatModel(seat.Id, seat.Row, seat.Column));
		
			await seatsService.NotifySeatChangedAsync(updatedSeatsDto, cancellationToken);
		}

		return mapper.Map<BookingModel>(existBooking);
	}
}