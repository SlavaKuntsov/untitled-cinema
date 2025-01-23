using System.ComponentModel;

namespace MovieService.Domain.Enums;

public enum SeatType
{
	None = -1,
	[Description("Стандарт")]
	Standart = 0,
	[Description("Комфорт")]
	Comfort = 1,
	[Description("Диван")]
	Sofa = 2,
	[Description("Кровать")]
	Bed = 3
}
