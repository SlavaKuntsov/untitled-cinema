using System.Reflection;
using BookingService.API.Behaviors;
using BookingService.API.Controllers.Grpc;
using BookingService.Infrastructure.Hubs.Seats;
using Domain.Constants;
using Extensions.Exceptions.Middlewares;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Protobufs.Auth;
using Utilities.DateTime;

namespace BookingService.API.Extensions;

public static class ApiExtensions
{
	public static IServiceCollection AddAPI(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		var usersPort = Environment.GetEnvironmentVariable("USERS_APP_PORT");

		if (string.IsNullOrEmpty(usersPort))
			usersPort = configuration.GetValue<string>("ApplicationSettings:UsersPort");

		services.AddGrpcClient<AuthService.AuthServiceClient>(
			options =>
			{
				options.Address = new Uri($"https://localhost:{usersPort}");
			});

		var hangfireConnectionString =
			Environment.GetEnvironmentVariable("HANGFIRE_CONNECTION_STRING");

		if (string.IsNullOrEmpty(hangfireConnectionString))
			hangfireConnectionString = configuration.GetConnectionString("HangfireDb");

		var mongoUrlBuilder = new MongoUrlBuilder(hangfireConnectionString);
		var mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());

		services.AddHangfire(
			configuration => configuration
				.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
				.UseSimpleAssemblyNameTypeSerializer()
				.UseRecommendedSerializerSettings()
				.UseMongoStorage(
					mongoClient,
					mongoUrlBuilder.DatabaseName,
					new MongoStorageOptions
					{
						MigrationOptions = new MongoMigrationOptions
						{
							MigrationStrategy = new MigrateMongoMigrationStrategy(),
							BackupStrategy = new NoneMongoBackupStrategy()
						},
						CheckQueuedJobsStrategy =
							CheckQueuedJobsStrategy.TailNotificationsCollection,
						Prefix = "hangfire.mongo",
						CheckConnection = true
					}));

		services.AddHangfireServer(
			serverOptions =>
			{
				serverOptions.ServerName = "Hangfire.Mongo server 1";
			});

		services.AddControllers()
			.AddJsonOptions(
				options =>
				{
					options.JsonSerializerOptions.Converters.Add(
						new DateTimeConverter(DateTimeConstants.DATE_TIME_FORMAT));
				});

		services.AddSwaggerGen(
			options =>
			{
				options.AddSecurityDefinition(
					"Bearer",
					new OpenApiSecurityScheme
					{
						Name = "Authorization",
						In = ParameterLocation.Header,
						Type = SecuritySchemeType.Http,
						Scheme = "Bearer",
						BearerFormat = "JWT",
						Description = "Введите токен JWT в формате 'Bearer {токен}'"
					});

				options.AddSecurityRequirement(
					new OpenApiSecurityRequirement
					{
						{
							new OpenApiSecurityScheme
							{
								Reference = new OpenApiReference
								{
									Type = ReferenceType.SecurityScheme,
									Id = "Bearer"
								}
							},
							Array.Empty<string>()
						}
					});
			});

		services.AddGrpc(
			options =>
			{
				options.Interceptors.Add<GlobalGrpcExceptionInterceptor>();
			});

		services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

		var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
		typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());

		Mapper mapperConfig = new(typeAdapterConfig);
		services.AddSingleton<IMapper>(mapperConfig);

		return services;
	}

	public static WebApplication AddAPI(this WebApplication app)
	{
		app.MapGrpcService<BookingController>();

		app.MapHub<SeatsHub>("/seatsHub");

		return app;
	}
}