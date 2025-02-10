using Microsoft.EntityFrameworkCore;

using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Persistence.Repositories;

public class TokensRepository : ITokensRepository
{
	private readonly UserServiceDBContext _context;

	public TokensRepository(UserServiceDBContext context)
	{
		_context = context;
	}

	public async Task<RefreshTokenEntity?> GetRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
	{
		var entity = await _context
			.RefreshTokens
			.AsNoTracking()
			.FirstOrDefaultAsync(r => r.Token == refreshToken, cancellationToken);

		return entity;
	}

	public async Task<RefreshTokenEntity?> GetRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
	{
		var entity = await _context
			.RefreshTokens
			.AsNoTracking()
			.FirstOrDefaultAsync(r => r.UserId == userId, cancellationToken);

		return entity;
	}

	public async Task AddRefreshTokenAsync(RefreshTokenEntity newRefreshTokenEntity, CancellationToken cancellationToken)
	{
		await _context.RefreshTokens.AddAsync(newRefreshTokenEntity, cancellationToken);
	}

	public void UpdateRefreshToken(RefreshTokenEntity refreshTokenEntity)
	{
		_context.RefreshTokens.Attach(refreshTokenEntity).State = EntityState.Modified;
	}
}