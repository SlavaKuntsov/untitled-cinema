using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts.Examples;

public class CreateMovieRequestExample : IExamplesProvider<CreateMovieRequest>
{
	public CreateMovieRequest GetExamples()
	{
		return new CreateMovieRequest(
			"Гладиатор 2",
			["боевик", "драма"],
			"После того как его дом завоевывают тиранические императоры, возглавляющие Рим, Луций, сын Луциллы и Максимуса, вынужден выйти на арену Колизея и обратиться к своему прошлому, чтобы найти в себе силы вернуть славу Рима его народу.",
			165,
			"Ридли Скотт",
			"23-12-2024 15:00:00");
	}
}