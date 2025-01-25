using BookingService.Domain.Enums;

namespace BookingService.Domain.Entities;

public class BookingEntity
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public Guid SessionId { get; set; }
	public IList<SeatEntity> Seats { get; set; } = [];
	public decimal TotalPrice { get; set; }
	public BookingStatus Status { get; set; } 
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
}