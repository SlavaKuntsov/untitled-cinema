using RabbitMQ.Client.Events;

namespace Brokers.Interfaces;

public interface IRabbitMQConsumer<T> 
{
	void ConsumeAsync(AsyncEventHandler<BasicDeliverEventArgs> handler);
}