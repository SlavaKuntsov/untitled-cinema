using Microsoft.EntityFrameworkCore;

using UserService.Domain.Entities;
using UserService.Domain.Enums;
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

		if (entity == null)
			return null;

		return entity;
	}

	public async Task SetRefreshTokenAsync(Guid userId, Role role, RefreshTokenEntity newRefreshToken, CancellationToken cancellationToken)
	{
		var existingToken = await _context.RefreshTokens
				.FirstOrDefaultAsync(rt => rt.UserId == userId, cancellationToken);

		if (existingToken is not null)
		{
			existingToken.Token = newRefreshToken.Token;
			existingToken.ExpiresAt = newRefreshToken.ExpiresAt;
			existingToken.CreatedAt = newRefreshToken.CreatedAt;

			_context.RefreshTokens.Update(existingToken);
			return;
		}
		else
		{
			await _context.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);
		}
	}

	public async Task DeleteRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
	{
		var token = await _context
			.RefreshTokens
			.FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);

		if (token != null)
			_context.RefreshTokens.Remove(token);
	}
}