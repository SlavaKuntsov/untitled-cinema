using MovieService.Application.Handlers.Commands.Seats.CreateSeat;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts.RequestExamples.Seats;

public class CreateSeatCommandExample : IExamplesProvider<CreateSeatCommand>
{
	public CreateSeatCommand GetExamples()
	{
		return new CreateSeatCommand(
			hallId: Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
			seatType: "Комфорт",
			row: 2,
			column: 2);
	}
}