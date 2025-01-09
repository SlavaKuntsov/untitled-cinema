using Microsoft.AspNetCore.Http;

using UserService.Application.Interfaces.Auth;

namespace UserService.Infrastructure.Auth;

public class CookieService : ICookieService
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public CookieService(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}

	public string GetRefreshToken()
	{
		var httpContext = _httpContextAccessor.HttpContext;
		if (httpContext == null)
			throw new InvalidOperationException("No active HTTP context available.");

		if (httpContext.Request.Cookies.TryGetValue(Domain.Constants.JwtConstants.COOKIE_NAME, out var refreshToken))
			return refreshToken;

		throw new InvalidOperationException("Refresh token not found in cookies.");
	}

	public void DeleteRefreshToken()
	{
		var httpContext = _httpContextAccessor.HttpContext;
		if (httpContext == null)
			throw new InvalidOperationException("No active HTTP context available.");

		httpContext.Response.Cookies.Delete(Domain.Constants.JwtConstants.COOKIE_NAME);
	}
}