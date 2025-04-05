using BookingService.Application.DTOs;
using BookingService.Domain.Constants;
using BookingService.Domain.Entities;
using BookingService.Domain.Enums;
using BookingService.Domain.Interfaces.Repositories;
using BookingService.Domain.Models;
using Brokers.Interfaces;
using Brokers.Models.Request;
using Brokers.Models.Response;
using Domain.Exceptions;
using Extensions.Enums;
using MapsterMapper;
using MediatR;
using UserService.Application.Interfaces.Notification;

namespace BookingService.Application.Handlers.Commands.Bookings.CreateBooking;

public record struct CreateBookingCommand(
	Guid? UserId,
	Guid SessionId,
	IList<SeatModel> Seats) : IRequest<Guid>;

public class CreateBookingCommandHandler(
	IRabbitMQProducer rabbitMQProducer,
	ISeatsService seatsService,
	ISessionSeatsRepository sessionSeatsRepository,
	IBookingsRepository bookingsRepository,
	IMapper mapper) : IRequestHandler<CreateBookingCommand, Guid>
{
	public async Task<Guid> Handle(
		CreateBookingCommand request,
		CancellationToken cancellationToken)
	{
		if (request.Seats.Count > BookingConstants.MAX_SEATS_COUNT_PER_PERSONE)
			throw new InvalidOperationException("You can't book more than 5 seats per person");

		var sessionSeats = await sessionSeatsRepository.GetAsync(
			s => s.SessionId == request.SessionId,
			cancellationToken);

		if (sessionSeats is null)
		{
			var sessionSeatsData = new SessionSeatsRequest(request.SessionId);

			var sessionSeatsResponse = await rabbitMQProducer
				.RequestReplyAsync<SessionSeatsRequest, SessionSeatsResponse>(
					sessionSeatsData,
					Guid.NewGuid(),
					cancellationToken);

			if (!string.IsNullOrWhiteSpace(sessionSeatsResponse.Error))
				throw new NotFoundException(sessionSeatsResponse.Error);

			var newSessionSeatModel = new SessionSeatsModel(
				Guid.NewGuid(),
				request.SessionId,
				sessionSeatsResponse.Seats!,
				[]);

			sessionSeats = mapper.Map<SessionSeatsEntity>(newSessionSeatModel);
		}

		var reservedSeats = sessionSeats.ReservedSeats.Select(s => s.Id).ToHashSet();
		var requestedSeats = request.Seats.Select(s => s.Id).ToHashSet();

		if (requestedSeats.Any(id => reservedSeats.Contains(id)))
			throw new InvalidOperationException(
				"One or more requested seats are already reserved.");

		var existBooking = await bookingsRepository.GetOneAsync(
			b => b.UserId == request.UserId && b.SessionId == request.SessionId,
			cancellationToken);

		if (existBooking is not null)
		{
			var cancelledBookingSeats = existBooking.Seats.Select(s => s.Id).ToHashSet();

			if (cancelledBookingSeats.SequenceEqual(requestedSeats))
			{
				if (existBooking.Status == BookingStatus.Reserved.GetDescription())
					throw new AlreadyExistsException(
						$"Booking with id '{existBooking.Id}' already exist.");

				existBooking.Status = BookingStatus.Reserved.GetDescription();

				await bookingsRepository.DeleteAsync(
					b => b.Id == existBooking.Id,
					cancellationToken);

				await rabbitMQProducer.PublishAsync(
					mapper.Map<BookingModel>(existBooking),
					cancellationToken);

				return existBooking.Id;
			}
		}

		var bookingPriceData = new BookingPriceRequest(
			request.SessionId,
			request.Seats);

		var bookingPriceResponse = await rabbitMQProducer
			.RequestReplyAsync<BookingPriceRequest, BookingPriceResponse>(
				bookingPriceData,
				Guid.NewGuid(),
				cancellationToken);

		if (!string.IsNullOrWhiteSpace(bookingPriceResponse.Error))
			throw new NotFoundException(bookingPriceResponse.Error);

		var bookingDateNow = DateTime.UtcNow;

		var booking = new BookingModel(
			Guid.NewGuid(),
			request.UserId!.Value,
			request.SessionId,
			request.Seats,
			bookingPriceResponse.TotalPrice,
			BookingStatus.Reserved.GetDescription(),
			bookingDateNow,
			bookingDateNow);

		await rabbitMQProducer.PublishAsync(booking, cancellationToken);

		var updatedSeatsDto = default(UpdatedSeatDTO);

		foreach (var seat in request.Seats)
			updatedSeatsDto = new UpdatedSeatDTO(
				request.SessionId,
				seat.Id);

		await seatsService.NotifySeatChangedAsync(updatedSeatsDto, cancellationToken);

		return booking.Id;
	}
}