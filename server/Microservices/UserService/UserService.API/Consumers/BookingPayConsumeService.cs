using Brokers.Interfaces;
using Brokers.Models.Request;
using Brokers.Models.Response;

using MapsterMapper;

using MediatR;

using UserService.Application.Handlers.Commands.Users.ChangeBalance;

namespace UserService.API.Consumers;

public class BookingPayConsumeService(
	IRabbitMQConsumer<BookingPayResponse> rabbitMQConsuner,
	IServiceScopeFactory serviceScopeFactory,
	IMapper mapper) : BackgroundService
{
	private readonly IRabbitMQConsumer<BookingPayResponse> _rabbitMQConsuner = rabbitMQConsuner;
	private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
	private readonly IMapper _mapper = mapper;

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await _rabbitMQConsuner
			.RequestReplyAsync<BookingPayRequest>(async (request) =>
			{
				using var scope = _serviceScopeFactory.CreateScope();
				var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

				var existUser = await mediator.Send(new ChangeBalanceCommand(
					request.UserId,
					request.Price,
					false));

				return new BookingPayResponse("");
			}, stoppingToken);

		return;
	}
}