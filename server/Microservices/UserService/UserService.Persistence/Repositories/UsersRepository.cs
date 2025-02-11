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

	public async Task<IList<Guid>> GetByRole(Role role, CancellationToken cancellationToken)
	{
		var users = await _context.Users
			.AsNoTracking()
			.Where(u => u.Role == role)
			.Select(u => u.Id)
			.ToListAsync(cancellationToken);

		return users ?? [];
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

	public void Update(UserEntity entity)
	{
		_context.Users.Attach(entity).State = EntityState.Modified;
	}

	public void Delete(UserEntity userEntity, RefreshTokenEntity refreshTolkenEntity)
	{
		_context.Users.Attach(userEntity);
		_context.RefreshTokens.Attach(refreshTolkenEntity);

		_context.Users.Remove(userEntity);
		_context.RefreshTokens.Remove(refreshTolkenEntity);

		return;
	}
}