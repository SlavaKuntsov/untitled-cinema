using MapsterMapper;

using MediatR;

using UserService.Application.DTOs;
using UserService.Application.Extensions;
using UserService.Application.Interfaces.Auth;
using UserService.Domain;
using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Models;

namespace UserService.Application.Handlers.Commands.Users.UserRegistration;

public class UserRegistrationCommandHandler(
		IUsersRepository usersRepository,
		IPasswordHash passwordHash,
		IJwt jwt,
		IMapper mapper)
		: IRequestHandler<UserRegistrationCommand, AuthDto>
{
	private readonly IUsersRepository _usersRepository = usersRepository;
	private readonly IPasswordHash _passwordHash = passwordHash;
	private readonly IJwt _jwt = jwt;
	private readonly IMapper _mapper = mapper;

	public async Task<AuthDto> Handle(UserRegistrationCommand request, CancellationToken cancellationToken)
	{
		if (!request.DateOfBirth.DateFormatTryParse(out DateTime parsedDateTime))
			throw new BadRequestException("Invalid date format.");

		var existUser = await _usersRepository.GetAsync(request.Email, cancellationToken);

		if (existUser is not null)
			throw new AlreadyExistsException($"User with email {request.Email} already exists");

		var userModel = new UserModel(
					Guid.NewGuid(),
					request.Email,
					_passwordHash.Generate(request.Password),
					Role.User,
					request.FirstName,
					request.LastName,
					parsedDateTime);

		var role = Role.User;

		var accessToken = _jwt.GenerateAccessToken(userModel.Id, role);
		var refreshToken = _jwt.GenerateRefreshToken();

		var refreshTokenModel = new RefreshTokenModel(
				userModel.Id,
				refreshToken,
				_jwt.GetRefreshTokenExpirationDays());

		await _usersRepository.CreateAsync(
			_mapper.Map<UserEntity>(userModel),
			_mapper.Map<RefreshTokenEntity>(refreshTokenModel),
			cancellationToken);

		return new AuthDto(accessToken, refreshToken);
	}
}