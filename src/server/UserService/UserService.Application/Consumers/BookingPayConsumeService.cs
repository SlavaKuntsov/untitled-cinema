using Brokers.Interfaces;
using Brokers.Models.Request;
using Brokers.Models.Response;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserService.Application.Handlers.Commands.Users.ChangeBalance;

namespace UserService.Application.Consumers;

public class BookingPayConsumeService(
	IRabbitMQConsumer<BookingPayResponse> rabbitMqConsumer,
	IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await rabbitMqConsumer
			.RequestReplyAsync<BookingPayRequest>(async request =>
				{
					using var scope = serviceScopeFactory.CreateScope();
					var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

					var existUser = await mediator.Send(new ChangeBalanceCommand(
							request.UserId,
							request.Price,
							false),
						stoppingToken);

					return new BookingPayResponse("");
				},
				stoppingToken);
	}
}