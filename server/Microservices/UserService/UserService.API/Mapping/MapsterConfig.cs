using System.Globalization;

using Mapster;

using UserService.Application.Handlers.Commands.Users.UpdateUser;
using UserService.Domain;
using UserService.Domain.Entities;
using UserService.Domain.Models;

namespace UserService.API.Mapping;

public class MapsterConfig : IRegister
{
	public void Register(TypeAdapterConfig config)
	{
		config.NewConfig<UserEntity, UserModel>()
			.MapWith(src => new UserModel(
				src.Id,
				src.Email,
				src.Password,
				src.Role,
				src.FirstName,
				src.LastName,
				src.DateOfBirth,
				src.Balance
			));

		config.NewConfig<UpdateUserCommand, UserModel>()
			.Map(dest => dest.FirstName, src => src.FirstName)
			.Map(dest => dest.LastName, src => src.LastName)
			.Map(dest => dest.DateOfBirth, src => ParseDateOrDefault(src.DateOfBirth));

		config.NewConfig<RefreshTokenModel, RefreshTokenEntity>()
			.Map(dest => dest.UserId, src => src.UserId)
			.Map(dest => dest.Token, src => src.Token)
			.Map(dest => dest.ExpiresAt, src => src.ExpiresAt)
			.Map(dest => dest.CreatedAt, src => src.CreatedAt)
			.Map(dest => dest.IsRevoked, src => src.IsRevoked)
			.Ignore(dest => dest.Id);
	}

	private static DateTime ParseDateOrDefault(string dateOfBirthString)
	{
		return DateTime.TryParseExact(
			dateOfBirthString,
			Domain.Constants.DateTimeConstants.DATE_FORMAT,
			CultureInfo.InvariantCulture,
			DateTimeStyles.None,
			out var parsedDateTime)
			? parsedDateTime
			: DateTime.MinValue;
	}
}