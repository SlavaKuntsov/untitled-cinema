using UserService.Domain.Entities;
using UserService.Domain.Enums;

namespace UserService.Domain.Interfaces.Repositories;

public interface IUsersRepository
{
	Task<Guid> CreateAsync(UserEntity user, RefreshTokenEntity refreshToken, CancellationToken cancellationToken);
	Task DeleteAsync(UserEntity entity, CancellationToken cancellationToken);
	Task<UserEntity?> GetAsync(Guid id, CancellationToken cancellationToken);
	Task<UserEntity?> GetAsync(string email, CancellationToken cancellationToken);
	Task<IList<UserEntity>> GetAsync(CancellationToken cancellationToken);
	Task<Role?> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken);
	Task<UserEntity> UpdateAsync(UserEntity entity, CancellationToken cancellationToken);
}