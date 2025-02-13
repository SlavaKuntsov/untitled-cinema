using MediatR;

using UserService.Domain.Models;

namespace UserService.Application.Handlers.Queries.Notifications.GetUserNotifications;

public record GetUserNotificationsQuery(Guid Id) : IRequest<IList<NotificationModel>>;