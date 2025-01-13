using MovieService.Application.Handlers.Commands.Halls.UpdateHall;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts.Examples.Movies;

public class UpdateHallRequestExample : IExamplesProvider<UpdateHallCommand>
{
	public UpdateHallCommand GetExamples()
	{
		return new UpdateHallCommand(
			Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
			"Большой зал 2",
			230,
			[
				[1, 1, 1],
				[1, 0, 0],
				[-1, 1, -1]
			]);
	}
}