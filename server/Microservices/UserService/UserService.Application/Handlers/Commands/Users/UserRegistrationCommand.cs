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
	Role role,
	string? firstName = null,
	string? lastName = null,
	string? dateOfBirth = null) : IRequest<AuthDto>
{
	public string Email { get; private set; } = email;
	public string Password { get; private set; } = password;
	public Role Role { get; private set; } = role;
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
				Constants.DateTimeConstants.DATE_TIME_FORMAT,
				CultureInfo.InvariantCulture,
				DateTimeStyles.None,
				out DateTime parsedDateTime))
				throw new BadRequestException("Invalid date format.");

			var existUser = await _usersRepository.Get(request.Email, cancellationToken);

			if (existUser is not null)
				throw new AlreadyExistsException($"User with email {request.Email} already exists");

			if (request.FirstName == null || request.LastName == null || request.DateOfBirth == null)
				throw new ArgumentException("First name, last name, and date of birth are required for participants.");

			UserModel userModel = request.Role == Role.User ?
				new(
					Guid.NewGuid(),
					request.Email,
					_passwordHash.Generate(request.Password),
					request.Role) :
				new(
					Guid.NewGuid(),
					request.Email,
					_passwordHash.Generate(request.Password),
					request.Role,
					request.FirstName,
					request.LastName,
					parsedDateTime);

			var accessToken = _jwt.GenerateAccessToken(userModel.Id, userModel.Role);
			var refreshToken = _jwt.GenerateRefreshToken();

			RefreshTokenModel refreshTokenModel = new(
				userModel.Id,
				userModel.Role,
				refreshToken,
				_jwt.GetRefreshTokenExpirationDays());

			await _usersRepository.Create(userModel, refreshTokenModel, cancellationToken);

			return new AuthDto
			{
				AccessToken = accessToken,
				RefreshToken = refreshToken
			};
		}
	}
}