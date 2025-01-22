using RabbitMQ.Client;

namespace Brokers.Services;

public class RabbitMQBase : IDisposable
{
	protected readonly IChannel _channel;
	private readonly IConnection _connection;

	protected RabbitMQBase(IConnectionFactory factory)
	{
		_connection = factory.CreateConnectionAsync().Result;
		_channel = _connection.CreateChannelAsync().Result;
	}

	protected async Task CreateQueueAsync<TQueue>(CancellationToken token = default)
	{
		if (_channel is null)
			throw new InvalidOperationException("Channel not initialized.");

		await _channel.QueueDeclareAsync(
			queue: typeof(TQueue).Name,
			durable: true,
			exclusive: false,
			autoDelete: false,
			cancellationToken: token);
	}

	protected async Task PublishBaseAsync<TQueue>(byte[] messageBody, CancellationToken token)
	{
		if (_channel is null)
			throw new InvalidOperationException("Channel not initialized.");

		await _channel.BasicPublishAsync(
			exchange: "",
			routingKey: typeof(TQueue).Name,
			mandatory: true,
			body: messageBody,
			cancellationToken: token);
	}

	protected async Task ConsumeBaseAsync<TQueue>(IAsyncBasicConsumer consumer, CancellationToken token = default)
	{
		if (_channel == null)
			throw new InvalidOperationException("Channel not initialized.");

		await _channel.BasicConsumeAsync(
			queue: typeof(TQueue).Name,
			autoAck: true,
			consumer: consumer,
			noLocal: false,
			exclusive: false,
			consumerTag: Guid.NewGuid().ToString(),
			arguments: null,
			cancellationToken: token);
	}

	public void Dispose()
	{
		if (_channel != null)
		{
			_connection?.Dispose();
			_channel?.Dispose();
		}
	}
}