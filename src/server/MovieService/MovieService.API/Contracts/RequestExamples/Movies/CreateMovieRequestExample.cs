using MovieService.Application.Handlers.Commands.Movies.CreateMovie;
using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts.RequestExamples.Movies;

public class CreateMovieRequestExample : IExamplesProvider<CreateMovieCommand>
{
	public CreateMovieCommand GetExamples()
	{
		return new CreateMovieCommand(
			"Сент-Экзюпери",
			["драма", "приключения", "биография"],
			"В 1930 году Антуан де Сент-Экзюпери, пилот французской авиапочты в Аргентине, вместе с лучшим другом и коллегой Анри Гийоме решает проложить более короткий маршрут через горы. Когда Гийоме пропадает в Андах, Сент-Экзюпери отправляется в рискованный ночной полет по спасению своего товарища. Это смертельно опасное путешествие не только меняет его, но и вдохновляет на создание всемирно известной книги «Маленький принц», которая станет символом безграничной силы воображения.",
			25.0m,
			150,
			"Пабло Агеро",
			"Луи Гаррель, Венсан Кассель",
			16,
			"06-02-2025 09:00");
	}
}