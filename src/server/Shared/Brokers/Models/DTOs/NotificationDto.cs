namespace Brokers.Models.DTOs;

public record NotificationDto(
	Guid UserId,
	string Message,
	string Type);