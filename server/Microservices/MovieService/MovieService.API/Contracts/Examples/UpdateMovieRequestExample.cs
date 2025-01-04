using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts.Examples;

public class UpdateMovieRequestExample : IExamplesProvider<UpdateMovieRequest>
{
	public UpdateMovieRequest GetExamples()
	{
		return new UpdateMovieRequest(
			Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
			"Гладиатор 2",
			["боевик", "драма"],
			"После того как его дом завоевывают тиранические императоры, возглавляющие Рим, Луций, сын Луциллы и Максимуса, вынужден выйти на арену Колизея и обратиться к своему прошлому, чтобы найти в себе силы вернуть славу Рима его народу.",
			166,
			"23-12-2024 15:00:00");
	}
}