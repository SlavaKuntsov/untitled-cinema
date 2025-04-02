namespace MovieService.Domain.Models;

public class SeatTypeModel
{
	public Guid Id { get; private set; }
	public string Name { get; private set; } = null!;
	public decimal PriceModifier { get; private set; } = 1;

	public SeatTypeModel()
	{
	}

	public SeatTypeModel(Guid id, string name, decimal priceModifier)
	{
		Id = id;
		Name = name;
		PriceModifier = priceModifier;
	}
}
