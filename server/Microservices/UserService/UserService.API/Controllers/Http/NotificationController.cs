using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using UserService.Application.Interfaces.Auth;
using UserService.Application.Interfaces.Notification;

namespace UserService.API.Controllers.Http;

[ApiController]
[Route("[controller]")]
public class NotificationController : ControllerBase
{
	private readonly IMediator _mediator;
	private readonly INotificationService _notificationService;
	private readonly IMapper _mapper;

	public NotificationController(IMediator mediator, 
		INotificationService notificationService, 
		IMapper mapper)
	{
		_mediator = mediator;
		_notificationService = notificationService;
		_mapper = mapper;
	}

	[HttpPost("/Notifications/Send")]
	public async Task<IActionResult> Push([FromBody] SendNotification request)
	{
		await _notificationService.SendAsync(request.UserId, request.Message);

		return Ok();
	}
}

public class SendNotification(Guid userId, string message)
{
	public Guid UserId { get; set; } = userId;
	public string Message { get; set; } = message;
}