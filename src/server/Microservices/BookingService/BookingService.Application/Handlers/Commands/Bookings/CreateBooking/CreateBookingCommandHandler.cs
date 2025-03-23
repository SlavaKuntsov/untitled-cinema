using BookingService.Domain.Constants;
using BookingService.Domain.Entities;
using BookingService.Domain.Enums;
using BookingService.Domain.Exceptions;
using BookingService.Domain.Extensions;
using BookingService.Domain.Interfaces.Repositories;
using BookingService.Domain.Models;

using Brokers.Interfaces;
using Brokers.Models.Request;
using Brokers.Models.Response;

using MapsterMapper;

using MediatR;

namespace BookingService.Application.Handlers.Commands.Bookings.CreateBooking;

public class CreateBookingCommandHandler(
	IRabbitMQProducer rabbitMQProducer,
	ISessionSeatsRepository sessionSeatsRepository,
	IBookingsRepository bookingsRepository,
	IMapper mapper) : IRequestHandler<CreateBookingCommand, Guid>
{
	private readonly IRabbitMQProducer _rabbitMQProducer = rabbitMQProducer;
	private readonly ISessionSeatsRepository _sessionSeatsRepository = sessionSeatsRepository;
	private readonly IBookingsRepository _bookingsRepository = bookingsRepository;
	private readonly IMapper _mapper = mapper;

	public async Task<Guid> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
	{
		if (request.Seats.Count > BookingConstants.MAX_SEATS_COUNT_PER_PERSONE)
			throw new InvalidOperationException("You can't book more than 5 seats per person");

		var sessionSeats = await _sessionSeatsRepository.GetAsync(
			s => s.SessionId == request.SessionId, cancellationToken);

		if (sessionSeats is null)
		{
			var sessionSeatsData = new SessionSeatsRequest(request.SessionId);

			var sessionSeatsResponse = await _rabbitMQProducer
				.RequestReplyAsync<SessionSeatsRequest, SessionSeatsResponse<SeatModel>>(
				sessionSeatsData,
				Guid.NewGuid(),
				cancellationToken);

			if (!string.IsNullOrWhiteSpace(sessionSeatsResponse.Error))
				throw new NotFoundException(sessionSeatsResponse.Error);

			var sessionSeatDateNow = DateTime.UtcNow;

			var newSessionSeatModel = new SessionSeatsModel(
				Guid.NewGuid(),
				request.SessionId,
				sessionSeatsResponse.Seats!,
				[],
				sessionSeatDateNow);

			sessionSeats = _mapper.Map<SessionSeatsEntity>(newSessionSeatModel);
		}

		var reservedSeats = sessionSeats.ReservedSeats.Select(s => s.Id).ToHashSet();
		var requestedSeats = request.Seats.Select(s => s.Id).ToHashSet();

		if (requestedSeats.Any(id => reservedSeats.Contains(id)))
			throw new InvalidOperationException("One or more requested seats are already reserved.");

		var existBooking = await _bookingsRepository.GetOneAsync(
				b => b.UserId == request.UserId && b.SessionId == request.SessionId,
				cancellationToken);

		if (existBooking is not null)
		{
			var cancelledBookingSeats = existBooking.Seats.Select(s => s.Id).ToHashSet();

			if (cancelledBookingSeats.SequenceEqual(requestedSeats))
			{
				if (existBooking.Status == BookingStatus.Reserved.GetDescription())
					throw new AlreadyExistsException($"Booking with id '{existBooking.Id}' already exist.");

				existBooking.Status = BookingStatus.Reserved.GetDescription();

				await _bookingsRepository.DeleteAsync(
					b => b.Id == existBooking.Id,
					cancellationToken);

				await _rabbitMQProducer.PublishAsync(
					_mapper.Map<BookingModel>(existBooking),
					cancellationToken);

				return existBooking.Id;
			}
		}

		var bookingPriceData = new BookingPriceRequest<SeatModel>(
			request.SessionId,
			request.Seats);

		var bookingPriceResponse = await _rabbitMQProducer
			.RequestReplyAsync<BookingPriceRequest<SeatModel>, BookingPriceResponse>(
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

		await _rabbitMQProducer.PublishAsync(booking, cancellationToken);

		return booking.Id;
	}
}