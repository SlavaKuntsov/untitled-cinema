using Swashbuckle.AspNetCore.Filters;

namespace UserService.API.Contracts.Examples;

public class UpdateUserRequestExample : IExamplesProvider<UpdateUserRequest>
{
	public UpdateUserRequest GetExamples()
	{
		return new UpdateUserRequest(
			"John",
			"Doe",
			"20-12-2020");
	}
}