using Microsoft.EntityFrameworkCore;

using UserService.Domain.Entities;

namespace UserService.Persistence.Repositories;

public class NotificationsRepository : INotificationsRepository
{
	private readonly UserServiceDBContext _context;

	public NotificationsRepository(UserServiceDBContext context)
	{
		_context = context;
	}

	public async Task<NotificationEntity?> GetAsync(Guid id, CancellationToken cancellationToken)
	{
		return await _context.Notifications
			.AsNoTracking()
			.Where(x => x.Id == id)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<IList<NotificationEntity>> GetByUserIdAsync(Guid id, CancellationToken cancellationToken)
	{
		return await _context.Notifications
			.AsNoTracking()
			.Where(n => n.UserId == id)
			.Where(n => n.IsDeleted == false)
			.ToListAsync(cancellationToken);
	}

	public async Task<Guid> CreateAsync(NotificationEntity notification, CancellationToken cancellationToken)
	{
		await _context.Notifications.AddAsync(notification, cancellationToken);

		return notification.Id;
	}

	public void Update(NotificationEntity notification)
	{
		_context.Notifications.Attach(notification).State = EntityState.Modified;
	}

	public void Delete(NotificationEntity notification)
	{
		_context.Notifications.Attach(notification);
		_context.Notifications.Remove(notification);

		return;
	}
}