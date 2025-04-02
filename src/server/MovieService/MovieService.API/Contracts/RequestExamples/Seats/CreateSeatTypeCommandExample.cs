using MovieService.Application.Handlers.Commands.Seats.CreateSeatType;

using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Contracts.RequestExamples.Seats;

public class CreateSeatTypeCommandExample : IExamplesProvider<CreateSeatTypeCommand>
{
	public CreateSeatTypeCommand GetExamples()
	{
		return new CreateSeatTypeCommand(
			name: "Комфорт",
			priceModifier: 1);
	}
}