using MediatR;

namespace UserService.Application.Handlers.Commands.Notifications.SendNotidication;

public record SendNotificationCommand(
	Guid UserId,
	string Message) : IRequest;