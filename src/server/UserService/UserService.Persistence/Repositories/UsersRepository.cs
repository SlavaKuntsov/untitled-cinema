using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Persistence.Repositories;

public class UsersRepository(UserServiceDBContext context) : IUsersRepository
{
	private static readonly Func<UserServiceDBContext, Guid, CancellationToken, Task<UserEntity?>> s_compiledQuery =
		EF.CompileAsyncQuery(
			(UserServiceDBContext context, Guid id, CancellationToken cancellationToken) =>
				context.Users
					.AsNoTracking()
					.FirstOrDefault(u => u.Id == id));

	public async Task<UserEntity?> GetAsync(Guid id, CancellationToken cancellationToken)
	{
		var a = await s_compiledQuery(context, id, cancellationToken);

		return a;
	}
	public async Task<UserEntity?> GetAsync(string email, CancellationToken cancellationToken)
	{
		return await context.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
	}

	public async Task<(Guid?, Role?, Guid?)> GetIdWithRoleAndTokenAsync(
		Guid userId,
		CancellationToken cancellationToken)
	{
		var result = await context.Users
			.AsNoTracking()
			.Where(u => u.Id == userId)
			.Select(u => new { u.Id, u.Role, RefreshTokenId = (Guid?)u.RefreshToken.Id })
			.FirstOrDefaultAsync(cancellationToken);

		return (result?.Id, result?.Role, result?.RefreshTokenId);
	}

	public async Task<(Guid?, string?, Role?)> GetIdWithRoleAndPasswordAsync(
		string email,
		CancellationToken cancellationToken)
	{
		var result = await context.Users
			.AsNoTracking()
			.Where(u => u.Email == email)
			.Select(u => new { u.Id, u.Password, u.Role })
			.FirstOrDefaultAsync(cancellationToken);

		if (result == null)
			return (null, null, null);

		return (result?.Id, result?.Password, result?.Role);
	}

	public async Task<Guid?> GetIdAsync(string email, CancellationToken cancellationToken)
	{
		return await context.Users
			.AsNoTracking()
			.Where(u => u.Email == email)
			.Select(u => u.Id)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<Role?> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken)
	{
		return await context.Users
			.AsNoTracking()
			.Where(u => u.Id == id)
			.Select(u => u.Role)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<IList<Guid>> GetByRole(Role role, CancellationToken cancellationToken)
	{
		return await context.Users
			.AsNoTracking()
			.Where(u => u.Role == role)
			.Select(u => u.Id)
			.ToListAsync(cancellationToken);
	}

	public async Task<IList<UserEntity>> GetAsync(CancellationToken cancellationToken)
	{
		return await context.Users
			.AsNoTracking()
			.ToListAsync(cancellationToken);
	}

	public async Task<Guid> CreateAsync(
		UserEntity user,
		RefreshTokenEntity refreshToken,
		CancellationToken cancellationToken)
	{
		await context.Users.AddAsync(user, cancellationToken);
		await context.RefreshTokens.AddAsync(refreshToken, cancellationToken);

		return user.Id;
	}

	public void Update(UserEntity entity)
	{
		context.Users.Attach(entity).State = EntityState.Modified;
	}

	public void Delete(Guid userId, Guid refreshTokenId)
	{
		var userEntity = new UserEntity { Id = userId };
		var refreshTokenEntity = new RefreshTokenEntity { Id = refreshTokenId };

		context.Users.Attach(userEntity);
		context.RefreshTokens.Attach(refreshTokenEntity);

		context.Users.Remove(userEntity);
		context.RefreshTokens.Remove(refreshTokenEntity);

		context.SaveChanges();
	}
}