using Domain.Exceptions;
using MediatR;

using UserService.Persistence.Repositories;

namespace UserService.Application.Handlers.Commands.Notifications.DeleteNotification;

public class DeleteNotificationCommandHandler(
	INotificationsRepository notificationsRepository) : IRequestHandler<DeleteNotificationCommand>
{
	private readonly INotificationsRepository _notificationsRepository = notificationsRepository;

	public async Task Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
	{
		var notification = await _notificationsRepository.GetAsync(request.Id, cancellationToken)
			?? throw new NotFoundException($"Notification with id {request.Id} doesn't exists");

		notification.IsDeleted = true;

		_notificationsRepository.Update(notification);

		return;
	}
}