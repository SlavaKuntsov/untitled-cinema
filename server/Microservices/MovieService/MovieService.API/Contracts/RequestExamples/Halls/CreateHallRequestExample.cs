using MovieService.API.Contracts.Requests.Halls;
using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts;

public class CreateHallRequestExample : IExamplesProvider<CreateHallRequest>
{
	public CreateHallRequest GetExamples()
	{
		return new CreateHallRequest(
			"Больщой зал",
			220);
	}
}