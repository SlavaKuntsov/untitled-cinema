using MovieService.Application.Handlers.Commands.Days.CreateSession;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts.RequestExamples.Days;

public class CreateDayRequestExample : IExamplesProvider<CreateDayCommand>
{
	public CreateDayCommand GetExamples()
	{
		return new CreateDayCommand(
			startTime: "05-01-2025 09:00",
			endTime: "05-01-2025 21:00");
	}
}