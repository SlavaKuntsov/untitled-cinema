using MovieService.Application.Handlers.Commands.Sessions.UpdateSession;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts.RequestExamples.Sessions;

public class UpdateSessionRequestExample : IExamplesProvider<UpdateSessionCommand>
{
	public UpdateSessionCommand GetExamples()
	{
		return new UpdateSessionCommand(
			id: Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
			movieId: Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
			hallId: Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
			startTime: "05-01-2025 10:00");
	}
}