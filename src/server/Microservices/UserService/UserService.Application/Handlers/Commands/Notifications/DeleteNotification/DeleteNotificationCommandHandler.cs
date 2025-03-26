using Domain.Exceptions;
using MediatR;
using UserService.Application.Data;
using UserService.Persistence.Repositories;

namespace UserService.Application.Handlers.Commands.Notifications.DeleteNotification;

public class DeleteNotificationCommandHandler(
	INotificationsRepository notificationsRepository,
	IDBContext dbContext) : IRequestHandler<DeleteNotificationCommand>
{
	public async Task Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
	{
		var notification = await notificationsRepository.GetAsync(request.Id, cancellationToken)
			?? throw new NotFoundException($"Notification with id {request.Id} doesn't exists");

		notification.IsDeleted = true;

		notificationsRepository.Update(notification);
		
		await dbContext.SaveChangesAsync(cancellationToken);
	}
}