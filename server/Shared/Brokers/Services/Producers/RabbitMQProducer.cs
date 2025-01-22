using System.Text;
using System.Text.Json;

using Brokers.Interfaces;

using RabbitMQ.Client;

namespace Brokers.Services.Producers;

public class RabbitMQProducer : RabbitMQBase, IRabbitMQProducer
{
	public RabbitMQProducer(IConnectionFactory connectionFactory)
		: base(connectionFactory)
	{
	}

	public async Task PublishAsync<T>(T message, CancellationToken cancellationToken)
	{
		var json = JsonSerializer.Serialize(message);
		var body = Encoding.UTF8.GetBytes(json);

		await CreateQueueAsync<T>(cancellationToken);
		await PublishBaseAsync<T>(body, cancellationToken);
	}
}