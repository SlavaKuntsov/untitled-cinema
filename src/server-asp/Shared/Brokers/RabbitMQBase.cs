using RabbitMQ.Client;

namespace Brokers.Services;

public class RabbitMQBase : IAsyncDisposable
{
	protected readonly IChannel _channel;

	private readonly IConnection _connection;

	protected RabbitMQBase(IConnectionFactory factory)
	{
		_connection = factory.CreateConnectionAsync().Result;
		_channel = _connection.CreateChannelAsync().Result;
	}

	public async ValueTask DisposeAsync()
	{
		if (_channel is not null)
			await _channel.CloseAsync();

		if (_connection is not null)
			await _connection.CloseAsync();
	}

	protected async Task<QueueDeclareOk> CreateQueueAsync<TQueue>(
		CancellationToken token = default,
		bool exclusive = false)
	{
		if (_channel is null)
			throw new InvalidOperationException("Channel not initialized.");

		var queue = await _channel.QueueDeclareAsync(
			typeof(TQueue).Name,
			true,
			false,
			false,
			cancellationToken: token);

		return queue;
	}

	protected async Task<QueueDeclareOk> CreateQueueAsync(
		string name,
		CancellationToken token = default,
		bool exclusive = false)
	{
		if (_channel is null)
			throw new InvalidOperationException("Channel not initialized.");

		var queue = await _channel.QueueDeclareAsync(
			name,
			true,
			exclusive,
			false,
			cancellationToken: token);

		return queue;
	}

	protected async Task PublishBaseAsync<TQueue>(byte[] messageBody, CancellationToken token)
	{
		if (_channel is null)
			throw new InvalidOperationException("Channel not initialized.");

		await _channel.BasicPublishAsync(
			"",
			typeof(TQueue).Name,
			true,
			messageBody,
			token);
	}

	protected async Task PublishBaseAsync(
		byte[] messageBody,
		string routingKey,
		BasicProperties properties,
		CancellationToken token)
	{
		if (_channel is null)
			throw new InvalidOperationException("Channel not initialized.");

		await _channel.BasicPublishAsync(
			"",
			routingKey,
			true,
			body: messageBody,
			basicProperties: properties,
			cancellationToken: token);
	}

	protected async Task ConsumeBaseAsync<TQueue>(
		IAsyncBasicConsumer consumer,
		CancellationToken token = default)
	{
		if (_channel == null)
			throw new InvalidOperationException("Channel not initialized.");

		await _channel.BasicConsumeAsync(
			typeof(TQueue).Name,
			true,
			consumer: consumer,
			noLocal: false,
			exclusive: false,
			consumerTag: Guid.NewGuid().ToString(),
			arguments: null,
			cancellationToken: token);
	}

	protected async Task ConsumeBaseAsync(
		IAsyncBasicConsumer consumer,
		string queueName,
		CancellationToken token = default)
	{
		if (_channel == null)
			throw new InvalidOperationException("Channel not initialized.");

		await _channel.BasicConsumeAsync(
			queueName,
			false,
			consumer: consumer,
			noLocal: false,
			exclusive: false,
			consumerTag: Guid.NewGuid().ToString(),
			arguments: null,
			cancellationToken: token);
	}
}