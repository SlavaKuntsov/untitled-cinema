using Mapster;

using MapsterMapper;

using MediatR;

using UserService.Application.Extensions;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Models;

namespace UserService.Application.Handlers.Commands.Users.UpdateUser;

public class UpdateUserCommandHandler(IUsersRepository usersRepository, IMapper mapper) : IRequestHandler<UpdateUserCommand, UserModel>
{
	private readonly IUsersRepository _usersRepository = usersRepository;
	private readonly IMapper _mapper = mapper;

	public async Task<UserModel> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
	{
		if (!request.DateOfBirth.DateFormatTryParse(out DateTime parsedDateTime))
			throw new BadRequestException("Invalid date format.");

		var userId = request.Id ?? 
			throw new ArgumentNullException(nameof(request.Id), "User ID is required.");

		var existUser = await _usersRepository.GetAsync(userId, cancellationToken)
				?? throw new NotFoundException($"User with id {request.Id} doesn't exists");

		request.Adapt(existUser);

		_usersRepository.Update(existUser);

		return _mapper.Map<UserModel>(existUser);
	}
}