using MovieService.API.Contracts.Requests.Days;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts.RequestExamples.Days;

public class CreateDayRequestExample : IExamplesProvider<CreateDayRequest>
{
	public CreateDayRequest GetExamples()
	{
		return new CreateDayRequest(
			"05-01-2025 09:00",
			"05-01-2025 21:00");
	}
}