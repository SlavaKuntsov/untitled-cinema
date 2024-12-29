using System.Globalization;

using Mapster;

using UserService.Application.Handlers.Commands.Users;
using UserService.Domain;
using UserService.Persistence.Entities;

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
				src.DateOfBirth
			));

		config.NewConfig<UpdateUserCommand, UserModel>()
			.Map(dest => dest.FirstName, src => src.FirstName)
			.Map(dest => dest.LastName, src => src.LastName)
			.Map(dest => dest.DateOfBirth, src => ParseDateOrDefault(src.DateOfBirth));
	}

	private static DateTime ParseDateOrDefault(string dateOfBirthString)
	{
		return DateTime.TryParseExact(
			dateOfBirthString,
			Domain.Constants.DateTimeConstants.DATE_TIME_FORMAT,
			CultureInfo.InvariantCulture,
			DateTimeStyles.None,
			out var parsedDateTime)
			? parsedDateTime
			: DateTime.MinValue;
	}
}