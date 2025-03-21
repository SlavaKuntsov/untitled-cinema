using MovieService.Application.Handlers.Commands.Seats.UpdateSeat;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts.RequestExamples.Seats;

public class UpdateSeatCommandExample : IExamplesProvider<UpdateSeatCommand>
{
	public UpdateSeatCommand GetExamples()
	{
		return new UpdateSeatCommand(
			id: Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
			hallId: Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
			seatTypeId: Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
			row: 2,
			column: 2);
	}
}