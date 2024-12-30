using Microsoft.EntityFrameworkCore;

using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Persistence.Repositories;
public class UsersRepository : IUsersRepository
{
	private readonly UserServiceDBContext _context;

	public UsersRepository(UserServiceDBContext context)
	{
		_context = context;
	}

	public async Task<UserEntity?> GetAsync(Guid id, CancellationToken cancellationToken)
	{
		return await _context.Users
			.AsNoTracking()
			.Where(u => u.Id == id)
			.Select(user => user)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<UserEntity?> GetAsync(string email, CancellationToken cancellationToken)
	{
		return await _context.Users
			.AsNoTracking()
			.Where(u => u.Email == email)
			.Select(user => user)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<Role?> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken)
	{
		return await _context.Users
			.AsNoTracking()
			.Where(u => u.Id == id)
			.Select(u => u.Role)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<IList<UserEntity>> GetAsync(CancellationToken cancellationToken)
	{
		var users = await _context.Users
			.AsNoTracking()
			.ToListAsync(cancellationToken);

		return users ?? [];
	}

	public async Task<Guid> CreateAsync(UserEntity user, RefreshTokenEntity refreshToken, CancellationToken cancellationToken)
	{
		await _context.Users.AddAsync(user, cancellationToken);
		await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);

		return user.Id;
	}

	public async Task<UserEntity> UpdateAsync(UserEntity entity, CancellationToken cancellationToken)
	{
		var existEntity = await _context.Users.FirstAsync(u => u.Id == entity.Id, cancellationToken)
				?? throw new NotFoundException($"User with id {entity.Id} doesn't exists");

		existEntity.FirstName = entity.FirstName;
		existEntity.LastName = entity.LastName;
		existEntity.DateOfBirth = entity.DateOfBirth;

		return existEntity!;
	}

	public async Task DeleteAsync(UserEntity entity, CancellationToken cancellationToken)
	{
		var token = await _context.RefreshTokens
			.FirstOrDefaultAsync(rt => rt.UserId == entity.Id, cancellationToken)
			?? throw new NotFoundException($"Refresh Token for user with id {entity.Id} not found.");

		using var transaction = _context.Database.BeginTransaction();
		try
		{
			if (entity.Role == Role.User)
			{
				_context.Users.Remove(entity);
				_context.RefreshTokens.Remove(token);
			}
			else if (entity.Role == Role.Admin)
			{
				var adminCount = await _context.Users
					.Where(u => u.Role == Role.Admin)
					.CountAsync(cancellationToken);

				if (adminCount == 1)
					throw new BadRequestException("Cannot delete the last Admin");

				_context.Users.Remove(entity);
				_context.RefreshTokens.Remove(token);
			}
			transaction.Commit();
			return;
		}
		catch (Exception ex)
		{
			await transaction.RollbackAsync(cancellationToken);
			throw new InvalidOperationException($"An error occurred while creating user and saving token: {ex.Message}", ex);
		}
	}
}