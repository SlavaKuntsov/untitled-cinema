using MapsterMapper;

using Microsoft.EntityFrameworkCore;

using UserService.Domain.Enums;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Models;
using UserService.Persistence.Entities;

namespace UserService.Persistence.Repositories;

public class TokensRepository : ITokensRepository
{
	private readonly UserServiceDBContext _context;
	private readonly IMapper _mapper;

	public TokensRepository(UserServiceDBContext context, IMapper mapper)
	{
		_context = context;
		_mapper = mapper;
	}

	public async Task<RefreshTokenModel?> GetRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
	{
		var entity = await _context
			.RefreshTokens
			.AsNoTracking()
			.FirstOrDefaultAsync(r => r.Token == refreshToken, cancellationToken);

		if (entity == null)
			return null;

		return _mapper.Map<RefreshTokenModel>(entity);
	}

	public async Task SetorUpdateRefreshTokenAsync(Guid userId, Role role, RefreshTokenModel newRefreshToken, CancellationToken cancellationToken)
	{
		var existingToken = await _context.RefreshTokens
				.FirstOrDefaultAsync(rt => rt.UserId == userId, cancellationToken);

		if (existingToken is not null)
		{
			existingToken.Token = newRefreshToken.Token;
			existingToken.ExpiresAt = newRefreshToken.ExpiresAt;
			existingToken.CreatedAt = newRefreshToken.CreatedAt;

			_context.RefreshTokens.Update(existingToken);
			await _context.SaveChangesAsync(cancellationToken);
			return;
		}
		else
		{
			var entity = _mapper.Map<RefreshTokenEntity>(newRefreshToken);

			await _context.RefreshTokens.AddAsync(entity, cancellationToken);
			await _context.SaveChangesAsync(cancellationToken);
		}
	}

	public async Task DeleteRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
	{
		var token = await _context
			.RefreshTokens
			.FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);

		if (token != null)
		{
			_context.RefreshTokens.Remove(token);
			await _context.SaveChangesAsync(cancellationToken);
		}
	}
}