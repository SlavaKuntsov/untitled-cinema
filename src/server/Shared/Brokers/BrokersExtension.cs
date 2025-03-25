using Brokers.Interfaces;
using Brokers.Services.Consumers;
using Brokers.Services.Producers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Brokers;

public static class BrokersExtension
{
	public static IServiceCollection AddRabbitMQ(
		this IServiceCollection services, 
		IConfiguration configuration)
	{
		var rabbitHostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
		var rabbitPortString = Environment.GetEnvironmentVariable("RABBITMQ_PORT");

		if (string.IsNullOrEmpty(rabbitHostName))
			rabbitHostName = configuration.GetValue<string>("RabbitMQ:HostName");

		if (string.IsNullOrEmpty(rabbitPortString))
			rabbitPortString = configuration.GetValue<string>("RabbitMQ:Port");

		if (!int.TryParse(rabbitPortString, out var rabbitPort))
			throw new InvalidOperationException($"Invalid port value: {rabbitPortString}");

		services.AddSingleton<IConnectionFactory>(_ => new ConnectionFactory
		{
			HostName = rabbitHostName!,
			Port = rabbitPort,
			UserName = "guest",
			Password = "guest"
		});

		services.AddSingleton(typeof(IRabbitMQConsumer<>), typeof(RabbitMQConsumer<>));
		services.AddSingleton<IRabbitMQProducer, RabbitMQProducer>();

		return services;
	}
}