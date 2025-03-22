using BookingService.API.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Extensions.Common;

public static class CommonExtension
{
	public static WebApplication AddCommon(
		this WebApplication app,
		IServiceCollection services)
	{
		services.AddProblemDetails();
		
		services.AddHttpContextAccessor();

		services.AddControllers();
		
		services.AddEndpointsApiExplorer();
		
		app.UseHttpsRedirection();

		app.UseAuthentication();
		app.UseAuthorization();

		app.MapControllers();

		return app;
	}
}