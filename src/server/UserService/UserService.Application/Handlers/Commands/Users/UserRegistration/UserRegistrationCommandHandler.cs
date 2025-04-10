using Domain.Enums;
using Domain.Exceptions;
using MapsterMapper;
using MediatR;
using UserService.Application.Data;
using UserService.Application.DTOs;
using UserService.Application.Interfaces.Auth;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Models;

namespace UserService.Application.Handlers.Commands.Users.UserRegistration;

public record UserRegistrationCommand(
	string Email,
	string? Password,
	string FirstName,
	string LastName,
	string? DateOfBirth) : IRequest<AuthDto>;

public class UserRegistrationCommandHandler(
	IUsersRepository usersRepository,
	IJwt jwt,
	IPasswordHash passwordHash,
	IDBContext dbContext,
	IMapper mapper) : IRequestHandler<UserRegistrationCommand, AuthDto>
{
	public async Task<AuthDto> Handle(UserRegistrationCommand request, CancellationToken cancellationToken)
	{
		var id = await usersRepository.GetIdAsync(request.Email, cancellationToken);

		if (id!.Value != Guid.Empty)
			throw new AlreadyExistsException($"User with email {request.Email} already exists");

		const Role role = Role.User;

		var userModel = new UserModel(
			Guid.NewGuid(),
			request.Email,
			request.Password != string.Empty ? passwordHash.Generate(request.Password) : "",
			role,
			request.FirstName,
			request.LastName,
			request.DateOfBirth
		);

		var accessToken = jwt.GenerateAccessToken(userModel.Id, role);
		var refreshToken = jwt.GenerateRefreshToken();

		var refreshTokenModel = new RefreshTokenModel(
			userModel.Id,
			refreshToken,
			jwt.GetRefreshTokenExpirationDays());

		await usersRepository.CreateAsync(
			mapper.Map<UserEntity>(userModel),
			mapper.Map<RefreshTokenEntity>(refreshTokenModel),
			cancellationToken);

		await dbContext.SaveChangesAsync(cancellationToken);

		return new AuthDto(accessToken, refreshToken);
	}
}