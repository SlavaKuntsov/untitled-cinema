using UserService.Domain.Entities;
using UserService.Domain.Enums;

namespace UserService.Domain.Interfaces.Repositories;

public interface ITokensRepository
{
	Task<RefreshTokenEntity?> GetAsync(string refreshToken, CancellationToken cancellationToken);
	Task<RefreshTokenEntity?> GetAsync(Guid userId, CancellationToken cancellationToken);
	Task CreateAsync(RefreshTokenEntity newRefreshTokenEntity, CancellationToken cancellationToken);
	void Update(RefreshTokenEntity refreshTolkenEntity);
}