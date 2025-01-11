using MovieService.API.Contracts.Requests.Halls;
using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts.Examples.Movies;

public class UpdateHallRequestExample : IExamplesProvider<UpdateHallRequest>
{
	public UpdateHallRequest GetExamples()
	{
		return new UpdateHallRequest(
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