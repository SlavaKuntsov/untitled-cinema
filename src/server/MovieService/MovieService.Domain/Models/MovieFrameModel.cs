namespace MovieService.Domain.Models;

public class MovieFrameModel
{
	public Guid Id { get; set; }
	public Guid MovieId { get; set; }
	public string FrameName { get; set; }
	public int Order { get; set; }

	public MovieFrameModel(
		Guid id,
		Guid movieId,
		string frameName,
		int order)
	{
		Id = id;
		MovieId = movieId;
		FrameName = frameName;
		Order = order;
	}
}