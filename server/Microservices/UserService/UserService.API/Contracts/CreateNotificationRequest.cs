namespace UserService.API.Contracts;

public record CreateNotificationRequest(
	Guid UserId,
	string Message);