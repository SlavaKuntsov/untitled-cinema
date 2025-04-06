using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BookingService.Infrastructure.Hubs.Seats;

// [Authorize(Policy = "UserOrAdmin")]
[AllowAnonymous]
public class SeatsHub(ILogger<SeatsHub> logger) : Hub
{
	private static readonly ConcurrentDictionary<Guid, HashSet<string>> _sessionGroups = new();

	public async Task JoinSession(Guid sessionId)
	{
		var groupName = GetGroupName(sessionId);
		await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

		_sessionGroups.AddOrUpdate(
			sessionId,
			_ =>
			[
				Context.ConnectionId
			],
			(_, connections) =>
			{
				connections.Add(Context.ConnectionId);

				return connections;
			});

		logger.LogInformation($"User {Context.UserIdentifier} joined session {sessionId}");
	}

	public async Task LeaveSession(Guid sessionId)
	{
		var groupName = GetGroupName(sessionId);
		await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

		if (_sessionGroups.TryGetValue(sessionId, out var connections))
		{
			connections.Remove(Context.ConnectionId);

			if (connections.Count == 0)
				_sessionGroups.TryRemove(sessionId, out _);
		}

		logger.LogInformation($"User {Context.UserIdentifier} left session {sessionId}");
	}

	private static string GetGroupName(Guid sessionId)
	{
		return $"session-{sessionId}";
	}
}