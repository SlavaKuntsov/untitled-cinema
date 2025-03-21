﻿using System.Text.Json.Serialization;

using BookingService.Domain.Enums;

namespace BookingService.Domain.Models;

public class BookingModel
{
	public Guid Id { get; private set; }
	public Guid UserId { get; private set; }
	public Guid SessionId { get; private set; }
	public IList<SeatModel> Seats { get; private set; } = [];
	public decimal TotalPrice { get; private set; }
	public string Status { get; private set; }
	public DateTime CreatedAt { get; private set; }
	public DateTime UpdatedAt { get; private set; }

	[JsonConstructor]
	public BookingModel(
		Guid id,
		Guid userId,
		Guid sessionId,
		IList<SeatModel> seats,
		decimal totalPrice,
		string status,
		DateTime createdAt,
		DateTime updatedAt)
	{
		Id = id;
		UserId = userId;
		SessionId = sessionId;
		Seats = seats;
		TotalPrice = totalPrice;
		Status = status;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}
}