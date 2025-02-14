using UserService.Domain.Entities;
using UserService.Domain.Enums;

namespace UserService.Domain.Interfaces.Repositories;

public interface IUsersRepository
{
	Task<Guid> CreateAsync(UserEntity user, RefreshTokenEntity refreshToken, CancellationToken cancellationToken);
	Task<UserEntity?> GetAsync(Guid id, CancellationToken cancellationToken);
	Task<IList<UserEntity>> GetAsync(CancellationToken cancellationToken);
	Task<Guid?> GetIdAsync(string email, CancellationToken cancellationToken);
	Task<IList<Guid>> GetByRole(Role role, CancellationToken cancellationToken);
	Task<Role?> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken);
	Task<(Guid?, string?)> GetIdWithPasswordAsync(string email, CancellationToken cancellationToken);
	Task<(Guid?, Role?, Guid?)> GetIdWithRoleAndTokenAsync(Guid userId, CancellationToken cancellationToken);
	void Update(UserEntity entity);
	void Delete(Guid userId, Guid refreshTokenId);
}