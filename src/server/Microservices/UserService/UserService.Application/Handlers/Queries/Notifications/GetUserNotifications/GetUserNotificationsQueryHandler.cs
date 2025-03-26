using MapsterMapper;

using MediatR;

using UserService.Domain.Models;
using UserService.Persistence.Repositories;

namespace UserService.Application.Handlers.Queries.Notifications.GetUserNotifications;

public class GetUserNotificationsQueryHandler(
	INotificationsRepository notificationsRepository,
	IMapper mapper) : IRequestHandler<GetUserNotificationsQuery, IList<NotificationModel>>
{
	public async Task<IList<NotificationModel>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
	{
		var entity = await notificationsRepository.GetByUserIdAsync(
			request.Id, 
			cancellationToken);

		return mapper.Map<IList<NotificationModel>>(entity);
	}
}