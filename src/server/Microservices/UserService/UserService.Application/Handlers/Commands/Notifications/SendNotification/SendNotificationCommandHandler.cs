using MapsterMapper;
using MediatR;
using UserService.Application.Data;
using UserService.Application.Handlers.Commands.Notifications.SendNotidication;
using UserService.Application.Interfaces.Notification;
using UserService.Domain.Entities;
using UserService.Domain.Models;
using UserService.Persistence.Repositories;

namespace UserService.Application.Handlers.Commands.Notifications.SendNotification;

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
			request.UserId,
			request.Message,
			DateTime.UtcNow);

		await notificationRepository.CreateAsync(
			mapper.Map<NotificationEntity>(notification),
			cancellationToken);

		await notificationService.SendAsync(
			notification,
			cancellationToken);

		await dbContext.SaveChangesAsync(cancellationToken);
	}
}