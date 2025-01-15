using MovieService.Application.Handlers.Commands.Seats.CreateSeat;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts.RequestExamples.Seats;

public class CreateSeatCommandExample : IExamplesProvider<CreateSeatCommand>
{
	public CreateSeatCommand GetExamples()
	{
		return new CreateSeatCommand(
			"Большой зал",
			"Комфорт",
			2,
			2);
	}
}