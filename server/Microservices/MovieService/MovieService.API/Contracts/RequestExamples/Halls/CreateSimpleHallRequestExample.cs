using MovieService.Application.Handlers.Commands.Halls.CreateSimpleHall;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts.RequestExamples.Halls;

public class CreateSimpleHallRequestExample : IExamplesProvider<CreateSimpleHallCommand>
{
	public CreateSimpleHallCommand GetExamples()
	{
		return new CreateSimpleHallCommand(
			"Большой зал",
			50,
			5,
			10);
	}
}