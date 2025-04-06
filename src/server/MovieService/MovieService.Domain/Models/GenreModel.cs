namespace MovieService.Domain.Models;

public class GenreModel
{
	public Guid Id { get; private set; }
	public string Name { get; private set; } = null!;

	public GenreModel() 
	{
	}

	public GenreModel(Guid id, string name)
	{
		Id = id;
		Name = name;
	}
}