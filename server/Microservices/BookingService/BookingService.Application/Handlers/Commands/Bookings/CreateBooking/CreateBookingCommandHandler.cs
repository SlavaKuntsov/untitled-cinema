using BookingService.Domain.Enums;
using BookingService.Domain.Models;

using Brokers.Interfaces;

using MapsterMapper;

using MediatR;

namespace BookingService.Application.Handlers.Commands.Bookings.CreateBooking;

public class CreateBookingCommandHandler(
	IRabbitMQProducer rabbitMQProducer,
	IMapper mapper) : IRequestHandler<CreateBookingCommand, Guid>
{
	private readonly IRabbitMQProducer _rabbitMQProducer = rabbitMQProducer;
	private readonly IMapper _mapper = mapper;

	public async Task<Guid> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
	{
		// 1. Validate (check data in other services)
		//		Return error ||	Go on
		// 2. Publish in queue

		// Validate
		//if (await _authGrpcService.CheckExistAsync(request.UserId, cancellationToken))
		//	throw new NotFoundException($"User with id '{request.UserId}' doesn't exists");

		//var existSessionAndSeats = await _movieGrpcService.CheckExistAsync(request.SessionId, cancellationToken)
		//	throw new NotFoundException($"Session with id '{request.SessionId}' doesn't exists");
		//

		DateTime dateNow = DateTime.UtcNow;

		var booking = new BookingModel(
			Guid.NewGuid(),
			request.UserId, // validate
			request.SessionId, // validate
			[], // validate
			10.0m,  // from Booking service
			BookingStatus.Reserved,
			dateNow,
			dateNow);

		await _rabbitMQProducer.PublishAsync(booking, cancellationToken);

		return booking.Id;
	}
}