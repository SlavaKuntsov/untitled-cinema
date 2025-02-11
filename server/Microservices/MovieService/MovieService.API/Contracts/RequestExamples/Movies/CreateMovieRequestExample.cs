using MovieService.Application.Handlers.Commands.Movies.CreateMovie;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts.Examples.Movies;

public class CreateMovieRequestExample : IExamplesProvider<CreateMovieCommand>
{
	public CreateMovieCommand GetExamples()
	{
		return new CreateMovieCommand(
			title: "Гладиатор",
			genres: ["боевик", "драма"],
			description: "После того как его дом завоевывают тиранические императоры, возглавляющие Рим, Луций, сын Луциллы и Максимуса, вынужден выйти на арену Колизея и обратиться к своему прошлому, чтобы найти в себе силы вернуть славу Рима его народу.",
			price: 20.0m,
			durationMinutes: 165,
			producer: "Ридли Скотт",
			ageLimit: 18,
			releaseDate: "23-12-2024 15:00");
	}
}