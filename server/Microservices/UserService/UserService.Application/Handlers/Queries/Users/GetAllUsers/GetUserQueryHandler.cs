using MapsterMapper;

using MediatR;

using UserService.Domain;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Application.Handlers.Queries.Users.GetAllUsers;

public class GetUserQueryHandler(IUsersRepository usersRepository, IMapper mapper) : IRequestHandler<GetAllUsersQuery, IList<UserModel>>
{
	private readonly IUsersRepository _usersRepository = usersRepository;
	private readonly IMapper _mapper = mapper;

	public async Task<IList<UserModel>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
	{
		var entities = await _usersRepository.GetAsync(cancellationToken);

		return _mapper.Map<IList<UserModel>>(entities);
	}
}