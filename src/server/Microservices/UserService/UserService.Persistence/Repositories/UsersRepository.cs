using Domain.Enums;
using Microsoft.EntityFrameworkCore;

using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Persistence.Repositories;

public class UsersRepository : IUsersRepository
{
	private readonly UserServiceDBContext _context;

	private static readonly Func<UserServiceDBContext, Guid, CancellationToken, Task<UserEntity?>> s_compiledQuery =
		EF.CompileAsyncQuery((UserServiceDBContext context, Guid id, CancellationToken cancellationToken) =>
			context.Users
				.AsNoTracking()
				.Where(u => u.Id == id)
				.FirstOrDefault());

	private static readonly Func<UserServiceDBContext, Guid, CancellationToken, Task<UserWithStringDateOfBirthEntity?>> s_compiledQuery2 =
	EF.CompileAsyncQuery((UserServiceDBContext context, Guid id, CancellationToken cancellationToken) =>
		context.Users
			.AsNoTracking()
			.Where(u => u.Id == id)
			.Select(u => new UserWithStringDateOfBirthEntity
			{
				Id = u.Id,
				Email = u.Email,
				Password = u.Password,
				Role = u.Role,
				FirstName = u.FirstName,
				LastName = u.LastName,
				DateOfBirth = u.DateOfBirth.ToString("yyyy-MM-dd"), // Преобразование в строку
                Balance = u.Balance
			})
			.FirstOrDefault());

	public UsersRepository(UserServiceDBContext context)
	{
		_context = context;
	}

	public async Task<UserEntity?> GetAsync(Guid id, CancellationToken cancellationToken)
	{
		var a = await s_compiledQuery(_context, id, cancellationToken);

		return a;
	}

	public async Task<UserWithStringDateOfBirthEntity?> GetWithStringDateOfBirthAsync(Guid id, CancellationToken cancellationToken)
	{
		return await s_compiledQuery2(_context, id, cancellationToken);
	}

	public async Task<(Guid?, Role?, Guid?)> GetIdWithRoleAndTokenAsync(Guid userId, CancellationToken cancellationToken)
	{
		var result = await _context.Users
			.AsNoTracking()
			.Where(u => u.Id == userId)
			.Select(u => new { u.Id, u.Role ,RefreshTokenId = (Guid?)u.RefreshToken.Id })
			.FirstOrDefaultAsync(cancellationToken);

		return (result?.Id, result?.Role, result?.RefreshTokenId);
	}

	public async Task<(Guid?, string?, Role?)> GetIdWithRoleAndPasswordAsync(string email, CancellationToken cancellationToken)
	{
		var result = await _context.Users
			.AsNoTracking()
			.Where(u => u.Email == email)
			.Select(u => new { u.Id, u.Password, u.Role})
			.FirstOrDefaultAsync(cancellationToken);

		if (result == null)
			return (null, null, null);

		return (result?.Id, result?.Password, result?.Role);
	}

	public async Task<Guid?> GetIdAsync(string email, CancellationToken cancellationToken)
	{
		return await _context.Users
			.AsNoTracking()
			.Where(u => u.Email == email)
			.Select(u => u.Id)
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
		return await _context.Users
			.AsNoTracking()
			.Where(u => u.Role == role)
			.Select(u => u.Id)
			.ToListAsync(cancellationToken);
	}

	public async Task<IList<UserEntity>> GetAsync(CancellationToken cancellationToken)
	{
		return await _context.Users
			.AsNoTracking()
			.ToListAsync(cancellationToken);
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

	public void Delete(Guid userId, Guid refreshTokenId)
	{
		var userEntity = new UserEntity { Id = userId };
		var refreshTokenEntity = new RefreshTokenEntity { Id = refreshTokenId };

		_context.Users.Attach(userEntity);
		_context.RefreshTokens.Attach(refreshTokenEntity);

		_context.Users.Remove(userEntity);
		_context.RefreshTokens.Remove(refreshTokenEntity);

		_context.SaveChanges();
	}
}