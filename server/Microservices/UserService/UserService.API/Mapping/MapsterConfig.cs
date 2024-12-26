using System.Globalization;

using Mapster;

using UserService.Application.Handlers.Commands.Users;
using UserService.Domain.Models.Users;
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

		//config.NewConfig<UserEntity, UserModel>()
		//	.Map(dest => dest.Id, src => src.Id)
		//	.Map(dest => dest.Email, src => src.Email)
		//	.Map(dest => dest.Password, src => src.Password)
		//	.Map(dest => dest.Role, src => src.Role)
		//	.Map(dest => dest.FirstName, src => src.FirstName)
		//	.Map(dest => dest.LastName, src => src.LastName)
		//	.Map(dest => dest.DateOfBirth, src => src.DateOfBirth);

		// Маппинг UpdateUserCommand -> UserModel
		config.NewConfig<UpdateUserCommand, UserModel>()
			.Map(dest => dest.FirstName, src => src.FirstName)
			.Map(dest => dest.LastName, src => src.LastName)
			.Map(dest => dest.DateOfBirth, src => ParseDateOrDefault(src.DateOfBirthString));
	}

	// Метод для безопасного парсинга даты
	private static DateTime ParseDateOrDefault(string dateOfBirthString)
	{
		return DateTime.TryParseExact(
			dateOfBirthString,
			Domain.Constants.DateTimeConstants.DATE_TIME_FORMAT,
			CultureInfo.InvariantCulture,
			DateTimeStyles.None,
			out var parsedDateTime)
			? parsedDateTime
			: DateTime.MinValue; // Вернём минимальную дату как значение по умолчанию
	}
}
