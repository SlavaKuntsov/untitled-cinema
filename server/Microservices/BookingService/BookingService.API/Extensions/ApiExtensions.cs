using System.Diagnostics;
using System.Reflection;
using System.Text;

using BookingService.API.Behaviors;
using BookingService.API.Consumers.Bookings;
using BookingService.API.Middlewares;
using BookingService.Domain.Constants;
using BookingService.Infrastructure.Auth;

using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;

using Mapster;

using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using MongoDB.Driver;

using Protobufs.Auth;

using Serilog;

namespace BookingService.API.Extensions;

public static class ApiExtensions
{
	public static IServiceCollection AddAPI(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddExceptionHandler<GlobalExceptionHandler>();
		services.AddProblemDetails();
		services.AddHealthChecks();

		services.AddHostedService<CreateBookingsConsumeService>();

		var usersPort = Environment.GetEnvironmentVariable("USERS_APP_PORT");

		if (string.IsNullOrEmpty(usersPort))
			usersPort = configuration.GetValue<string>("ApplicationSettings:UsersPort");

		services.AddGrpcClient<AuthService.AuthServiceClient>(options =>
		{
			options.Address = new Uri($"https://localhost:{usersPort}");
		});

		var hangfireConnectionString = Environment.GetEnvironmentVariable("HANGFIRE_CONNECTION_STRING");

		if (string.IsNullOrEmpty(hangfireConnectionString))
		{
			hangfireConnectionString = configuration.GetConnectionString("HangfireDb");
		}

		var mongoUrlBuilder = new MongoUrlBuilder(hangfireConnectionString);
		var mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());

		services.AddHangfire(configuration => configuration
			.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
			.UseSimpleAssemblyNameTypeSerializer()
			.UseRecommendedSerializerSettings()
			.UseMongoStorage(mongoClient, mongoUrlBuilder.DatabaseName, new MongoStorageOptions
			{
				MigrationOptions = new MongoMigrationOptions
				{
					MigrationStrategy = new MigrateMongoMigrationStrategy(),
					BackupStrategy = new NoneMongoBackupStrategy(),

				},
				CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection,
				Prefix = "hangfire.mongo",
				CheckConnection = true
			}));

		services.AddHangfireServer(serverOptions =>
		{
			serverOptions.ServerName = "Hangfire.Mongo server 1";
		});

		services.AddHttpContextAccessor();

		services.AddControllers()
			.AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.Converters.Add(
					new Infrastructure.DateTime.DateTimeConverter(DateTimeConstants.DATE_TIME_FORMAT));
			});
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen(options =>
		{
			options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
			{
				Name = "Authorization",
				In = ParameterLocation.Header,
				Type = SecuritySchemeType.Http,
				Scheme = "Bearer",
				BearerFormat = "JWT",
				Description = "Введите токен JWT в формате 'Bearer {токен}'"
			});
			options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

		TypeAdapterConfig typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
		typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());

		Mapper mapperConfig = new(typeAdapterConfig);
		services.AddSingleton<IMapper>(mapperConfig);

		JwtModel? jwtOptions = configuration.GetSection(nameof(JwtModel)).Get<JwtModel>();

		services
			.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
			{
				options.RequireHttpsMetadata = true;
				options.SaveToken = true;
				options.TokenValidationParameters = new()
				{
					ValidateIssuer = false,
					ValidateAudience = false,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(
						Encoding.UTF8.GetBytes(jwtOptions!.SecretKey))
				};
				options.Events = new JwtBearerEvents
				{
					OnAuthenticationFailed = context =>
					{
						Debug.WriteLine(context.Request.Headers.Authorization);
						Debug.WriteLine("Authentication failed: " + context.Exception.Message);
						return Task.CompletedTask;
					},
					OnTokenValidated = context =>
					{
						Debug.WriteLine("Token is valid.");
						return Task.CompletedTask;
					}
				};
			});

		services.Configure<JwtModel>(configuration.GetSection(nameof(JwtModel)));
		services.Configure<AuthorizationOptions>(configuration.GetSection(nameof(AuthorizationOptions)));

		services.AddCors(options =>
		{
			options.AddDefaultPolicy(policy =>
			{
				policy.WithOrigins("https://localhost");
				policy.WithOrigins("https://localhost:7000");
				policy.WithOrigins("https://localhost:7003");
				policy.AllowAnyHeader();
				policy.AllowAnyMethod();
				policy.AllowCredentials();
			});
		});

		services.AddAuthorizationBuilder()
			.AddPolicy("AdminOnly", policy =>
			{
				policy.RequireRole("Admin");
				policy.AddRequirements(new ActiveAdminRequirement());
			})
			.AddPolicy("UserOnly", policy => policy.RequireRole("User"))
			.AddPolicy("UserOrAdmin", policy =>
			{
				policy.RequireRole("User", "Admin");
				policy.AddRequirements(new ActiveAdminRequirement());
			});

		services.AddScoped<IAuthorizationHandler, ActiveAdminHandler>();

		services.AddTransient(typeof(RequestLoggingPipelineBehavior<,>));

		services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

		return services;
	}

	public static WebApplicationBuilder UseHttps(this WebApplicationBuilder builder)
	{
		var environment = builder.Environment;

		var portString = Environment.GetEnvironmentVariable("PORT");

		if (string.IsNullOrEmpty(portString))
			portString = builder.Configuration.GetValue<string>("ApplicationSettings:Port");

		if (!int.TryParse(portString, out int port))
			throw new InvalidOperationException($"Invalid port value: {portString}");

		if (environment.IsProduction())
		{
			var certPath = "/app/localhost.pfx";
			var certPassword = "1";
			builder.WebHost.ConfigureKestrel(options =>
			{
				options.ListenAnyIP(port, listenOptions =>
				{
					listenOptions.UseHttps(certPath, certPassword);
				});
			});
		}
		else
		{
			builder.WebHost.ConfigureKestrel(options =>
			{
				options.ListenAnyIP(port, listenOptions =>
				{
					listenOptions.UseHttps();
				});
			});
		}

		return builder;
	}

	public static IHostBuilder AddLogging(this IHostBuilder hostBuilder, IConfiguration configuration)
	{
		var logstashPort = Environment.GetEnvironmentVariable("LOGSTASH_PORT");

		if (string.IsNullOrEmpty(logstashPort))
			logstashPort = configuration.GetValue<string>("ApplicationSettings:LogstashPort");

		hostBuilder.UseSerilog((context, config) =>
		{
			config
				.WriteTo.Console(outputTemplate: "{Timestamp:HH:mm} [{Level}] {Message}{NewLine}{Exception}")
				.WriteTo.Http($"http://logstash:{logstashPort}", queueLimitBytes: null);
		});

		return hostBuilder;
	}
}