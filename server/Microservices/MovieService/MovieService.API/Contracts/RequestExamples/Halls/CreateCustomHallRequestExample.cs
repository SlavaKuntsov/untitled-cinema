using MovieService.Application.Handlers.Commands.Halls.CreateHall;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts;

public class CreateCustomHallRequestExample : IExamplesProvider<CreateCustomHallCommand>
{
	public CreateCustomHallCommand GetExamples()
	{
		return new CreateCustomHallCommand(
			name: "Большой зал",
			totalSeats: 14,
			seats: [
				[-1, 1, 1, -1],
				[1, 1, 1, 1],
				[1, 1, 1, 1],
				[1, 1, 1, 1]
			]);
	}
}