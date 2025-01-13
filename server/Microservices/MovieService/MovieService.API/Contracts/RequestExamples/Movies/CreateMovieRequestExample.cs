using MovieService.Application.Handlers.Commands.Movies.CreateMovie;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts.Examples.Movies;

public class CreateMovieRequestExample : IExamplesProvider<CreateMovieCommand>
{
	public CreateMovieCommand GetExamples()
	{
		return new CreateMovieCommand(
			"Гладиатор",
			["боевик", "драма"],
			"После того как его дом завоевывают тиранические императоры, возглавляющие Рим, Луций, сын Луциллы и Максимуса, вынужден выйти на арену Колизея и обратиться к своему прошлому, чтобы найти в себе силы вернуть славу Рима его народу.",
			165,
			"Ридли Скотт",
			"23-12-2024 15:00");
	}
}