using BookingService.Domain.Models;

using MediatR;

namespace BookingService.Application.Handlers.Commands.Seats.UpdateSeats;

public class UpdateSeatsCommand(
	Guid sessionId,
	IList<SeatModel> seats,
	bool isFromAvailableToReserved) : IRequest
{
	public Guid SessionId { get; private set; } = sessionId;
	public IList<SeatModel> Seats { get; private set; } = seats;
	public bool IsFromAvailableToReserved { get; private set; } = isFromAvailableToReserved;
}