using System.ComponentModel;

namespace MovieService.Domain.Enums;

public enum SeatType
{
	None = -1,

	[Description("Стандарт")]
	Standart = 1,

	[Description("Комфорт")]
	Comfort = 2,

	[Description("Диван")]
	Sofa = 3,

	[Description("Кровать")]
	Bed = 4
}