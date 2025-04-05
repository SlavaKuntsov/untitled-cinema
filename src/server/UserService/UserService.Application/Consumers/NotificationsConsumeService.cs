using System.Text;
using System.Text.Json;
using Brokers.Interfaces;
using Brokers.Models.DTOs;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UserService.Application.Handlers.Commands.Notifications.SendNotification;

namespace BookingService.Application.Consumers;

public class NotificationsConsumeService(
	IRabbitMQConsumer<NotificationDto> rabbitMQConsumer,
	IServiceScopeFactory serviceScopeFactory,
	ILogger<NotificationsConsumeService> logger) : BackgroundService
{
	protected override Task ExecuteAsync(CancellationToken cancellationToken)
	{
		rabbitMQConsumer.ConsumeAsync(
			async (_, args) =>
			{
				var notification = JsonSerializer.Deserialize<NotificationDto>(
					Encoding.UTF8.GetString(args.Body.ToArray()));

				using var scope = serviceScopeFactory.CreateScope();
				var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

				await mediator.Send(
					new SendNotificationCommand(notification.UserId, notification.Message),
					cancellationToken);
			});

		return Task.CompletedTask;
	}
}