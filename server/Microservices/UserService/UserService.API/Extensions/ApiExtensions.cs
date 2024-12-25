using System.Reflection;
using System.Text;

using FluentValidation;

using Mapster;

using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using UserService.API.Behaviors;
using UserService.API.Validators;
using UserService.Application.Handlers.Commands.Users;
using UserService.Infrastructure.Auth;

namespace UserService.API.Extensions;

public static class ApiExtensions
{
	public static IServiceCollection AddAPI(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddControllers();
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen(options =>
		{
			options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
			{
				Name = "Authorization",
				In = ParameterLocation.Header,
				Type = SecuritySchemeType.Http,
				Scheme = "Bearer"
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

		services.AddProblemDetails();

		services.AddHealthChecks();

		services.AddControllers(options =>
		{
			options.Filters.Add<GlobalRoleValidationFilter>();
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
						Console.WriteLine("Authentication failed: " + context.Exception.Message);
						return Task.CompletedTask;
					},
					OnTokenValidated = context =>
					{
						Console.WriteLine("Token is valid.");
						return Task.CompletedTask;
					},
					OnMessageReceived = context =>
					{
						context.Token = context.Request.Cookies[Application.Constants.JwtConstants.COOKIE_NAME];
						return Task.CompletedTask;
					}
				};
			});


		//services.Configure<JwtModel>(configuration.GetSection(nameof(JwtModel)));
		services.Configure<AuthorizationOptions>(configuration.GetSection(nameof(AuthorizationOptions)));


		services.AddCors(options =>
		{
			options.AddDefaultPolicy(policy =>
			{
				policy.WithOrigins("http://localhost:3000");
				policy.WithOrigins("http://localhost:5000");
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

		//services.AddStackExchangeRedisCache(options =>
		//{
		//	options.Configuration = configuration.GetConnectionString("RedisCache");
		//});

		services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

		services.AddValidatorsFromAssemblyContaining<BaseCommandValidator<UserRegistrationCommand>>();

		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

		return services;
	}
}
