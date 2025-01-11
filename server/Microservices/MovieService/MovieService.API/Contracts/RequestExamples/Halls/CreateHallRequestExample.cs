using MovieService.API.Contracts.Requests.Halls;
using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts;

public class CreateHallRequestExample : IExamplesProvider<CreateHallRequest>
{
	public CreateHallRequest GetExamples()
	{
		return new CreateHallRequest(
			"Большой зал",
			220,
			[
				[1, 0, 1],
				[1, 0, 0],
				[-1, 1, -1]
			]);
	}
}