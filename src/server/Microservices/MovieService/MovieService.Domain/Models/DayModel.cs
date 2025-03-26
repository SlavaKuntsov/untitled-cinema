namespace MovieService.Domain.Models;

public class DayModel
{
	public Guid Id { get; private set; }
	public DateTime StartTime { get; private set; }
	public DateTime EndTime { get; private set; }

	public DayModel()
	{
	}

	public DayModel(
		Guid id,
		DateTime startTime,
		DateTime endTime)
	{
		Id = id;
		StartTime = startTime;
		EndTime = endTime;
	}
}