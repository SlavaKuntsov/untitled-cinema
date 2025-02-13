using MediatR;

namespace UserService.Application.Handlers.Commands.Notifications.DeleteNotification;

public record DeleteNotificationCommand(Guid Id) : IRequest;