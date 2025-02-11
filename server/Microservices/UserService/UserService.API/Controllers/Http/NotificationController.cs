using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using UserService.API.Contracts;
using UserService.Application.Interfaces.Notification;

namespace UserService.API.Controllers.Http;

[ApiController]
[Route("[controller]")]
public class NotificationController : ControllerBase
{
	private readonly INotificationService _notificationService;

	public NotificationController(INotificationService notificationService)
	{
		_notificationService = notificationService;
	}

	[HttpPost("/Notifications/Send")]
	public async Task<IActionResult> Push([FromBody] CreateNotificationRequest request)
	{
		await _notificationService.SendAsync(request.UserId, request.Message);

		return Ok();
	}
}