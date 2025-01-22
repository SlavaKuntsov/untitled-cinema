using Brokers.Configurations;

using RabbitMQ.Client;

namespace Brokers;

public class RabbitMQConnection
{
	private readonly RabbitMQSettings _settings;

	public RabbitMQConnection(RabbitMQSettings settings)
	{
		_settings = settings;
	}

	public async Task<IConnection> CreateConnectionAsync()
	{
		var factory = new ConnectionFactory
		{
			HostName = _settings.HostName,
			UserName = _settings.UserName,
			Password = _settings.Password,
			Port = _settings.Port,
			VirtualHost = _settings.VirtualHost
		};

		return await factory.CreateConnectionAsync();
	}
}