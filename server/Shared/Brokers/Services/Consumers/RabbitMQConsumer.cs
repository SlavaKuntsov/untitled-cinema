using Brokers.Interfaces;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Brokers.Services.Consumers;

public class RabbitMQConsumer<T> : RabbitMQBase, IRabbitMQConsumer<T>
{
	private readonly AsyncEventingBasicConsumer _consumer;
	public RabbitMQConsumer(IConnectionFactory connectionFactory)
		: base(connectionFactory)
	{
		_consumer = new AsyncEventingBasicConsumer(_channel!);
	}

	public async void ConsumeAsync(AsyncEventHandler<BasicDeliverEventArgs> handler)
	{
		_consumer.ReceivedAsync += handler;

		await CreateQueueAsync<T>();

		await ConsumeBaseAsync<T>(_consumer);
	}
}