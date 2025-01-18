using MovieService.Application.Handlers.Commands.Seats.CreateSeat;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts.RequestExamples.Seats;

public class CreateSeatCommandExample : IExamplesProvider<CreateSeatCommand>
{
	public CreateSeatCommand GetExamples()
	{
		return new CreateSeatCommand(
			hall: "Большой зал",
			seatType: "Комфорт",
			row: 2,
			column: 2);
	}
}