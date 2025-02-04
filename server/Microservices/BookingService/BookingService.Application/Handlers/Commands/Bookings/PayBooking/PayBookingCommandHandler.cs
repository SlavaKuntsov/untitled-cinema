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

namespace BookingService.Application.Handlers.Commands.Bookings.PayBooking;

public class PayBookingCommandHandler(
	IRabbitMQProducer rabbitMQProducer,
	IBookingsRepository bookingsRepository,
	IMapper mapper) : IRequestHandler<PayBookingCommand, BookingModel>
{
	private readonly IRabbitMQProducer _rabbitMQProducer = rabbitMQProducer;
	private readonly IBookingsRepository _bookingsRepository = bookingsRepository;
	private readonly IMapper _mapper = mapper;

	public async Task<BookingModel> Handle(PayBookingCommand request, CancellationToken cancellationToken)
	{
		var existBooking = await _bookingsRepository.GetOneAsync(
			b => b.Id == request.BookingId,
			cancellationToken)
			?? throw new NotFoundException($"Booking with id '{request.BookingId}' doesn't exists");

		if(existBooking.UserId != request.UserId)
			throw new InvalidOperationException($"Booking and Request user id's are mismatch.");

		if (existBooking.Status == BookingStatus.Paid.GetDescription())
			throw new InvalidOperationException($"Booking with id '{existBooking.Id}' already paid.");

		var data = new BookingPayRequest(
			request.UserId,
			existBooking.TotalPrice);

		var response = await _rabbitMQProducer.RequestReplyAsync<BookingPayRequest, BookingPayResponse>(
			data,
			Guid.NewGuid(),
			cancellationToken);

		if (!string.IsNullOrWhiteSpace(response.Error))
			throw new InvalidOperationException(response.Error);

		await _bookingsRepository.UpdateStatusAsync(
			request.BookingId,
			BookingStatus.Paid.GetDescription(),
			cancellationToken);

		existBooking.Status = BookingStatus.Paid.GetDescription();

		return _mapper.Map<BookingModel>(existBooking);
	}
}