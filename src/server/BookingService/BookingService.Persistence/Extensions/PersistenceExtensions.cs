using BookingService.Domain.Interfaces.Repositories;
using BookingService.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;

namespace BookingService.Persistence.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
                         ?? configuration.GetConnectionString("BookingServiceDBContext");

        var databaseName = Environment.GetEnvironmentVariable("DATABASE_NAME")
                      ?? configuration["MongoDb:DatabaseName"];

        if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(databaseName))
          throw new InvalidOperationException("Databases configuration is missing.");

        // Глобальная настройка обработки дат в MongoDB
        ConfigureMongoDbConventions();

        services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));

        services.AddScoped(
          sp =>
          {
             var client = sp.GetRequiredService<IMongoClient>();
             return new BookingServiceDBContext(client, databaseName);
          });

        services.AddScoped<IBookingsRepository, BookingsRepository>();
        services.AddScoped<ISessionSeatsRepository, SessionSeatsRepository>();

        return services;
    }

    private static void ConfigureMongoDbConventions()
    {
        // Регистрация глобального сериализатора для DateTime
        BsonSerializer.RegisterSerializer(typeof(DateTime), 
            new DateTimeSerializer(DateTimeKind.Utc));
            
        // Установка конвенций
        var pack = new ConventionPack
        {
            new EnumRepresentationConvention(BsonType.String),
            new IgnoreExtraElementsConvention(true),
            new DateTimeSerializationConvention()
        };
        
        ConventionRegistry.Register("ApplicationConventions", pack, t => true);
    }
}

public class DateTimeSerializationConvention : ConventionBase, IMemberMapConvention
{
    public void Apply(BsonMemberMap memberMap)
    {
        if (memberMap.MemberType == typeof(DateTime) || 
            memberMap.MemberType == typeof(DateTime?))
        {
            memberMap.SetSerializer(new DateTimeSerializer(DateTimeKind.Utc));
        }
        
        // Если в модели используется DateTimeOffset, можно добавить обработку и для него
        if (memberMap.MemberType == typeof(DateTimeOffset) || 
            memberMap.MemberType == typeof(DateTimeOffset?))
        {
            memberMap.SetSerializer(new DateTimeOffsetSerializer(BsonType.Document));
        }
    }
}