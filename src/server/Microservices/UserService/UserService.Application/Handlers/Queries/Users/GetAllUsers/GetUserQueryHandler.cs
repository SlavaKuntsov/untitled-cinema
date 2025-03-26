using MapsterMapper;

using MediatR;

using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Models;

namespace UserService.Application.Handlers.Queries.Users.GetAllUsers;

public class GetUserQueryHandler(
	IUsersRepository usersRepository,
	IMapper mapper) : IRequestHandler<GetAllUsersQuery, IList<UserModel>>
{
	public async Task<IList<UserModel>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
	{
		var entities = await usersRepository.GetAsync(cancellationToken);

		return mapper.Map<IList<UserModel>>(entities);
	}
}