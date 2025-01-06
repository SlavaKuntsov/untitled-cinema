namespace MovieService.Domain.Models;

public class GenreModel
{
	public Guid Id { get; set; }
	public string Name { get; set; } = null!;

	public GenreModel() { }

	public GenreModel(Guid id, string name)
	{
		Id = id;
		Name = name;
	}
}