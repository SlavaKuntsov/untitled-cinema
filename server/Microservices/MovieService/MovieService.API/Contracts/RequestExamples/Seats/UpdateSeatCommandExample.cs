using MovieService.Application.Handlers.Commands.Seats.UpdateSeat;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts.RequestExamples.Seats;

public class UpdateSeatCommandExample : IExamplesProvider<UpdateSeatCommand>
{
	public UpdateSeatCommand GetExamples()
	{
		return new UpdateSeatCommand(
			Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
			Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
			Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
			2,
			2);
	}
}