using BookingService.Application.Handlers.Commands.Seats.CreateEmptySeats;
using BookingService.Domain.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Protobufs.Seats;

namespace BookingService.API.Controllers.Grpc;

public class BookingController(IMediator mediator) : SeatsService.SeatsServiceBase
{
	public override async Task<Empty> CreateSeats(CreateSeatsRequest request, ServerCallContext context)
	{
		var seats = new List<SeatModel>(request.Seats.Count);

		foreach (var seat in request.Seats)
			seats.Add(new SeatModel(Guid.Parse(seat.Id), seat.Row, seat.Column));

		Console.WriteLine(seats);

		await mediator.Send(
			new CreateEmptySeats(Guid.Parse(request.SessionId), seats),
			context.CancellationToken);

		Console.WriteLine("Created empty seats.");
		
		return await Task.FromResult(new Empty());
	}
}