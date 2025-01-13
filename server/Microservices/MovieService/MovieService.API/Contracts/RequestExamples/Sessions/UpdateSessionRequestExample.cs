using MovieService.Application.Handlers.Commands.Sessions.UpdateSession;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts.RequestExamples.Sessions;

public class UpdateSessionRequestExample : IExamplesProvider<UpdateSessionCommand>
{
	public UpdateSessionCommand GetExamples()
	{
		return new UpdateSessionCommand(
			Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
			Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
			Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
			"05-01-2025 10:00");
	}
}