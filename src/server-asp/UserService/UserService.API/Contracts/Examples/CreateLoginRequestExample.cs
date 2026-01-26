using Swashbuckle.AspNetCore.Filters;

namespace UserService.API.Contracts.Examples;

public class CreateLoginRequestExample : IExamplesProvider<CreateLoginRequest>
{
	public CreateLoginRequest GetExamples()
	{
		return new CreateLoginRequest(
			"example@email.com",
			"qweQWE123");
	}
}