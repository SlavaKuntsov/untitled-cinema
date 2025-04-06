﻿namespace BookingService.Domain.Entities;

public class BookingHistoryEntity
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public IList<BookingRecordEntity> Bookings { get; set; } = [];
}