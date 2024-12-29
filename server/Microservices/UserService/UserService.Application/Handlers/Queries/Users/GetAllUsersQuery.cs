using MediatR;

using UserService.Domain;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Application.Handlers.Queries.Users;

public class GetAllUsersQuery() : IRequest<IList<UserModel>>
{
	public class GetUserQueryHandler(IUsersRepository usersRepository) : IRequestHandler<GetAllUsersQuery, IList<UserModel>>
	{
		private readonly IUsersRepository _usersRepository = usersRepository;

		public async Task<IList<UserModel>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
		{
			return await _usersRepository.GetAsync(cancellationToken);
		}
	}
}