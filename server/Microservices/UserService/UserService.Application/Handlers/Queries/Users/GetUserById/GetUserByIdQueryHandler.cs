using MapsterMapper;

using MediatR;

using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Models;

namespace UserService.Application.Handlers.Queries.Users.GetUser;

public class GetUserByIdQueryHandler(IUsersRepository usersRepository, IMapper mapper) : IRequestHandler<GetUserByIdQuery, UserModel?>
{
	private readonly IUsersRepository _usersRepository = usersRepository;
	private readonly IMapper _mapper = mapper;

	public async Task<UserModel?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
	{
		var entity = await _usersRepository.GetAsync(request.Id, cancellationToken);

		return _mapper.Map<UserModel>(entity);
	}
}