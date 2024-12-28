using System.Globalization;

using MediatR;

using UserService.Application.DTOs;
using UserService.Application.Interfaces.Auth;
using UserService.Domain.Enums;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Models.Auth;
using UserService.Domain.Models.Users;

namespace UserService.Application.Handlers.Commands.Users;

public class UserRegistrationCommand(
	string email,
	string password,
	string? firstName = null,
	string? lastName = null,
	string? dateOfBirth = null) : IRequest<AuthDto>
{
	public string Email { get; private set; } = email;
	public string Password { get; private set; } = password;
	public string? FirstName { get; private set; } = firstName;
	public string? LastName { get; private set; } = lastName;
	public string? DateOfBirth { get; private set; } = dateOfBirth;

	public class UserRegistrationCommandHandler(
		IUsersRepository usersRepository,
		IPasswordHash passwordHash,
		IJwt jwt)
		: IRequestHandler<UserRegistrationCommand, AuthDto>
	{
		private readonly IUsersRepository _usersRepository = usersRepository;
		private readonly IPasswordHash _passwordHash = passwordHash;
		private readonly IJwt _jwt = jwt;

		public async Task<AuthDto> Handle(UserRegistrationCommand request, CancellationToken cancellationToken)
		{
			if (!DateTime.TryParseExact(
				request.DateOfBirth,
				Domain.Constants.DateTimeConstants.DATE_TIME_FORMAT,
				CultureInfo.InvariantCulture,
				DateTimeStyles.None,
				out DateTime parsedDateTime))
				throw new BadRequestException("Invalid date format.");

			var existUser = await _usersRepository.Get(request.Email, cancellationToken);

			if (existUser is not null)
				throw new AlreadyExistsException($"User with email {request.Email} already exists");

			UserModel userModel = new(
					Guid.NewGuid(),
					request.Email,
					_passwordHash.Generate(request.Password),
					Role.User,
					request.FirstName,
					request.LastName,
					parsedDateTime);

			Role role = Role.User;

			var accessToken = _jwt.GenerateAccessToken(userModel.Id, role);
			var refreshToken = _jwt.GenerateRefreshToken();

			RefreshTokenModel refreshTokenModel = new(
				userModel.Id,
				role,
				refreshToken,
				_jwt.GetRefreshTokenExpirationDays());

			await _usersRepository.Create(userModel, refreshTokenModel, cancellationToken);

			return new AuthDto(accessToken, refreshToken);
		}
	}
}