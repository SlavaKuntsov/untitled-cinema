using Domain.Enums;
using Domain.Exceptions;
using Extensions.Strings;
using MapsterMapper;
using MediatR;
using UserService.Application.Data;
using UserService.Application.DTOs;
using UserService.Application.Interfaces.Auth;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Models;

namespace UserService.Application.Handlers.Commands.Users.UserRegistration;

public class UserRegistrationCommandHandler(
	IUsersRepository usersRepository,
	IPasswordHash passwordHash,
	IJwt jwt,
	IDBContext dbContext,
	IMapper mapper) : IRequestHandler<UserRegistrationCommand, AuthDto>
{
	public async Task<AuthDto> Handle(UserRegistrationCommand request, CancellationToken cancellationToken)
	{
		if (!request.DateOfBirth.DateFormatTryParse(out var parsedDateTime))
			throw new BadRequestException("Invalid date format.");

		var id = await usersRepository.GetIdAsync(request.Email, cancellationToken);

		if (id!.Value != Guid.Empty)
			throw new AlreadyExistsException($"User with email {request.Email} already exists");

		var userModel = new UserModel(
			Guid.NewGuid(),
			request.Email,
			passwordHash.Generate(request.Password),
			Role.User,
			request.FirstName,
			request.LastName,
			parsedDateTime);

		const Role role = Role.User;

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