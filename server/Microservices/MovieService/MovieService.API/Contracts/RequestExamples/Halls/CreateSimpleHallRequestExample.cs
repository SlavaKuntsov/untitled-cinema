using MovieService.API.Contracts.Requests.Halls;
using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts.RequestExamples.Halls;

public class CreateSimpleHallRequestExample : IExamplesProvider<CreateSimpleHallRequest>
{
	public CreateSimpleHallRequest GetExamples()
	{
		return new CreateSimpleHallRequest(
			"Большой зал",
			50,
			5,
			10);
	}
}