namespace Utilities.Service;

public interface ICookieService
{
	void DeleteRefreshToken();
	string GetRefreshToken();
}