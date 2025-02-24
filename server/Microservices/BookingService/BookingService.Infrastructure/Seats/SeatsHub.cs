using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

using Microsoft.Extensions.Logging;

namespace BookingService.Infrastructure.Seats;

public class SeatsHub(ILogger<SeatsHub> logger) : Hub
{
	private static readonly ConcurrentDictionary<Guid, HashSet<string>> _userConnections = new();

	public override Task OnConnectedAsync()
	{
		if (Guid.TryParse(Context.UserIdentifier, out Guid userId))
		{
			logger.LogError("NEW CONNECT - " + userId.ToString());
			logger.LogError("connect list = " + _userConnections.Count());

			_userConnections.AddOrUpdate(userId,
				_ => [Context.ConnectionId],
				(_, connections) =>
				{
					connections.Add(Context.ConnectionId);
					return connections;
				});
		}

		return base.OnConnectedAsync();
	}

	public override Task OnDisconnectedAsync(Exception? exception)
	{
		if (Guid.TryParse(Context.UserIdentifier, out Guid userId) &&
			_userConnections.TryGetValue(userId, out var connections))
		{
			logger.LogError("NEW DISCONNECT - " + userId.ToString());

			connections.Remove(Context.ConnectionId);

			if (connections.Count == 0)
				_userConnections.TryRemove(userId, out _);
		}

		return base.OnDisconnectedAsync(exception);
	}

	public static IEnumerable<string> GetConnections(Guid userId)
	{
		return _userConnections
			.TryGetValue(userId, out var connections) ?
			connections :
			Enumerable.Empty<string>();
	}
}
