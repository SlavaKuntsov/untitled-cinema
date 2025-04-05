using MediatR;

namespace UserService.Application.Handlers.Commands.Notifications.SendNotification;

public record SendNotificationCommand(
	Guid UserId,
	string Message) : IRequest;