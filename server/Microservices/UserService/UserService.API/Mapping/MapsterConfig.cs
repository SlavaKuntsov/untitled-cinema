using System.Globalization;

using Mapster;

using UserService.Application.Handlers.Commands.Users.UpdateUser;
using UserService.Domain;
using UserService.Domain.Entities;

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