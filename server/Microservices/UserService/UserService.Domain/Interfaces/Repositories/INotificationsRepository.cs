using UserService.Domain.Entities;

namespace UserService.Persistence.Repositories;

public interface INotificationsRepository
{
	Task<Guid> CreateAsync(NotificationEntity notification, CancellationToken cancellationToken);
	void Delete(NotificationEntity notification);
	Task<NotificationEntity?> GetAsync(Guid id, CancellationToken cancellationToken);
	Task<IList<NotificationEntity>> GetByUserIdAsync(Guid id, CancellationToken cancellationToken);
}