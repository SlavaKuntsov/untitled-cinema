using UserService.Domain.Enums;
using UserService.Domain.Models;

namespace UserService.Domain.Interfaces.Repositories;

public interface ITokensRepository
{
	public Task<RefreshTokenModel?> GetRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);

	public Task SetorUpdateRefreshTokenAsync(Guid userId, Role role, RefreshTokenModel newRefreshToken, CancellationToken cancellationToken);

	public Task DeleteRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
}