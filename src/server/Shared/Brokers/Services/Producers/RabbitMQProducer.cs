using System.Text;
using System.Text.Json;
using Brokers.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Brokers.Services.Producers;

public class RabbitMQProducer(
	IConnectionFactory connectionFactory,
	ILogger<RabbitMQProducer> logger)
	: RabbitMQBase(connectionFactory), IRabbitMQProducer
{
	public async Task PublishAsync<T>(T message, CancellationToken cancellationToken)
	{
		var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

		await CreateQueueAsync<T>(cancellationToken);
		await PublishBaseAsync<T>(body, cancellationToken);
	}
	
	public async Task<TResponse> RequestReplyAsync<TRequest, TResponse>(TRequest message, Guid guid, CancellationToken cancellationToken)
	{
		var responseQueueName = $"{typeof(TResponse).Name}";
		var requestQueueName = $"{typeof(TRequest).Name}";

		var tcs = new TaskCompletionSource<TResponse>(
			TaskCreationOptions.RunContinuationsAsynchronously);

		var correlationId = guid.ToString();
		var properties = new BasicProperties
		{
			CorrelationId = correlationId,
			ReplyTo = requestQueueName
		};

		await CreateQueueAsync(requestQueueName, cancellationToken, true);

		var consumer = new AsyncEventingBasicConsumer(_channel);

		consumer.ReceivedAsync += async (model, args) =>
		{
			logger.LogInformation("CorrelationId: " + args.BasicProperties.CorrelationId + " - " + correlationId);

			if (args.BasicProperties.CorrelationId == correlationId)
			{
				var body = JsonSerializer.Deserialize<TResponse>(
					Encoding.UTF8.GetString(args.Body.ToArray()));

				logger.LogInformation("Get Recieved: " + Encoding.UTF8.GetString(args.Body.ToArray()));

				tcs.TrySetResult((body!));

				await _channel.QueueDeleteAsync(requestQueueName);
			}
			else
			{
				logger.LogWarning("CorrelationId mismatch: " + args.BasicProperties.CorrelationId + " != " + correlationId);

				tcs.SetException(new InvalidOperationException("CorrelationId mismatch"));
			}

			await _channel.BasicAckAsync(args.DeliveryTag, multiple: false);
		};

		await ConsumeBaseAsync(consumer, requestQueueName, cancellationToken);

		var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

		await PublishBaseAsync(body, responseQueueName, properties, cancellationToken);

		logger.LogInformation("Send request: " + properties.CorrelationId);

		using CancellationTokenRegistration ctr = cancellationToken.Register(tcs.SetCanceled);

		return await tcs.Task;
	}

	// public async Task<TResponse> RequestReplyAsync<TRequest, TResponse>(
	// 	TRequest message,
	// 	Guid guid,
	// 	CancellationToken cancellationToken)
	// {
	// 	var responseQueueName = $"{typeof(TResponse).Name}";
	// 	var requestQueueName = $"{typeof(TRequest).Name}";
	// 	
	// 	try
	// 	{
	// 		var tcs = new TaskCompletionSource<TResponse>(
	// 			TaskCreationOptions.RunContinuationsAsynchronously);
	//
	// 		var correlationId = guid.ToString();
	//
	// 		var properties = new BasicProperties
	// 		{
	// 			CorrelationId = correlationId,
	// 			ReplyTo = requestQueueName
	// 		};
	//
	// 		await CreateQueueAsync(requestQueueName, cancellationToken, true);
	//
	// 		var consumer = new AsyncEventingBasicConsumer(_channel);
	//
	// 		consumer.ReceivedAsync += async (model, args) =>
	// 		{
	// 			_logger.LogInformation(
	// 				"CorrelationId: " + args.BasicProperties.CorrelationId + " - " + correlationId);
	//
	// 			if (args.BasicProperties.CorrelationId == correlationId)
	// 			{
	// 				var body = JsonSerializer.Deserialize<TResponse>(
	// 					Encoding.UTF8.GetString(args.Body.ToArray()));
	//
	// 				_logger.LogInformation("Get Recieved: " + Encoding.UTF8.GetString(args.Body.ToArray()));
	//
	// 				tcs.TrySetResult(body!);
	//
	// 				await _channel.QueueDeleteAsync(requestQueueName, cancellationToken: cancellationToken);
	// 			}
	// 			else
	// 			{
	// 				_logger.LogWarning(
	// 					"CorrelationId mismatch: " + args.BasicProperties.CorrelationId + " != " +
	// 					correlationId);
	//
	// 				tcs.SetException(new InvalidOperationException("CorrelationId mismatch"));
	// 			}
	//
	// 			await _channel.BasicAckAsync(args.DeliveryTag, false, cancellationToken);
	// 		};
	//
	// 		await ConsumeBaseAsync(consumer, requestQueueName, cancellationToken);
	//
	// 		var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
	//
	// 		await PublishBaseAsync(body, responseQueueName, properties, cancellationToken);
	//
	// 		_logger.LogInformation("Send request: " + properties.CorrelationId);
	//
	// 		await using var ctr = cancellationToken.Register(tcs.SetCanceled);
	//
	// 		return await tcs.Task;
	// 	}
	// 	finally
	// 	{
	// 		await _channel.QueueDeleteAsync(responseQueueName, cancellationToken: cancellationToken);
	// 	}
	// }
}