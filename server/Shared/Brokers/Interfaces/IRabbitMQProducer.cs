
namespace Brokers.Interfaces;

public interface IRabbitMQProducer
{
	Task PublishAsync<T>(T message, CancellationToken cancellationToken);
	Task<TResponse> RequestReplyAsync<TRequest, TResponse>(TRequest message, Guid guid, CancellationToken cancellationToken);
}