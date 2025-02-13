using UserService.Domain.Entities;
using UserService.Domain.Enums;

namespace UserService.Domain.Interfaces.Repositories;

public interface IUsersRepository
{
	Task<Guid> CreateAsync(UserEntity user, RefreshTokenEntity refreshToken, CancellationToken cancellationToken);
	void Delete(UserEntity userEntity, RefreshTokenEntity refreshTolkenEntity);
	Task<UserEntity?> GetAsync(Guid id, CancellationToken cancellationToken);
	Task<UserEntity?> GetAsync(string email, CancellationToken cancellationToken);
	Task<IList<UserEntity>> GetAsync(CancellationToken cancellationToken);
	Task<IList<Guid>> GetByRole(Role role, CancellationToken cancellationToken);
	Task<Role?> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken);
	Task<UserEntity?> GetWithTokenAsync(Guid id, CancellationToken cancellationToken);
	void Update(UserEntity entity);
}