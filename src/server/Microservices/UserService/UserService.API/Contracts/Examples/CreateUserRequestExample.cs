using Swashbuckle.AspNetCore.Filters;

namespace UserService.API.Contracts.Examples;
public class CreateUserRequestExample : IExamplesProvider<CreateUserRequest>
{
	public CreateUserRequest GetExamples()
	{
		return new CreateUserRequest(
			"example@email.com",
			"qweQWE123",
			"qweQWE123",
			"John",
			"Doe",
			"20-12-2020");
	}
}