using Brokers.Models.DTOs;
using Domain.Enums;
using Extensions.Enums;
using MapsterMapper;
using MediatR;
using UserService.Application.Data;
using UserService.Application.Interfaces.Notification;
using UserService.Domain.Entities;
using UserService.Domain.Models;
using UserService.Persistence.Repositories;

namespace UserService.Application.Handlers.Commands.Notifications.SendNotification;

public record SendNotificationCommand(
	NotificationDto notification) : IRequest;

public class SendNotificationCommandHandler(
	INotificationsRepository notificationRepository,
	INotificationService notificationService,
	IDBContext dbContext,
	IMapper mapper) : IRequestHandler<SendNotificationCommand>
{
	public async Task Handle(SendNotificationCommand request, CancellationToken cancellationToken)
	{
		var notification = new NotificationModel(
			Guid.NewGuid(),
			request.notification.UserId,
			request.notification.Message,
			DateTime.UtcNow);

		await notificationRepository.CreateAsync(
			mapper.Map<NotificationEntity>(notification),
			cancellationToken);

		await notificationService.SendAsync(
			request.notification,
			cancellationToken);

		await dbContext.SaveChangesAsync(cancellationToken);
	}
}