using BookingService.Application.DTOs;
using BookingService.Application.Handlers.Commands.Seats.UpdateSeats;
using BookingService.Domain.Enums;
using BookingService.Domain.Interfaces.Repositories;
using BookingService.Domain.Models;
using Brokers.Interfaces;
using Brokers.Models.DTOs;
using Domain.Enums;
using Extensions.Enums;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UserService.Application.Interfaces.Notification;

namespace BookingService.Application.Jobs.Bookings;

public class CancelBookingAfterExpiredJob(
	IRabbitMQProducer rabbitMQProducer,
	ISeatsService seatsService,
	IBookingsRepository bookingsRepository,
	IServiceScopeFactory serviceScopeFactory,
	ILogger<CancelBookingAfterExpiredJob> logger)
{
	public async Task ExecuteAsync(
		Guid bookingId,
		Guid sessionId,
		IList<SeatModel> seats,
		Guid userId,
		CancellationToken cancellationToken)
	{
		logger.LogInformation("CancelBookingAfterExpired job for '{BookingId}' started", bookingId);

		// Change booking status for the canceled if it was not paid 
		await bookingsRepository.UpdateStatusAsync(
			bookingId,
			BookingStatus.Cancelled.GetDescription(),
			cancellationToken);
		//


		foreach (var seat in seats)
		{
			var updatedSeatsDto = new UpdatedSeatDTO(
				sessionId,
				seat);

			await seatsService.NotifySeatChangedAsync(updatedSeatsDto, cancellationToken);
		}

		using var scope = serviceScopeFactory.CreateScope();
		var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

		await mediator.Send(
			new UpdateSeatsCommand(
				sessionId,
				seats,
				false),
			cancellationToken);

		var notification = new NotificationDto(
			userId,
			"You did not pay for the reservation, it was canceled.",
			NotificationType.Error.GetDescription());
		
		logger.LogInformation("Notification type {Type} in cancel booking", NotificationType.Error.GetDescription());

		await rabbitMQProducer.PublishAsync(notification, cancellationToken);

		logger.LogInformation("CancelBookingAfterExpired job for '{BookingId}' finished", bookingId);
	}
}