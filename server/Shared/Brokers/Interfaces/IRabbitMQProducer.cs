
namespace Brokers.Interfaces;

public interface IRabbitMQProducer
{
	Task PublishAsync<T>(T message, CancellationToken cancellationToken);
}