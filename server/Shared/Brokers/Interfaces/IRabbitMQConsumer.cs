using RabbitMQ.Client.Events;

namespace Brokers.Interfaces;

public interface IRabbitMQConsumer<TResponse> 
{
	void ConsumeAsync(AsyncEventHandler<BasicDeliverEventArgs> handler);
	Task RequestReplyAsync<TRequest>(Func<TRequest, Task<TResponse>> handler, CancellationToken cancellationToken);
}