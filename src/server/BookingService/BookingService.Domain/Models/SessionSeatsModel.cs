﻿namespace BookingService.Domain.Models;

public class SessionSeatsModel
{
	public Guid Id { get; private set; }
	public Guid SessionId { get; private set; }
	public IList<SeatModel> AvailableSeats { get; private set; } = [];
	public IList<SeatModel> ReservedSeats { get; private set; } = [];
	public DateTime UpdatedAt { get; private set; }

	public SessionSeatsModel()
	{
	}

	public SessionSeatsModel(
		Guid id,
		Guid sessionId,
		IList<SeatModel> availableSeats,
		IList<SeatModel> reservedSeats)
	{
		Id = id;
		SessionId = sessionId;
		AvailableSeats = availableSeats;
		ReservedSeats = reservedSeats;
		UpdatedAt = DateTime.UtcNow;
	}
}