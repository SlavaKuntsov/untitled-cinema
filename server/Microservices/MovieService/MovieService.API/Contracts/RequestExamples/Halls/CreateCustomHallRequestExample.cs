using MovieService.Application.Handlers.Commands.Halls.CreateHall;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts;

public class CreateCustomHallRequestExample : IExamplesProvider<CreateCustomHallCommand>
{
	public CreateCustomHallCommand GetExamples()
	{
		return new CreateCustomHallCommand(
			"Большой зал",
			10,
			[
				[-1, 0, 0, -1],
				[0, 0, 0, 0],
				[0, 0, 0, 0]
			]);
	}
}