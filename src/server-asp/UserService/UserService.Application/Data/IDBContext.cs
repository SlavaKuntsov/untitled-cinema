namespace UserService.Application.Data;

public interface IDBContext
{
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}