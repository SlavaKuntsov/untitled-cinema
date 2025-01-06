using MovieService.API.Contracts.Requests.Sessions;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts.RequestExamples.Sessions;

public class FillSessionRequestExample : IExamplesProvider<FillSessionRequest>
{
	public FillSessionRequest GetExamples()
	{
		return new FillSessionRequest(
			Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
			Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
			"06-01-2025 09:00");
	}
}
