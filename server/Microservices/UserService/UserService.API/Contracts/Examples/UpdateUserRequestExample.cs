using Swashbuckle.AspNetCore.Filters;

namespace UserService.API.Contracts.Examples;

public class UpdateUserRequestExample : IExamplesProvider<UpdateUserRequest>
{
	public UpdateUserRequest GetExamples()
	{
		return new UpdateUserRequest(
			Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
			"John",
			"Doe",
			"20-12-2020");
	}
}