using UserService.Domain.Entities;
using UserService.Domain.Enums;

namespace UserService.Domain.Interfaces.Repositories;

public interface ITokensRepository
{
	Task DeleteRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
	Task<RefreshTokenEntity?> GetRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
	Task<RefreshTokenEntity?> GetRefreshTokenAsync(Guid userId, CancellationToken cancellationToken);
	Task AddRefreshTokenAsync(RefreshTokenEntity newRefreshTokenEntity, CancellationToken cancellationToken);
	void UpdateRefreshToken(RefreshTokenEntity refreshTolkenEntity);
}