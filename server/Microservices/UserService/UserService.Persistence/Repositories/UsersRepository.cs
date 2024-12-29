using MapsterMapper;

using Microsoft.EntityFrameworkCore;

using UserService.Domain;
using UserService.Domain.Enums;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Models;
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

	public async Task<UserModel?> GetAsync(Guid id, CancellationToken cancellationToken)
	{
		return await _context.Users
			.AsNoTracking()
			.Where(u => u.Id == id)
			.Select(user => _mapper.Map<UserModel>(user))
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<UserModel?> GetAsync(string email, CancellationToken cancellationToken)
	{
		return await _context.Users
			.AsNoTracking()
			.Where(u => u.Email == email)
			.Select(user => _mapper.Map<UserModel>(user))
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

	public async Task<IList<UserModel>> GetAsync(CancellationToken cancellationToken)
	{
		var users = await _context.Users
			.AsNoTracking()
			.ToListAsync(cancellationToken);

		return _mapper.Map<IList<UserModel>>(users) ?? [];
	}

	public async Task<Guid> CreateAsync(UserModel user, RefreshTokenModel refreshTokenModel, CancellationToken cancellationToken)
	{
		var userEntity = _mapper.Map<UserEntity>(user);
		var refreshTokenEntity = _mapper.Map<RefreshTokenEntity>(refreshTokenModel);

		using var transaction = _context.Database.BeginTransaction();
		try
		{
			await _context.Users.AddAsync(userEntity, cancellationToken);
			await _context.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);
			await _context.SaveChangesAsync(cancellationToken);

			transaction.Commit();

			return userEntity.Id;
		}
		catch (Exception ex)
		{
			await transaction.RollbackAsync(cancellationToken);
			throw new InvalidOperationException($"An error occurred while creating user and saving token: {ex.Message}", ex);
		}
	}

	public async Task<UserModel> UpdateAsync(UserModel model, CancellationToken cancellationToken)
	{
		var entity = await _context.Users.FindAsync(model.Id, cancellationToken)
		?? throw new InvalidOperationException("The entity was not found.");

		if (entity.Role is not Role.Admin)
		{
			_mapper.Map(model, entity); 
		}
		
		await _context.SaveChangesAsync(cancellationToken);

		return _mapper.Map<UserModel>(entity);
	}

	public async Task DeleteAsync(UserModel model, CancellationToken cancellationToken)
	{
		var entity = _mapper.Map<UserEntity>(model);
		
		var token = await _context.RefreshTokens
			.FirstOrDefaultAsync(rt => rt.UserId == model.Id, cancellationToken);

		if (entity == null)
		{
			throw new NotFoundException("User not found.");
		}

		using var transaction = _context.Database.BeginTransaction();
		try
		{
			if (entity.Role == Role.User)
			{
				_context.Users.Remove(entity);
				_context.RefreshTokens.Remove(token);

				await _context.SaveChangesAsync(cancellationToken);
			}
			else if (entity.Role == Role.Admin)
			{
				var adminCount = await _context.Users
					.Where(u => u.Role == Role.Admin)
					.CountAsync(cancellationToken);

				if (adminCount == 1)
				{
					throw new BadRequestException("Cannot delete the last Admin");
				}

				_context.Users.Remove(entity);
				_context.RefreshTokens.Remove(token);

				await _context.SaveChangesAsync(cancellationToken);
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
