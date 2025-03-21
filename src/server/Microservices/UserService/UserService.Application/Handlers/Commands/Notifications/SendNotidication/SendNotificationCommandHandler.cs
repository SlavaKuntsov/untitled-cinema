using MapsterMapper;

using MediatR;

using UserService.Application.Interfaces.Notification;
using UserService.Domain.Entities;
using UserService.Domain.Models;
using UserService.Persistence.Repositories;

namespace UserService.Application.Handlers.Commands.Notifications.SendNotidication;

public class SendNotificationCommandHandler(
	INotificationsRepository notificationRepository,
	INotificationService notificationService,
	IMapper mapper) : IRequestHandler<SendNotificationCommand>
{
	private readonly INotificationService _notificationService = notificationService;
	private readonly INotificationsRepository _notificationRepository = notificationRepository;
	private readonly IMapper _mapper = mapper;

	public async Task Handle(SendNotificationCommand request, CancellationToken cancellationToken)
	{
		var notification = new NotificationModel(
				Guid.NewGuid(),
				request.UserId,
				request.Message,
				DateTime.UtcNow);

		await _notificationRepository.CreateAsync(
			_mapper.Map<NotificationEntity>(notification),
			cancellationToken);

		await _notificationService.SendAsync(
			notification,
			cancellationToken);

		return;
	}
}