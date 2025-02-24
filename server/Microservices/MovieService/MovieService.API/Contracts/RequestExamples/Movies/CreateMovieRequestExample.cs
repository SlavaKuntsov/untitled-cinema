using MovieService.Application.Handlers.Commands.Movies.CreateMovie;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts.RequestExamples.Movies;

public class CreateMovieRequestExample : IExamplesProvider<CreateMovieCommand>
{
	public CreateMovieCommand GetExamples()
	{
		var posterPath = Path.Combine("Contracts", "RequestExamples", "Movies", "Assets", "poster.jpg");
		var posterBytes = File.ReadAllBytes(posterPath);

		return new CreateMovieCommand(
			Title: "Сент-Экзюпери",
			Genres: ["драма", "приключения", "биография"],
			Description: "В 1930 году Антуан де Сент-Экзюпери, пилот французской авиапочты в Аргентине, вместе с лучшим другом и коллегой Анри Гийоме решает проложить более короткий маршрут через горы. Когда Гийоме пропадает в Андах, Сент-Экзюпери отправляется в рискованный ночной полет по спасению своего товарища. Это смертельно опасное путешествие не только меняет его, но и вдохновляет на создание всемирно известной книги «Маленький принц», которая станет символом безграничной силы воображения.",
			Poster: posterBytes,
			Price: 25.0m,
			DurationMinutes: 150,
			Producer: "Пабло Агеро",
			InRoles: "Луи Гаррель, Венсан Кассель",
			AgeLimit: 16,
			ReleaseDate: "06-02-2025 09:00");
	}
}