using MapsterMapper;

using Microsoft.EntityFrameworkCore;

using UserService.Domain.Enums;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Models.Auth;
using UserService.Domain.Models.Users;
using UserService.Persistence.Entities;

namespace UserService.Persistence.Repositories;
public class UsersRepository : IUsersRepository
{
	private readonly UserServiceDBContext _context;
	private readonly IMapper _mapper;

	public UsersRepository(UserServiceDBContext context, IMapper mapper)
	{
		_context = context;
		_mapper = mapper;
	}

	public async Task<UserModel?> Get(Guid id, CancellationToken cancellationToken)
	{
		return await _context.Users
			.AsNoTracking()
			.Where(u => u.Id == id)
			.Select(user => _mapper.Map<UserModel>(user))
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<UserModel?> Get(string email, CancellationToken cancellationToken)
	{
		return await _context.Users
			.AsNoTracking()
			.Where(u => u.Email == email)
			.Select(user => _mapper.Map<UserModel>(user))
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<IList<UserModel>> Get(CancellationToken cancellationToken)
	{
		var users = await _context.Users
			.AsNoTracking()
			.ToListAsync(cancellationToken);

		return _mapper.Map<IList<UserModel>>(users) ?? [];
	}

	public async Task<Guid> Create(UserModel user, RefreshTokenModel refreshTokenModel, CancellationToken cancellationToken)
	{
		var refreshTokenEntity = _mapper.Map<RefreshTokenEntity>(refreshTokenModel);

		using var transaction = _context.Database.BeginTransaction();

		try
		{
			var userEntity = _mapper.Map<UserEntity>(user);

			await _context.Users.AddAsync(userEntity, cancellationToken);
			await _context.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);
			await _context.SaveChangesAsync(cancellationToken);

			transaction.Commit();

			return userEntity.Id;

			//if (user.Role is Role.User)
			//{
			//	await _context.Users.AddAsync(userEntity, cancellationToken);
			//	await _context.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);
			//	await _context.SaveChangesAsync(cancellationToken);

			//	transaction.Commit();

			//	return userEntity.Id;
			//}
			//else if (user.Role is Role.Admin)
			//{
			//	//var adminCount = await _context.Users
			//	//	.Where(u => u.Role == Role.Admin)
			//	//	.CountAsync(cancellationToken);

			//	//userEntity.IsActiveAdmin = adminCount == 0; // true, если это первая запись

			//	await _context.Users.AddAsync(userEntity, cancellationToken);
			//	await _context.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);
			//	await _context.SaveChangesAsync(cancellationToken);

			//	transaction.Commit();

			//	return userEntity.Id;
			//}
		}
		catch (Exception ex)
		{
			await transaction.RollbackAsync(cancellationToken);
			throw new InvalidOperationException($"An error occurred while creating user and saving token: {ex.Message}", ex);
		}
	}

	public async Task<UserModel> Update(UserModel model, CancellationToken cancellationToken)
	{
		var entity = await _context.Users.FindAsync(model.Id, cancellationToken)
			?? throw new InvalidOperationException("The entity was not found.");

		if (entity.Role is not Role.Admin)
		{
			entity = _mapper.Map<UserEntity>(model);
		}

		await _context.SaveChangesAsync(cancellationToken);

		return _mapper.Map<UserModel>(entity);
	}

	public async Task Delete(Guid userId, CancellationToken cancellationToken)
	{
		var entity = await _context.Users.FindAsync(userId, cancellationToken, cancellationToken)
			?? throw new InvalidOperationException("The entity was not found.");

		if (entity.Role is not Role.Admin)
		{
			_context.Users.Remove(entity!);
			await _context.SaveChangesAsync(cancellationToken);
		}
		else if (entity.Role is Role.Admin)
		{
			var adminCount = await _context.Users
				.Where(u => u.Role == Role.Admin)
				.CountAsync(cancellationToken);

			if (adminCount == 1)
				throw new BadRequestException("Сannot delete the last Admin");

			_context.Users.Remove(entity);
			await _context.SaveChangesAsync(cancellationToken);
		}
	}
}
