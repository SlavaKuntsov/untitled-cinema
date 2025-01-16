using MovieService.Application.Handlers.Commands.Movies.UpdateGenre;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts.RequestExamples.Movies;

public class UpdateGenreCommandExample : IExamplesProvider<UpdateGenreCommand>
{
	public UpdateGenreCommand GetExamples()
	{
		return new UpdateGenreCommand(
			Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
			"Комфорт");
	}
}