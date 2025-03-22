using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using UserService.API.Extensions;
using UserService.Infrastructure.Auth;

namespace Extensions.Authorization;

public static class AuthorizationExtension
{
	public static WebApplication AddAuthorization(
		this WebApplication app,
		IServiceCollection services,
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

		app.UseCookiePolicy(new CookiePolicyOptions
		{
			MinimumSameSitePolicy = SameSiteMode.None,
			HttpOnly = HttpOnlyPolicy.Always,
			Secure = CookieSecurePolicy.Always
		});

		app.UseForwardedHeaders(new ForwardedHeadersOptions
		{
			ForwardedHeaders = ForwardedHeaders.All
		});
		app.UseCors();

		return app;
	}
}