using System.Diagnostics;
using System.Text;
using System.Text.Json;

using Brokers.Interfaces;
using Brokers.Services;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Brokers.Services.Consumers;

public class RabbitMQConsumer<TResponse> : RabbitMQBase, IRabbitMQConsumer<TResponse>
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

		await CreateQueueAsync<TResponse>();
		await ConsumeBaseAsync<TResponse>(_consumer);
	}

	public async Task RequestReplyAsync<TRequest>(Func<TRequest, Task<TResponse>> handler, CancellationToken cancellationToken)
	{
		var responseQueueName = $"{typeof(TResponse).Name}";

		await CreateQueueAsync(responseQueueName, cancellationToken);
		await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

		var consumer = new AsyncEventingBasicConsumer(_channel);

		consumer.ReceivedAsync += async (sender, args) =>
		{
			Debug.WriteLine("--------- CorrelationId consumer: " + args.BasicProperties.CorrelationId);

			var properties = new BasicProperties
			{
				ReplyTo = args.BasicProperties.ReplyTo,
				CorrelationId = args.BasicProperties.CorrelationId
			};

			var body = JsonSerializer.Deserialize<TRequest>(
					Encoding.UTF8.GetString(args.Body.ToArray()));

			Debug.WriteLine("--------- Get Recieved: " + body);

			var response = await handler(body);

			var responseBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));

			await PublishBaseAsync(responseBody, properties.ReplyTo!, properties, cancellationToken);

			Debug.WriteLine("-------- Send request: " + responseBody);
		};

		await ConsumeBaseAsync(consumer, responseQueueName, cancellationToken);
	}
}