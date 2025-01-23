namespace MovieService.Domain.Models;

public class SeatTypeModel
{
	public Guid Id { get; set; }
	public string Name { get; set; } = null!;
	public decimal PriceModifier { get; set; } = 1;

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
