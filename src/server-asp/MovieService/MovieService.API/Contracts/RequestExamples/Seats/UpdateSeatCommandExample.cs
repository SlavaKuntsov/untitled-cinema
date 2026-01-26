using MovieService.Application.Handlers.Commands.Seats.UpdateSeat;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts.RequestExamples.Seats;

public class UpdateSeatCommandExample : IExamplesProvider<UpdateSeatCommand>
{
	public UpdateSeatCommand GetExamples()
	{
		return new UpdateSeatCommand(
			Id: Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
			HallId: Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
			SeatTypeId: Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
			Row: 2,
			Column: 2);
	}
}