﻿using System.Reflection;
using System.Text;

using FluentValidation;

using Mapster;

using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using MovieService.Application.Validators;

using Serilog;

using Swashbuckle.AspNetCore.Filters;

using UserService.API.Behaviors;
using UserService.API.Consumers;
using UserService.API.Contracts.Examples;
using UserService.API.Middlewares;
using UserService.Application.Handlers.Commands.Users.UserRegistration;
using UserService.Infrastructure.Auth;
using UserService.Infrastructure.Notification;

namespace UserService.API.Extensions;

public static class ApiExtensions
{
	public static IServiceCollection AddAPI(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddHostedService<BookingPayConsumeService>();

		services.AddExceptionHandler<GlobalExceptionHandler>();
		services.AddProblemDetails();
		services.AddHealthChecks();

		services.AddGrpc(options =>
		{
			options.Interceptors.Add<GlobalGrpcExceptionInterceptor>();
		});

		services.AddHttpContextAccessor();

		services.AddControllers();
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen(options =>
		{
			options.ExampleFilters();
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
		services.AddSwaggerExamplesFromAssemblyOf<CreateUserRequestExample>();

		TypeAdapterConfig typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
		typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());

		Mapper mapperConfig = new(typeAdapterConfig);
		services.AddSingleton<IMapper>(mapperConfig);

		var jwtOptions = configuration.GetSection(nameof(JwtModel)).Get<JwtModel>();

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
						return Task.CompletedTask;
					},
					OnTokenValidated = context =>
					{
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
				policy.WithOrigins("http://localhost:7000");
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

		services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

		services.AddValidatorsFromAssemblyContaining<BaseCommandValidator<UserRegistrationCommand>>();

		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(SaveChangesBehavior<,>));

		return services;
	}

	public static void UseAPI(this WebApplication app)
	{
		app.MapGrpcService<Controllers.Grpc.AuthController>();

		app.MapHub<NotificationHub>("/notify");
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