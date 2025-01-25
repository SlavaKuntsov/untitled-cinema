using System.Text;
using System.Text.Json;

using BookingService.Domain.Entities;
using BookingService.Domain.Interfaces.Repositories;
using BookingService.Domain.Models;

using Brokers.Interfaces;

using MapsterMapper;

namespace BookingService.Application.Consumers.Bookings;

public class CreateBookingsConsumeService(
	IRabbitMQConsumer<BookingModel> rabbitMQConsuner,
	IServiceScopeFactory serviceScopeFactory,
	IMapper mapper) : BackgroundService
{
	private readonly IRabbitMQConsumer<BookingModel> _rabbitMQConsuner = rabbitMQConsuner;
	private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
	private readonly IMapper _mapper = mapper;

	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_rabbitMQConsuner.ConsumeAsync(async (sender, args) =>
		{
			var booking = JsonSerializer.Deserialize<BookingModel>(
				Encoding.UTF8.GetString(args.Body.ToArray()));

			using var scope = _serviceScopeFactory.CreateScope();
			var bookingsRepository = scope.ServiceProvider.GetRequiredService<IBookingsRepository>();

			await bookingsRepository.CreateAsync(_mapper.Map<BookingEntity>(booking!), stoppingToken);
		});

		return Task.CompletedTask;
	}
}