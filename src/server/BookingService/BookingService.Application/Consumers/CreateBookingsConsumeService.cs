using System.Text;
using System.Text.Json;
using BookingService.Application.Handlers.Commands.Bookings.SaveBooking;
using BookingService.Application.Handlers.Commands.Seats.UpdateSeats;
using BookingService.Application.Handlers.Query.Bookings.GetUserBookings;
using BookingService.Application.Jobs.Bookings;
using BookingService.Domain.Constants;
using BookingService.Domain.Enums;
using BookingService.Domain.Interfaces.Repositories;
using BookingService.Domain.Models;
using Brokers.Interfaces;
using Brokers.Models.DTOs;
using Domain.Enums;
using Extensions.Enums;
using Grpc.Core;
using Hangfire;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BookingService.Application.Consumers;

public class CreateBookingsConsumeService(
	IRabbitMQConsumer<BookingModel> rabbitMQConsumer,
	IRabbitMQProducer rabbitMQProducer,
	IServiceScopeFactory serviceScopeFactory,
	IBackgroundJobClient backgroundJobClient,
	ILogger<CreateBookingsConsumeService> logger) : BackgroundService
{
	protected override Task ExecuteAsync(CancellationToken cancellationToken)
	{
		rabbitMQConsumer.ConsumeAsync(
			async (_, args) =>
			{
				logger.LogInformation("Starting consume booking create.");

				var booking = JsonSerializer.Deserialize<BookingModel>(
					Encoding.UTF8.GetString(args.Body.ToArray()));

				using var scope = serviceScopeFactory.CreateScope();
				var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
				var repository = scope.ServiceProvider.GetRequiredService<IBookingsRepository>();

				var existBooking = await repository.GetOneAsync(
					x => x.UserId == booking!.UserId && x.Status == BookingStatus.Reserved.GetDescription(), 
					cancellationToken);

				if (existBooking is not null)
				{
					logger.LogInformation("Booking with {Id} is already exists.", existBooking.Id);
					return;
				}

				await mediator.Send(
					new UpdateSeatsCommand(
						booking!.SessionId,
						booking!.Seats,
						true),
					cancellationToken);

				await mediator.Send(
					new SaveBookingCommand(booking),
					cancellationToken);

				var notification = new NotificationDto(
					booking.UserId,
					$"You have {JobsConstants.AfterBookingExpiredTest} to pay tickets, " +
					$"otherwise the booking can be canceled.",
					NotificationType.Success.GetDescription());
				
				logger.LogInformation("Notification type {Type} in create booking", NotificationType.Success.GetDescription());

				await rabbitMQProducer.PublishAsync(notification, cancellationToken);

				backgroundJobClient.Schedule<CancelBookingAfterExpiredJob>(
					b => b.ExecuteAsync(
						booking.Id,
						booking!.SessionId,
						booking!.Seats,
						booking!.UserId,
						cancellationToken),
					JobsConstants.AfterBookingExpiredTest);

				logger.LogInformation("Processed consume booking create.");
			});

		return Task.CompletedTask;
	}
}