using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace UserService.Infrastructure.Hubs.Notification;

[Authorize(Policy = "UserOrAdmin")]
// [AllowAnonymous]
public class NotificationHub(ILogger<NotificationHub> logger) : Hub
{
	private static readonly ConcurrentDictionary<Guid, HashSet<string>> _userConnections = new();

	public override Task OnConnectedAsync()
	{
		if (!Guid.TryParse(Context.UserIdentifier, out var userId))
			return base.OnConnectedAsync();

		_userConnections.AddOrUpdate(
			userId,
			_ => [Context.ConnectionId],
			(_, connections) =>
			{
				connections.Add(Context.ConnectionId);

				return connections;
			});

		return base.OnConnectedAsync();
	}

	public override Task OnDisconnectedAsync(Exception? exception)
	{
		if (!Guid.TryParse(Context.UserIdentifier, out var userId) ||
			!_userConnections.TryGetValue(userId, out var connections))
			return base.OnDisconnectedAsync(exception);

		connections.Remove(Context.ConnectionId);

		if (connections.Count == 0)
			_userConnections.TryRemove(userId, out _);

		return base.OnDisconnectedAsync(exception);
	}

	public static IEnumerable<string> GetConnections(Guid userId)
	{
		return _userConnections
			.TryGetValue(userId, out var connections)
			? connections
			: Enumerable.Empty<string>();
	}
}