using MovieService.API.Contracts.Requests.Movies;
using MovieService.API.Contracts.Requests.Sessions;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts.RequestExamples.Sessions;

public class UpdateSessionRequestExample : IExamplesProvider<UpdateSessionRequest>
{
	public UpdateSessionRequest GetExamples()
	{
		return new UpdateSessionRequest(
			Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
			Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
			Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
			"05-01-2025 10:00");
	}
}