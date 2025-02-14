using MovieService.Application.Handlers.Commands.Sessions.FillSession;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts.RequestExamples.Sessions;

public class FillSessionRequestExample : IExamplesProvider<FillSessionCommand>
{
	public FillSessionCommand GetExamples()
	{
		return new FillSessionCommand(
			movieId: Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
			hallId: Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
			priceModifier: 1.0m,
			startTime: "05-01-2025 09:00");
	}
}