using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Extensions.Authorization;

public static class AuthorizationExtension
{
	public static IServiceCollection AddAuthorization(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		var jwtOptions = configuration.GetSection(nameof(JwtModel)).Get<JwtModel>();

		services
			.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
				options =>
				{
					options.RequireHttpsMetadata = true;
					options.SaveToken = true;

					options.TokenValidationParameters = new TokenValidationParameters
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
						OnAuthenticationFailed = _ => Task.CompletedTask,
						OnTokenValidated = _ => Task.CompletedTask
					};
				});

		services.Configure<JwtModel>(configuration.GetSection(nameof(JwtModel)));

		services.Configure<AuthorizationOptions>(
			configuration.GetSection(nameof(AuthorizationOptions)));

		services.AddCors(options =>
		{
			options.AddDefaultPolicy(policy =>
			{
				policy.WithOrigins(
					"https://localhost",
					"http://localhost:3000",
					"https://localhost:3000",
					"https://localhost:7001",
					"https://localhost:7002",
					"https://localhost:7003"
				);
				policy.AllowAnyHeader();
				policy.AllowAnyMethod();
				policy.AllowCredentials();
			});
		});

		services.AddAuthorizationBuilder()
			.AddPolicy("AdminOnly",
				policy =>
				{
					policy.RequireRole("Admin");
					policy.AddRequirements(new ActiveAdminRequirement());
				})
			.AddPolicy("UserOnly", policy => policy.RequireRole("User"))
			.AddPolicy("UserOrAdmin",
				policy =>
				{
					policy.RequireRole("User", "Admin");
					policy.AddRequirements(new ActiveAdminRequirement());
				});

		services.AddScoped<IAuthorizationHandler, ActiveAdminHandler>();

		return services;
	}
}