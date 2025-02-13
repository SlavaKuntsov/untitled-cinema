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

	public async Task<RefreshTokenEntity?> GetAsync(string refreshToken, CancellationToken cancellationToken)
	{
		var entity = await _context
			.RefreshTokens
			.AsNoTracking()
			.FirstOrDefaultAsync(r => r.Token == refreshToken, cancellationToken);

		return entity;
	}

	public async Task<RefreshTokenEntity?> GetAsync(Guid userId, CancellationToken cancellationToken)
	{
		var entity = await _context
			.RefreshTokens
			.AsNoTracking()
			.FirstOrDefaultAsync(r => r.UserId == userId, cancellationToken);

		return entity;
	}

	public async Task CreateAsync(RefreshTokenEntity newRefreshTokenEntity, CancellationToken cancellationToken)
	{
		await _context.RefreshTokens.AddAsync(newRefreshTokenEntity, cancellationToken);
	}

	public void Update(RefreshTokenEntity refreshTolkenEntity)
	{
		_context.RefreshTokens.Attach(refreshTolkenEntity).State = EntityState.Modified;
	}
}