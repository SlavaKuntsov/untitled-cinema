namespace BookingService.Domain.Models;

public class BookingHistoryModel
{
	public Guid Id { get; private set; }
	public Guid UserId { get; private set; }
	public List<BookingRecordModel> Bookings { get; private set; } = [];
}