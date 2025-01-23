using MovieService.Application.Handlers.Commands.Halls.CreateSimpleHall;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts.RequestExamples.Halls;

public class CreateSimpleHallRequestExample : IExamplesProvider<CreateSimpleHallCommand>
{
	public CreateSimpleHallCommand GetExamples()
	{
		return new CreateSimpleHallCommand(
			name: "Большой зал",
			totalSeats: 50,
			rows: 5,
			columns: 10);
	}
}