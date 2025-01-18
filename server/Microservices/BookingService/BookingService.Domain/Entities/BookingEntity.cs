using BookingService.Domain.Enums;

namespace BookingService.Domain.Entities;

public class BookingEntity
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public Guid SessionId { get; set; }
	public List<SeatEntity> Seats { get; set; } = [];
	public decimal TotalPrice { get; set; }
	public BookingStatus Status { get; set; } 
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }

	public BookingEntity(
		Guid id,
		Guid userId,
		Guid sessionId,
		List<SeatEntity> seats,
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