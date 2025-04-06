using BookingService.Domain.Interfaces.Grpc;
using BookingService.Infrastructure.Grpc;
using BookingService.Infrastructure.Hubs.Seats;
using BookingService.Infrastructure.Serializers;

using Microsoft.Extensions.DependencyInjection;

using MongoDB.Bson.Serialization;
using UserService.Application.Interfaces.Notification;

namespace BookingService.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services)
	{
		services.AddSignalR();

		services.AddScoped<IAuthGrpcService, AuthGrpcService>()
			.AddScoped<ISeatsService, SeatsService>();

		BsonSerializer.RegisterSerializer(new BookingStatusSerialization());
		BsonSerializer.RegisterSerializationProvider(new GuidSerialization());

		return services;
	}
}