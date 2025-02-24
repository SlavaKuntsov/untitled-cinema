using MapsterMapper;

using MediatR;

using UserService.Domain.Models;
using UserService.Persistence.Repositories;

namespace UserService.Application.Handlers.Queries.Notifications.GetUserNotifications;

public class GetUserNotificationsQueryHandler(
	INotificationsRepository notificationsRepository,
	IMapper mapper) : IRequestHandler<GetUserNotificationsQuery, IList<NotificationModel>>
{
	private readonly INotificationsRepository _notificationsRepository = notificationsRepository;
	private readonly IMapper _mapper = mapper;

	public async Task<IList<NotificationModel>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
	{
		var entity = await _notificationsRepository.GetByUserIdAsync(
			request.Id, 
			cancellationToken);

		return _mapper.Map<IList<NotificationModel>>(entity);
	}
}