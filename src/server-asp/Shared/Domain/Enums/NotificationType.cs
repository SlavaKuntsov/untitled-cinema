using System.ComponentModel;

namespace Domain.Enums;

public enum NotificationType
{
	[Description(nameof(Success))]
	Success,
	[Description(nameof(Error))]
	Error,
	[Description(nameof(Info))]
	Info,
	[Description(nameof(Warn))]
	Warn
}