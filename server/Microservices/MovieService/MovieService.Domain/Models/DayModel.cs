namespace MovieService.Domain.Models;

public class DayModel
{
	public Guid Id { get; set; }
	public DateTime StartTime { get; set; }
	public DateTime EndTime { get; set; }

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