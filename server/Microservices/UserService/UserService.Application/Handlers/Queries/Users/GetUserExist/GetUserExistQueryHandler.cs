using MapsterMapper;

using MediatR;

using UserService.Domain.Interfaces.Repositories;

namespace UserService.Application.Handlers.Queries.Users.GetUserExist;

public class GetUserExistQueryHandler(
	IUsersRepository usersRepository,
	IMapper mapper) : IRequestHandler<GetUserExistQuery, bool>
{
	private readonly IUsersRepository _usersRepository = usersRepository;
	private readonly IMapper _mapper = mapper;

	public async Task<bool> Handle(GetUserExistQuery request, CancellationToken cancellationToken)
	{
		//var entity = await _usersRepository.GetAsync(request.Id, cancellationToken)
		//	?? throw new NotFoundException("User not found");

		//return _mapper.Map<UserModel>(entity);

		return false;
	}
}