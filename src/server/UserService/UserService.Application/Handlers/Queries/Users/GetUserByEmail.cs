using MapsterMapper;
using MediatR;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Models;

namespace UserService.Application.Handlers.Queries.Users;

public record GetUserByEmailQuery(string Email) : IRequest<UserModel?>;

public class GetUserByEmailQueryHandler(
	IUsersRepository usersRepository,
	IMapper mapper)
	: IRequestHandler<GetUserByEmailQuery, UserModel?>
{
	public async Task<UserModel?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
	{
		var model = await usersRepository.GetAsync(
			request.Email,
			cancellationToken);

		if (model is null)
			return null;

		return mapper.Map<UserModel>(model);
	}
}