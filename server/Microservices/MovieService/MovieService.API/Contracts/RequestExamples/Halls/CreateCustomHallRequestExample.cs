using MovieService.API.Contracts.Requests.Halls;
using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts;

public class CreateCustomHallRequestExample : IExamplesProvider<CreateCustomHallRequest>
{
	public CreateCustomHallRequest GetExamples()
	{
		return new CreateCustomHallRequest(
			"Большой зал",
			10,
			[
				[-1, 0, 0, -1],
				[0, 0, 0, 0],
				[0, 0, 0, 0]
			]);
	}
}