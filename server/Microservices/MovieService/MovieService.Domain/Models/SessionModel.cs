namespace MovieService.Domain.Models;

public class SessionModel
{
	public Guid Id { get; set; }
	public Guid MovieId { get; set; }
	public Guid HallId { get; set; }
	public Guid DayId { get; set; }
	public decimal PriceModifier { get; set; } = 1;
	public DateTime StartTime { get; set; }
	public DateTime EndTime { get; set; }

	public SessionModel()
	{
	}

	public SessionModel(
		Guid id,
		Guid movieId,
		Guid hallId,
		Guid dayId,
		decimal priceModifier,
		DateTime startTime,
		DateTime endTime)
	{
		Id = id;
		MovieId = movieId;
		HallId = hallId;
		DayId = dayId;
		PriceModifier = priceModifier;
		StartTime = startTime;
		EndTime = endTime;
	}
}