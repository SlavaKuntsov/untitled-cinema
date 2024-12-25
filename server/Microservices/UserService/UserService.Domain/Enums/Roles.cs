using System.ComponentModel;

namespace UserService.Domain.Enums;

public enum Role
{
	[Description("Guest")]
	Guest = 1,
	[Description("User")]
	User = 2,
	[Description("Admin")]
	Admin = 3
}