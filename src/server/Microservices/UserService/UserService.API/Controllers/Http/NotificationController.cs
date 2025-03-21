using System.Security.Claims;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using UserService.Application.Handlers.Commands.Notifications.DeleteNotification;
using UserService.Application.Handlers.Commands.Notifications.SendNotidication;
using UserService.Application.Handlers.Queries.Notifications.GetUserNotifications;
using UserService.Application.Interfaces.Notification;

namespace UserService.API.Controllers.Http;

[ApiController]
[Route("[controller]")]
public class NotificationController : ControllerBase
{
	private readonly IMediator _mediator;
	private readonly INotificationService _notificationService;

	public NotificationController(
		IMediator mediator,
		INotificationService notificationService)
	{
		_mediator = mediator;
		_notificationService = notificationService;
	}

	[HttpGet("/notifications")]
	[Authorize(Policy = "UserOrAdmin")]
	public async Task<IActionResult> Get(CancellationToken cancellationToken)
	{
		var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
			?? throw new UnauthorizedAccessException("User ID not found in claims.");

		if (!Guid.TryParse(userIdClaim.Value, out var userId))
			throw new UnauthorizedAccessException("Invalid User ID format in claims.");

		var notifications = await _mediator.Send(
			new GetUserNotificationsQuery(userId), cancellationToken);

		return Ok(notifications);
	}

	[HttpPost("/notifications/")]
	[Authorize(Policy = "UserOrAdmin")]
	public async Task<IActionResult> Send([FromBody] string message, CancellationToken cancellationToken)
	{
		var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
			?? throw new UnauthorizedAccessException("User ID not found in claims.");

		if (!Guid.TryParse(userIdClaim.Value, out var userId))
			throw new UnauthorizedAccessException("Invalid User ID format in claims.");

		//await _notificationService.SendAsync(userId, message, cancellationToken);

		await _mediator.Send(new SendNotificationCommand(userId, message), cancellationToken);

		return Ok();
	}

	[HttpDelete("/notifications/{id:Guid}")]
	[Authorize(Policy = "UserOrAdmin")]
	public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
	{
		await _mediator.Send(new DeleteNotificationCommand(id), cancellationToken);

		return NoContent();
	}
}