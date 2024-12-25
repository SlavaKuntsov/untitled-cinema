using UserService.Domain.Enums;
using UserService.Domain.Models.Auth;

namespace UserService.Domain.Interfaces.Repositories;

public interface ITokensRepository
{
	public Task<RefreshTokenModel?> GetRefreshToken(string refreshToken, CancellationToken cancellationToken);

	public Task UpdateRefreshToken(Guid userId, Role role, RefreshTokenModel newRefreshToken, CancellationToken cancellationToken);

	public Task DeleteRefreshToken(string refreshToken, CancellationToken cancellationToken);
}