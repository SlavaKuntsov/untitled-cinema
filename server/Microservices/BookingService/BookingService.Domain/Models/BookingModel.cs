using BookingService.Domain.Entities;
using BookingService.Domain.Enums;

namespace BookingService.Domain.Models;

public class BookingModel
{
	public Guid Id { get; private set; }
	public Guid UserId { get; private set; }
	public Guid SessionId { get; private set; }
	public List<SeatModel> Seats { get; private set; } = [];
	public decimal TotalPrice { get; private set; }
	public BookingStatus Status { get; private set; }
	public DateTime CreatedAt { get; private set; }
	public DateTime UpdatedAt { get; private set; }

	public BookingModel(
		Guid id,
		Guid userId,
		Guid sessionId,
		List<SeatModel> seats,
		BookingStatus status,
		DateTime createdAt,
		DateTime updatedAt)
	{
		Id = id;
		UserId = userId;
		SessionId = sessionId;
		Seats = seats;
		Status = status;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}

	public BookingModel(
		Guid id,
		Guid userId,
		Guid sessionId,
		List<SeatModel> seats,
		decimal totalPrice,
		BookingStatus status,
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