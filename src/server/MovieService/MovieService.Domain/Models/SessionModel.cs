namespace MovieService.Domain.Models;

public class SessionModel
{
	public Guid Id { get; private set; }
	public Guid MovieId { get; private set; }
	public Guid HallId { get; private set; }
	public Guid DayId { get; private set; }
	public decimal PriceModifier { get; private set; } = 1;
	public DateTime StartTime { get; private set; }
	public DateTime EndTime { get; private set; }

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