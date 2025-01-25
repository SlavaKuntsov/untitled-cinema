using System.Diagnostics;
using System.Text;
using System.Text.Json;

using Brokers.Interfaces;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Brokers.Services.Producers;

public class RabbitMQProducer : RabbitMQBase, IRabbitMQProducer
{
	public RabbitMQProducer(IConnectionFactory connectionFactory)
		: base(connectionFactory)
	{
	}

	public async Task PublishAsync<T>(T message, CancellationToken cancellationToken)
	{
		var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

		await CreateQueueAsync<T>(cancellationToken);
		await PublishBaseAsync<T>(body, cancellationToken);
	}

	public async Task<TResponse> RequestReplyAsync<TRequest, TResponse>(TRequest message, Guid guid, CancellationToken cancellationToken)
	{
		var responseQueueName = $"{typeof(TResponse).Name}"; // create server
		var requestQueueName = $"{typeof(TRequest).Name}"; // create client

		var tcs = new TaskCompletionSource<TResponse>(
			TaskCreationOptions.RunContinuationsAsynchronously);

		string correlationId = guid.ToString();
		var properties = new BasicProperties
		{
			CorrelationId = correlationId,
			ReplyTo = requestQueueName
		};

		await CreateQueueAsync(requestQueueName, cancellationToken, true);

		var consumer = new AsyncEventingBasicConsumer(_channel);

		consumer.ReceivedAsync += async (model, args) =>
		{
			Debug.WriteLine("--------- CorrelationId: " + args.BasicProperties.CorrelationId + " - " + correlationId);

			if (args.BasicProperties.CorrelationId == correlationId)
			{
				var body = JsonSerializer.Deserialize<TResponse>(
							Encoding.UTF8.GetString(args.Body.ToArray()));

				Debug.WriteLine("--------- Get Recieved: " + Encoding.UTF8.GetString(args.Body.ToArray()));

				tcs.TrySetResult((body!));

				await _channel.QueueDeleteAsync(requestQueueName);
			}
			else
			{
				Debug.WriteLine("--------- CorrelationId mismatch: " + args.BasicProperties.CorrelationId + " != " + correlationId);

				tcs.SetException(new InvalidOperationException("CorrelationId mismatch"));
				//throw new InvalidOperationException("CorrelationId mismatch");
			}
		};

		await ConsumeBaseAsync(consumer, requestQueueName, cancellationToken);

		var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

		await PublishBaseAsync(body, responseQueueName, properties, cancellationToken);

		Debug.WriteLine("-------- Send request: " + properties.CorrelationId);

		using CancellationTokenRegistration ctr = cancellationToken.Register(tcs.SetCanceled);

		return await tcs.Task;
	}
}